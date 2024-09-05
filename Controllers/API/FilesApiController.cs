using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using FilesApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeMapping;

namespace FilesApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesApiController : ControllerBase
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
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var items = _itemsRepository.GetTopLevelItems();

            return Ok(new
            {
                files = items.OfType<UserFile>().ToList(),
                folders = items.OfType<Folder>().ToList().Select(f => new
                {
                    f.Id,
                    f.Name,
                    size = _foldersRepository.GetSize(f.Id),
                    lastModified = _foldersRepository.GetLastModified(f.Id),
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

            var folderName = folder != null ? _foldersRepository.GetFolderName(folder) : null;

            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";

                var filename = folderName != null ? $"{folderName}/{files[i].FileName}" : files[i].FileName;
                string? folderId = SaveFolders(filename);

                using (var memoryStream = files[i].OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    _itemsRepository.Add(new UserFile
                    {
                        Name = files[i].FileName.Split("/").Last(),
                        Size = files[i].Length,
                        LastModified = long.Parse(lastModified[lastModifiedKey]),
                        Content = buffer,
                        FolderId = folderId
                    });
                }
            }
            await _itemsRepository.SaveAsync();

            return Ok();
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
                var isFolderAdded = _foldersRepository.IsTrackedByName(folderName);

                if (!(isFolderAdded || _foldersRepository.ExistsByName(folderName)))
                {
                    var folder = new Folder
                    {
                        Name = folderName,
                        FolderId = folderId
                    };
                    _itemsRepository.Add(folder);
                    folderId = folder.Id;
                }
                else
                {
                    folderId = _foldersRepository.GetFolderIdByName(folderName, isFolderAdded);
                }
            });

            return folderId;
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _filesRepository.Get(id);
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
