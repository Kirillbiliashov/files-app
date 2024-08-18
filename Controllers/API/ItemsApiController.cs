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
        private readonly FilesStorage _filesStorage;
        private readonly FoldersStorage _foldersStorage;
        private readonly FilesAppDbContext _context;

        public ItemsApiController(FilesStorage filesStorage, FoldersStorage foldersStorage, FilesAppDbContext context)
        {
            _filesStorage = filesStorage;
            _foldersStorage = foldersStorage;
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

        private int DeleteFolderContents(List<string> folderIds)
        {
            var accumulator = folderIds.ToList();
            GetNestedFolderIds(folderIds, accumulator);

            var foldersCount = _foldersStorage.Remove(accumulator);
            var filesCount = _filesStorage.RemoveFilesByFolder(accumulator);

            return foldersCount + filesCount;
        }

        private void GetNestedFolderIds(List<string> folders, List<string> accumulator)
        {
            folders.ForEach(f =>
            {
                var subfolders = _foldersStorage.GetByFolder(f);
                if (subfolders != null)
                {
                    subfolders.ForEach(sf =>
                    {
                        accumulator.Add(sf.Id);
                        GetNestedFolderIds(new List<string> { sf.Id }, accumulator);
                    });
                }
            });
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
            _context.Entry(item).Property(x => x.Name).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("unstar")]
        public async Task<IActionResult> UnstarItem([FromBody] SelectedItem body)
        {
            Item item = body.Type == "file" ? new UserFile { Id = body.Id } : new Folder { Id = body.Id };
            _context.Items.Attach(item);
            item.IsStarred = false;
            _context.Entry(item).Property(x => x.Name).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}