using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers
{

    [ApiController]
    [Route("api/folders")]
    public class FoldersApiController : BaseApiController
    {
        public FoldersApiController(FilesStorage filesStorage, FoldersStorage foldersStorage) : base(filesStorage, foldersStorage)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolderData(string id)
        {
            var folder = _foldersStorage.Get(id);
            var subfolders = _foldersStorage.GetByFolder(folder.Id);
            var files = _filesStorage.GetFolderFiles(id);

            return Ok(new
            {
                folder,
                subfolders = subfolders.Select(f => new
                {
                    f.Id,
                    f.Name,
                    size = GetFolderSize(f.Id),
                    lastModified = GetFolderLastModified(f.Id),
                    f.IsStarred
                }),
                files
            });
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewFolder([FromBody] CreateFolderBody body)
        {
            var foldersCount = _foldersStorage.FoldersCount(body.Name);
            var folderName = foldersCount > 0 ? $"{body.Name} ({foldersCount})" : body.Name;
            var newFolder = new Folder
            {
                Id = Guid.NewGuid().ToString(),
                Name = folderName,
                FolderId = body.FolderId
            };
            _foldersStorage.Add(newFolder);

            return Created($"api/folders/{newFolder.Id}", new { folderId = newFolder.Id });
        }


    }
}