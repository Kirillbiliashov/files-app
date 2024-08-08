using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers
{

    [ApiController]
    [Route("api/folders")]
    public class FoldersApiController : ControllerBase
    {
        private readonly FilesStorage _filesStorage;
        private readonly FoldersStorage _foldersStorage;

        public FoldersApiController(FilesStorage filesStorage, FoldersStorage foldersStorage)
        {
            _filesStorage = filesStorage;
            _foldersStorage = foldersStorage;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolderData(string id)
        {
            var folder = _foldersStorage.Get(id);
            var subfolders = _foldersStorage.GetByFolder(folder.Id);
            var files = _filesStorage.GetFolderFiles(id);

            return Ok(new { folder, subfolders, files });
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewFolder([FromBody] CreateFolderBody body)
        {
            var newFolder = new Folder 
            {
                Id = Guid.NewGuid().ToString(),
                Name = body.Name,
                FolderId = body.FolderId
            };
            _foldersStorage.Add(newFolder);

            return Created($"api/folders/{newFolder.Id}", new { folderId = newFolder.Id });
        }


    }
}