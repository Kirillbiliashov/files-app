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

        public UserFile? Get(string id) => _files.Where(f => f.Id == id).FirstOrDefault();
    }
}