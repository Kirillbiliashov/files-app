using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.DAL
{
    public class FoldersStorage
    {
        private readonly List<Folder> _folders = new List<Folder>();

        public bool Exists(string folder) => _folders.Any(f => f.Name == folder);

        public void Add(Folder folder)
        {
            _folders.Add(folder);
        }

        public List<Folder> GetTopLevelFolders()
        {
            var matches = _folders.Where(f => f.FolderId == null).ToList();
            Folder[] arr = new Folder[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }

        public string? GetFolderId(string folder)
        {
            return _folders.Where(f => f.Name == folder).Select(f => f.Id).FirstOrDefault();
        }

        public Folder? Get(string folderId) => _folders.Where(f => f.Id == folderId).FirstOrDefault();

        public List<Folder> GetByFolder(string folderId) 
        {
            var matches = _folders.Where(f => f.FolderId == folderId).ToList();
            Folder[] arr = new Folder[matches.Count];
            matches.CopyTo(arr);

            return arr.ToList();
        }

    }
}