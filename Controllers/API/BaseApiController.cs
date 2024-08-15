using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FilesApp.Controllers.API
{
    public class BaseApiController : ControllerBase
    {

        protected readonly FilesStorage _filesStorage;
        protected readonly FoldersStorage _foldersStorage;

        public BaseApiController(FilesStorage filesStorage, FoldersStorage foldersStorage)
        {
            _filesStorage = filesStorage;
            _foldersStorage = foldersStorage;
        }

        protected long GetFolderSize(string folderId)
        {
            var filesSize = _filesStorage.GetFolderSize(folderId);
            var subFolders = _foldersStorage.GetByFolder(folderId);

            if (subFolders.Count == 0)
            {
                return filesSize;
            }

            return filesSize + subFolders.Select(sf => GetFolderSize(sf.Id)).Sum();
        }

        protected long? GetFolderLastModified(string folderId)
        {
            var filesLastModified = _filesStorage.GetFolderLastModified(folderId);
            var subFolders = _foldersStorage.GetByFolder(folderId);

            if (subFolders.Count == 0)
            {
                return filesLastModified;
            }

            var subfoldersLastModified = subFolders.Select(sf => GetFolderLastModified(sf.Id)).Max();

            if (filesLastModified == null || subfoldersLastModified == null)
            {
                return null;
            }

            return Math.Max((long)filesLastModified, (long)subfoldersLastModified);
        }
    }
}