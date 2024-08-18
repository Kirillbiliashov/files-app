using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Controllers.API
{
    [ApiController]
    [Route("api/items")]
    public class ItemsApiController : ControllerBase
    {

        private readonly FilesAppDbContext _context;

        public ItemsApiController(FilesAppDbContext context)
        {
            _context = context;
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteItems([FromBody] SelectedItemsBody body)
        {
            var groupedItems = body.Items.GroupBy(i => i.Type);
            foreach (var group in groupedItems)
            {
                if (group.Key == "file")
                {
                    foreach (var item in group)
                    {
                        var fileToDelete = new UserFile { Id = item.Id };
                        _context.Entry(fileToDelete).State = EntityState.Deleted;
                    }
                }
                else if (group.Key == "folder")
                {
                    DeleteNestedFolderItems(group.Select(g => g.Id));
                    foreach (var item in group)
                    {
                        var folderToDelete = new Folder { Id = item.Id };
                        _context.Entry(folderToDelete).State = EntityState.Deleted;
                    }
                }
            }

            var deletedCount = await _context.SaveChangesAsync();

            return Ok(new { deletedCount });
        }

        private void DeleteNestedFolderItems(IEnumerable<string> folderIds)
        {
            if (folderIds == null || folderIds.Count() == 0)
            {
                return;
            }

            var nestedItems = _context.Items
            .Where(i => i.FolderId != null && folderIds.Contains(i.FolderId))
            .Select(i => new Item { Id = i.Id, FolderId = i.FolderId })
            .ToList();

            DeleteNestedFolderItems(nestedItems.Select(i => i.Id));
            nestedItems.ForEach(i => _context.Entry(i).State = EntityState.Deleted);
            _context.SaveChanges();
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadItems([FromBody] SelectedItemsBody body)
        {
            var files = new List<UserFile>();

            var groupedItems = body.Items.GroupBy(i => i.Type);
            foreach (var group in groupedItems)
            {
                var ids = group.Select(g => g.Id).ToList();
                if (group.Key == "file")
                {
                    var dbFiles = _context.Items.OfType<UserFile>().Where(f => ids.Contains(f.Id)).ToList();
                    files.AddRange(dbFiles);
                }
                else if (group.Key == "folder")
                {
                    // var folderIds = group.Select(g => g.Id).ToList();
                    // var accumulator = folderIds.ToList();
                    // GetNestedFolderIds(folderIds, accumulator);
                    var dbFiles = _context.Items.OfType<UserFile>().Where(f => f.FolderId != null && ids.Contains(f.FolderId)).ToList();
                    files.AddRange(dbFiles);
                }
            }

            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = archive.CreateEntry(file.Name, CompressionLevel.Fastest);
                        using (var zipStream = entry.Open())
                        {
                            zipStream.Write(file.Content, 0, file.Content.Length);
                        }
                    }
                }

                return File(ms.ToArray(), "application/zip", "files.zip");
            }

        }

        [HttpPatch("star")]
        public async Task<IActionResult> StarItem([FromBody] SelectedItem body)
        {
            Item item = body.Type == "file" ? new UserFile { Id = body.Id } : new Folder { Id = body.Id };
            _context.Items.Attach(item);
            item.IsStarred = true;
            _context.Entry(item).Property(x => x.IsStarred).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("unstar")]
        public async Task<IActionResult> UnstarItem([FromBody] SelectedItem body)
        {
            Item item = body.Type == "file" ? new UserFile { Id = body.Id } : new Folder { Id = body.Id };
            _context.Items.Attach(item);
            item.IsStarred = false;
            _context.Entry(item).Property(x => x.IsStarred).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}