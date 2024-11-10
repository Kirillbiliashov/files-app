using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IFoldersRepository: IRepository<Folder>
    {

        List<Folder> GetSubfolders(string userId, string folderId);

        string? GetFolderName(string userId, string folderId);

        long GetSize(string userId, string id);

        Folder? GetByName(string userId, string name, string? parentId);

        long? GetLastModified(string userId, string id);

        bool ExistsByName(string userId, string folderId, string name);

        bool IsTrackedByName(string userId, string name, string folderId);

        string? GetFolderIdByName(string userId, string parentId, string name, bool isFolderAdded);
    }
}