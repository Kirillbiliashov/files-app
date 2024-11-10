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
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public FilesApiController(IItemsRepository itemsRepository, IFoldersRepository foldersRepository, IFilesRepository filesRepository, ISharedLinkRepository sharedLinkRepository)
        {
            _itemsRepository = itemsRepository;
            _foldersRepository = foldersRepository;
            _filesRepository = filesRepository;
            _sharedLinkRepository = sharedLinkRepository;
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
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromForm] Dictionary<string, string> lastModified, [FromForm] string? folderId)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            var filesToAdd = new List<UserFile>();
            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";
                var filename = files[i].FileName;
                string? fileFolderId = SaveFolders(folderId, files[i].FileName.Split("/").SkipLast(1));

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
                        FolderId = fileFolderId
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
//com.apple.Photos.NSItemProvider/uuid=62C755BB-8E4D-4695-BBA9-0C80C6C09158&library=1&type=1&mode=1&loc=true&cap=true.jpeg/photo-1570295999919-56ceb5ecca61.jpeg
//com.apple.Photos.NSItemProvider/uuid=17E5EED1-5323-44AB-A680-93E528F01DE1&library=1&type=1&mode=1&loc=true&cap=true.jpeg/tree-736885_1280.jpeg

        private string? SaveFolders(string? folderId, IEnumerable<string>folders)
        {
            if (folders.Count() == 0)
            {
                return folderId;
            }

            // string? folderId = null;

            var currentFolderId = folderId;

            folders.ToList().ForEach(folderName =>
            {
                var isFolderAdded = _foldersRepository.IsTrackedByName(UserId, folderName, currentFolderId);
                if (!(isFolderAdded || _foldersRepository.ExistsByName(UserId, currentFolderId, folderName)))
                {
                    var folder = new Folder
                    {
                        UserId = UserId,
                        Name = folderName,
                        FolderId = currentFolderId
                    };
                    _itemsRepository.Add(folder);
                    currentFolderId = folder.Id;
                }
                else
                {
                    currentFolderId = _foldersRepository.GetFolderIdByName(UserId, currentFolderId, folderName, isFolderAdded);
                }
            });


            return currentFolderId;
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _filesRepository.Get(UserId, id);
            if (file == null)
            {
                return NotFound();
            }

            return GetFileContent(file);
        }

        [HttpGet("shared/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> OpenSharedFile(string id)
        {
            var sharedLink = _sharedLinkRepository.Get(UserId, id);
            if (sharedLink == null)
            {
                return NotFound();
            }

            return GetFileContent(sharedLink.Item);
        }

        private FileContentResult GetFileContent(UserFile file)
        {
            Response.Headers.Add("Content-Disposition", "inline; filename=" + file.Name);
            string contentType = MimeUtility.GetMimeMapping(file.Name);

            return File(file.Content, contentType);
        }

    }
}
