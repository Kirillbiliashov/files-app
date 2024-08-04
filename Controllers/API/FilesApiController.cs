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

        private readonly FilesStorage _storage;

        public FilesApiController(FilesStorage storage)
        {
            _storage = storage;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllFiles()
        {
            return Ok(_storage.GetAll());
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
                using (var memoryStream = file.OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    _storage.Add(new UserFile
                    {
                        Id = Guid.NewGuid().ToString(),
                        Filename = file.FileName,
                        Length = file.Length,
                        Content = buffer
                    });
                }
            }

            return Ok();
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _storage.Get(id);

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