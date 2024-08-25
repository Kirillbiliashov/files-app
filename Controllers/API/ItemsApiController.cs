using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using FilesApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Controllers.API
{
    [ApiController]
    [Route("api/items")]
    public class ItemsApiController : ControllerBase
    {

        private readonly IItemsRepository _itemsRepository;
        private readonly IFilesRepository _filesRepository;

        public ItemsApiController(IItemsRepository itemsRepository, IFilesRepository filesRepository)
        {
            _itemsRepository = itemsRepository;
            _filesRepository = filesRepository;
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
                        _itemsRepository.Delete(new UserFile { Id = item.Id });
                    }
                }
                else if (group.Key == "folder")
                {
                    DeleteNestedFolderItems(group.Select(g => g.Id));
                    foreach (var item in group)
                    {
                        _itemsRepository.Delete(new Folder { Id = item.Id });
                    }
                }
            }

            var deletedCount = await _itemsRepository.SaveAsync();

            return Ok(new { deletedCount });
        }

        private void DeleteNestedFolderItems(IEnumerable<string> folderIds)
        {
            if (folderIds == null || folderIds.Count() == 0)
            {
                return;
            }

            var nestedItems = _itemsRepository.GetByFolderIds(folderIds);
            DeleteNestedFolderItems(nestedItems.Select(i => i.Id));
            _itemsRepository.DeleteMany(nestedItems);
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
                    var dbFiles = _filesRepository.GetAll(ids);
                    files.AddRange(dbFiles);
                }
                else if (group.Key == "folder")
                {
                    var dbFiles = GetNestedFolderItems(ids);
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

        private List<UserFile> GetNestedFolderItems(IEnumerable<string> folderIds)
        {
            if (folderIds == null || folderIds.Count() == 0)
            {
                return new List<UserFile>();
            }

            var nestedItems = _itemsRepository.GetAllByFolderIds(folderIds);

            return nestedItems
            .OfType<UserFile>()
            .Concat(GetNestedFolderItems(nestedItems.OfType<Folder>().Select(f => f.Id)))
            .ToList();
        }

        [HttpPatch("star")]
        public async Task<IActionResult> StarItem([FromBody] SelectedItem body)
        {
            Item item = new Item { Id = body.Id };
            _itemsRepository.Update(item, i => i.IsStarred, true);
            await _itemsRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("unstar")]
        public async Task<IActionResult> UnstarItem([FromBody] SelectedItem body)
        {
            var item = new Item { Id = body.Id };
            _itemsRepository.Update(item, i => i.IsStarred, false);
            await _itemsRepository.SaveAsync();

            return NoContent();
        }
    }
}