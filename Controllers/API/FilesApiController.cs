using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using FilesApp.Attributes;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.Auth;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using FilesApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeMapping;

namespace FilesApp.Controllers
{
    [AllowOnlyAuthorized]
    [ApiController]
    [Route("api/files")]
    public class FilesApiController : BaseApiController
    {

        private readonly IItemsRepository _itemsRepository;
        private readonly IFoldersRepository _foldersRepository;
        private readonly IFilesRepository _filesRepository;

        public FilesApiController(IItemsRepository itemsRepository, IFoldersRepository foldersRepository, IFilesRepository filesRepository)
        {
            _itemsRepository = itemsRepository;
            _foldersRepository = foldersRepository;
            _filesRepository = filesRepository;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAllFiles()
        {
            var items = _itemsRepository.GetTopLevelItems(UserId);
            return Ok(new
            {
                files = items.OfType<UserFile>().ToList(),
                folders = items.OfType<Folder>().ToList().Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.NameIdx,
                    size = _foldersRepository.GetSize(UserId, f.Id),
                    lastModified = _foldersRepository.GetLastModified(UserId, f.Id),
                    f.IsStarred
                })
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromForm] Dictionary<string, string> lastModified, [FromForm] string? folder)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            var folderName = folder != null ? _foldersRepository.GetFolderName(UserId, folder) : null;
            var filesToAdd = new List<UserFile>();
            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";

                var filename = folderName != null ? $"{folderName}/{files[i].FileName}" : files[i].FileName;
                string? folderId = SaveFolders(filename);
                filename = files[i].FileName.Split("/").Last();

                using (var memoryStream = files[i].OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    var hash = ComputeFileHash(buffer, filename, folderId);
                    filesToAdd.Add(new UserFile
                    {
                        UserId = UserId,
                        Name = files[i].FileName.Split("/").Last(),
                        Size = files[i].Length,
                        LastModified = long.Parse(lastModified[lastModifiedKey]),
                        Content = buffer,
                        Hash = hash,
                        FolderId = folderId
                    });
                }
            }

            var existingHashes = _filesRepository.GetExistingFilesHashes(UserId, filesToAdd.Select(f => f.Hash));
            foreach (var fileToAdd in filesToAdd)
            {
                if (!existingHashes.Contains(fileToAdd.Hash))
                {
                    _filesRepository.Add(fileToAdd);
                }
            }

            await _itemsRepository.SaveAsync();

            return Ok();
        }

        private string ComputeFileHash(byte[] fileBytes, string fileName, string? folderId)
        {
            using (var sha256 = SHA256.Create())
            {
                var fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                var folderIdBytes = string.IsNullOrWhiteSpace(folderId) ?
                 new byte[] { } : System.Text.Encoding.UTF8.GetBytes(folderId);

                var combinedBytes = new byte[fileBytes.Length + fileNameBytes.Length + folderIdBytes.Length];
                Buffer.BlockCopy(fileBytes, 0, combinedBytes, 0, fileBytes.Length);
                Buffer.BlockCopy(fileNameBytes, 0, combinedBytes, fileBytes.Length, fileNameBytes.Length);
                Buffer.BlockCopy(folderIdBytes, 0, combinedBytes, fileBytes.Length + fileNameBytes.Length, folderIdBytes.Length);

                var hashBytes = sha256.ComputeHash(combinedBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string? SaveFolders(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.Contains("/"))
            {
                return null;
            }

            string? folderId = null;
            var parts = path.Split("/");
            var folderPaths = parts.SkipLast(1).ToList();

            folderPaths.ForEach(folderName =>
            {
                var isFolderAdded = _foldersRepository.IsTrackedByName(UserId, folderName);

                if (!(isFolderAdded || _foldersRepository.ExistsByName(UserId, folderName)))
                {
                    var folder = new Folder
                    {
                        UserId = UserId,
                        Name = folderName,
                        FolderId = folderId
                    };
                    _itemsRepository.Add(folder);
                    folderId = folder.Id;
                }
                else
                {
                    folderId = _foldersRepository.GetFolderIdByName(UserId, folderName, isFolderAdded);
                }
            });

            return folderId;
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _filesRepository.Get(UserId, id);
            if (file == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=" + file.Name);
            string contentType = MimeUtility.GetMimeMapping(file.Name);

            return File(file.Content, contentType);
        }

    }
}
