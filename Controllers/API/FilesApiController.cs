using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeMapping;

namespace FilesApp.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesApiController : BaseApiController
    {
        private const string _allFilesQuery = "SELECT Id, Discriminator, FolderId, IsStarred, Name, NULL AS Content, LastModified, Size FROM Items WHERE FolderId IS NULL";
        public FilesApiController(FilesAppDbContext context) : base(context)
        {
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllFiles()
        {
            var items = _context.Items.FromSqlRaw(_allFilesQuery).ToList();

            return Ok(new
            {
                files = items.OfType<UserFile>().ToList(),
                folders = items.OfType<Folder>().ToList().Select(f => new
                {
                    f.Id,
                    f.Name,
                    size = GetFolderSize(f.Id),
                    lastModified = GetFolderLastModified(f.Id),
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

            for (int i = 0; i < files.Count; i++)
            {
                var lastModifiedKey = $"lastModified_{i}";
                var folderName = folder != null ? _context.Items
                .OfType<Folder>()
                .Where(f => f.Id == folder)
                .Select(f => f.Name)
                .FirstOrDefault() : null;

                var filename = folderName != null ? $"{folderName}/{files[i].FileName}" : files[i].FileName;
                string? folderId = SaveFolders(filename);

                using (var memoryStream = files[i].OpenReadStream())
                {
                    byte[] buffer = new byte[memoryStream.Length];
                    memoryStream.Read(buffer, 0, buffer.Length);
                    _context.Items.Add(new UserFile
                    {
                        Name = files[i].FileName.Split("/").Last(),
                        Size = files[i].Length,
                        LastModified = long.Parse(lastModified[lastModifiedKey]),
                        Content = buffer,
                        FolderId = folderId
                    });
                }
            }
            await _context.SaveChangesAsync();

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
                var isFolderAdded = _context.ChangeTracker.Entries<Folder>()
                .Where(f => f.Entity.Name == folderName)
                .Any();

                if (!(isFolderAdded || _context.Items.OfType<Folder>().Any(i => i.Name == folderName)))
                {
                    var folder = new Folder
                    {
                        Name = folderName,
                        FolderId = folderId
                    };
                    _context.Items.Add(folder);
                    folderId = folder.Id;
                }
                else
                {
                    var folders = isFolderAdded ?
                    _context.ChangeTracker.Entries<Folder>().Select(e => e.Entity) :
                    _context.Items.OfType<Folder>();

                    folderId = folders
                    .Where(f => f.Name == folderName)
                    .Select(f => f.Id)
                    .FirstOrDefault();
                }
            });

            return folderId;
        }

        [HttpGet("open/{id}")]
        public async Task<IActionResult> OpenFile(string id)
        {
            var file = _context.Items
            .OfType<UserFile>()
            .Where(f => f.Id == id)
            .Select(f => new UserFile { Name = f.Name, Content = f.Content })
            .FirstOrDefault();

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
