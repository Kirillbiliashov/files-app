using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IFilesRepository
    {
        void Add(UserFile file);

        void Save();

        UserFile? Get(string id);

        List<UserFile> GetAll(List<string> ids);
    }
}