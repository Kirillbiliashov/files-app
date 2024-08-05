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
            var files = _storage.GetAll();
            return Ok(files.GroupBy(f => f.Folder).Select(g => new 
            {
                Folder = g.Key,
                Files = g
            }));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, [FromForm] Dictionary<string, string> lastModified, [FromForm] string? folder)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest();
            }

            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";
                using (var memoryStream = files[i].OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    Console.WriteLine($"folder: {folder}");
                    _storage.Add(new UserFile
                    {
                        Id = Guid.NewGuid().ToString(),
                        Filename = files[i].FileName,
                        Length = files[i].Length,
                        Modified =long.Parse(lastModified[lastModifiedKey]),
                        Content = buffer,
                        Folder = folder
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