using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Attributes;
using FilesApp.Controllers.API;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Models.Http;
using FilesApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Controllers
{

    [AllowOnlyAuthorized]
    [ApiController]
    [Route("api/folders")]
    public class FoldersApiController : BaseApiController
    {
        private readonly IFoldersRepository _foldersRepository;

        public FoldersApiController(IFoldersRepository foldersRepository) => _foldersRepository = foldersRepository;
        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolderData(string id)
        {
            var folder = _foldersRepository.Get(UserId, id);
            var subfolders = folder?.Items.OfType<Folder>();
            var files = folder?.Items.OfType<UserFile>();

            return Ok(new
            {
                folder,
                subfolders = subfolders?.ToList().Select(f => new
                {
                    f.Id,
                    f.Name,
                    size = _foldersRepository.GetSize(UserId, f.Id),
                    lastModified = _foldersRepository.GetLastModified(UserId, f.Id),
                    f.IsStarred
                }),
                files
            });
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewFolder([FromBody] CreateFolderBody body)
        {
            var existingFolder = _foldersRepository.GetByName(UserId, body.Name, body.FolderId);
            var newFolder = new Folder
            {
                UserId = UserId,
                Name = body.Name,
                FolderId = body.FolderId,
                NameIdx = existingFolder == null ? 0 : existingFolder.NameIdx + 1
            };
            _foldersRepository.Add(newFolder);
            await _foldersRepository.SaveAsync();

            return Created($"api/folders/{newFolder.Id}", new { folderId = newFolder.Id });
        }


    }
}