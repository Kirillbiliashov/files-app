using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IFoldersRepository
    {
        Folder? Get(string id);

        string? GetFolderName(string folderId);

        long GetSize(string id);

        int GetCount(string name);

        long? GetLastModified(string id);

        bool ExistsByName(string name);

        bool IsTrackedByName(string name);

        string? GetFolderIdByName(string name, bool isFolderAdded);
    }
}