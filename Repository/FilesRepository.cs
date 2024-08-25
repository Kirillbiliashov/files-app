using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public class FilesRepository : IFilesRepository
    {

        private readonly FilesAppDbContext _context;

        public FilesRepository(FilesAppDbContext context)
        {
            _context = context;
        }

        public void Add(UserFile file)
        {
            throw new NotImplementedException();
        }

        public UserFile? Get(string id) => _context.Items
            .OfType<UserFile>()
            .Where(f => f.Id == id)
            .Select(f => new UserFile { Name = f.Name, Content = f.Content })
            .FirstOrDefault();


        public List<UserFile> GetAll(List<string> ids) =>
         _context.Items.OfType<UserFile>().Where(f => ids.Contains(f.Id)).ToList();

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}