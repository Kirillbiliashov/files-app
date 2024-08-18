using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Controllers
{

    [ApiController]
    [Route("api/folders")]
    public class FoldersApiController : BaseApiController
    {
       private readonly  FilesAppDbContext _context;
        public FoldersApiController(FilesStorage filesStorage, FoldersStorage foldersStorage, FilesAppDbContext context) : base(filesStorage, foldersStorage)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolderData(string id)
        {
            var folder = _context.Items.Where(i => i.Id == id).OfType<Folder>().Include(f => f.Items).FirstOrDefault();
            var subfolders = folder?.Items.OfType<Folder>();
            var files = folder?.Items.OfType<UserFile>();

            return Ok(new
            {
                folder,
                subfolders,
                files
            });
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewFolder([FromBody] CreateFolderBody body)
        {
            var foldersCount = _context.Items.OfType<Folder>().Where(f => f.Name == body.Name).Count();

            var folderName = foldersCount > 0 ? $"{body.Name} ({foldersCount})" : body.Name;
            var newFolder = new Folder
            {
                Name = folderName,
                FolderId = body.FolderId
            };
            _context.Items.Add(newFolder);
            await _context.SaveChangesAsync();

            return Created($"api/folders/{newFolder.Id}", new { folderId = newFolder.Id });
        }


    }
}