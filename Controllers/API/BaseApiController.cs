using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers.API
{
    public class BaseApiController : ControllerBase
    {
        protected readonly FilesAppDbContext _context;

        public BaseApiController(FilesAppDbContext context) => _context = context;

        protected long GetFolderSize(string folderId)
        {
            if (folderId == null)
            {
                return 0;
            }

            var filesSize = _context.Items
            .Where(i => i.FolderId == folderId)
            .OfType<UserFile>()
            .Select(i => i.Size)
            .Sum();

            var subfoldersIds = _context.Items
            .Where(i => i.FolderId == folderId)
            .OfType<Folder>()
            .Select(i => i.Id)
            .ToList();

            return filesSize + subfoldersIds.Select(GetFolderSize).Sum();
        }

        protected long? GetFolderLastModified(string folderId)
        {
           if (folderId == null)
            {
                return null;
            }

            var filesLastModified = _context.Items
            .Where(i => i.FolderId == folderId)
            .OfType<UserFile>()
            .Select(i => i.LastModified)
            .Max();
            
            var subfoldersIds = _context.Items
            .Where(i => i.FolderId == folderId)
            .OfType<Folder>()
            .Select(i => i.Id)
            .ToList();

            if (subfoldersIds.Count == 0)
            {
                return filesLastModified;
            }

            var subfoldersLastModified = subfoldersIds.Select(GetFolderLastModified).Max();
            if (filesLastModified == 0 || subfoldersLastModified == null)
            {
                return null;
            }

            return Math.Max(filesLastModified, (long) subfoldersLastModified);
        }
    }
}