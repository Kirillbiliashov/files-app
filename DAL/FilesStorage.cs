using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.DAL
{
    public class FilesStorage
    {
        private readonly List<UserFile> _files = new List<UserFile>();

        public UserFile? Get(string fileId) => _files.Where(f => f.Id == fileId).FirstOrDefault();

        public void Add(UserFile file)
        {
            _files.Add(file);
        }

        public List<UserFile> GetAll()
        {
            UserFile[] arr = new UserFile[_files.Count];
            _files.CopyTo(arr);

            return arr.ToList();
        }

        public List<UserFile> GetTopLevelFiles()
        {
            var matches = _files.Where(f => f.FolderId == null).ToList();
            UserFile[] arr = new UserFile[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }


        public List<UserFile> GetFilesByIds(List<string> ids)
        {
            var matches = _files.Where(f => ids.Contains(f.Id)).ToList();
            UserFile[] arr = new UserFile[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }

        public List<UserFile> GetFolderFiles(string folderId)
        {
            var matches = _files.Where(f => f.FolderId == folderId).ToList();
            UserFile[] arr = new UserFile[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }

        public List<UserFile> GetFoldersFiles(List<string> folderIds)
        {
            var matches = _files.Where(f => f.FolderId != null && folderIds.Contains(f.FolderId)).ToList();
            UserFile[] arr = new UserFile[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }

        public long GetFolderSize(string folderId) => _files.Where(f => f.FolderId == folderId).Sum(f => f.Length);

        public long? GetFolderLastModified(string folderId) => _files.Where(f => f.FolderId == folderId).MaxBy(f => f.Modified)?.Modified;

        public int RemoveFiles(List<string> ids) => _files.RemoveAll(f => ids.Contains(f.Id));

        public int RemoveFilesByFolder(List<string> folderIds) => _files.RemoveAll(f => f.FolderId != null && folderIds.Contains(f.FolderId));
        
    }
}