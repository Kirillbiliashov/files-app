using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers.API
{
    [ApiController]
    [Route("api/items")]
    public class ItemsApiController : ControllerBase
    {
        private readonly FilesStorage _filesStorage;
        private readonly FoldersStorage _foldersStorage;

        public ItemsApiController(FilesStorage filesStorage, FoldersStorage foldersStorage)
        {
            _filesStorage = filesStorage;
            _foldersStorage = foldersStorage;
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteItems([FromBody] SelectedItemsBody body)
        {
            var groupedItems = body.Items.GroupBy(i => i.Type);
            var deletedCount = 0;
            foreach (var group in groupedItems)
            {
                if (group.Key == "file")
                {
                    deletedCount += _filesStorage.RemoveFiles(group.Select(g => g.Id).ToList());
                }
                else if (group.Key == "folder")
                {
                    deletedCount += DeleteFolderContents(group.Select(g => g.Id).ToList());
                }
            }

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
                if (group.Key == "file")
                {
                    files.AddRange(_filesStorage.GetFilesByIds(group.Select(g => g.Id).ToList()));
                }
                else if (group.Key == "folder")
                {
                    var folderIds = group.Select(g => g.Id).ToList();
                    var accumulator = folderIds.ToList();
                    GetNestedFolderIds(folderIds, accumulator);
                    files.AddRange(_filesStorage.GetFoldersFiles(accumulator));
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
            if (body.Type == "file")
            {
                var file = _filesStorage.Get(body.Id);
                if (file != null)
                {
                    file.IsStarred = true;
                }
            }
            else if (body.Type == "folder")
            {
                var folder = _foldersStorage.Get(body.Id);
                if (folder != null)
                {
                    folder.IsStarred = true;
                }
            }

            return NoContent();
        }

        [HttpPatch("unstar")]
        public async Task<IActionResult> UnstarItem([FromBody] SelectedItem body)
        {
            if (body.Type == "file")
            {
                var file = _filesStorage.Get(body.Id);
                if (file != null)
                {
                    file.IsStarred = false;
                }
            }
            else if (body.Type == "folder")
            {
                var folder = _foldersStorage.Get(body.Id);
                if (folder != null)
                {
                    folder.IsStarred = false;
                }
            }

            return NoContent();
        }
    }
}