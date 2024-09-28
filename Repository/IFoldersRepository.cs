using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IFoldersRepository: IRepository<Folder>
    {
        string? GetFolderName(string userId, string folderId);

        long GetSize(string userId, string id);

        Folder? GetByName(string userId, string name, string? parentId);

        long? GetLastModified(string userId, string id);

        bool ExistsByName(string userId, string name);

        bool IsTrackedByName(string userId, string name);

        string? GetFolderIdByName(string userId, string name, bool isFolderAdded);
    }
}