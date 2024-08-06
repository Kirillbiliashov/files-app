using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
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
            var files = _filesStorage.GetFolderFiles(id);

            return Ok(new { folder, files });
        }


    }
}