using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesApiController : ControllerBase
    {

        private readonly FilesStorage _storage;

        public FilesApiController(FilesStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllFiles()
        {
            return Ok( _storage.GetAll());
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            foreach (var file in files)
            {
                _storage.Add(new UserFile 
                {
                    Filename = file.FileName,
                    Length = file.Length
                });
            }

            return Ok();
        }
    }
}