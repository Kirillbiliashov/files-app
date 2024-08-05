using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;

namespace FilesApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesApiController : ControllerBase
    {

        private readonly FilesStorage _filesStorage;
        private readonly FoldersStorage _foldersStorage;

        public FilesApiController(FilesStorage filesStorage, FoldersStorage foldersStorage)
        {
            _filesStorage = filesStorage;
            _foldersStorage = foldersStorage;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = _filesStorage.GetTopLevelFiles();
            var folders = _foldersStorage.GetTopLevelFolders();

            return Ok(new { files, folders });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromForm] Dictionary<string, string> lastModified, [FromForm] Dictionary<string, string> paths)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";
                var pathKey = $"paths_{i}";
                string? folderId = paths.ContainsKey(pathKey) ? SaveFolders(paths[pathKey]) : null;

                using (var memoryStream = files[i].OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    _filesStorage.Add(new UserFile
                    {
                        Id = Guid.NewGuid().ToString(),
                        Filename = files[i].FileName,
                        Length = files[i].Length,
                        Modified = long.Parse(lastModified[lastModifiedKey]),
                        Content = buffer,
                        FolderId = folderId
                    });
                }
            }
            return Ok();
        }

        private string? SaveFolders(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            string? folderId = null;
            var parts = path.Split("/");
            var foundTopLevelFolder = false;
            parts.SkipLast(1).ToList().ForEach(folder =>
            {
                if (!_foldersStorage.Exists(folder))
                {
                    folderId = Guid.NewGuid().ToString();
                    _foldersStorage.Add(new Folder
                    {
                        Id = folderId,
                        Name = folder,
                        IsTopLevel = !foundTopLevelFolder
                    });
                }
                if (!foundTopLevelFolder)
                {
                    foundTopLevelFolder = true;
                }
            });

            return folderId;
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _filesStorage.Get(id);

            if (file == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=" + file.Filename);
            string contentType = MimeUtility.GetMimeMapping(file.Filename);

            return File(file.Content, contentType);
        }
    }
}