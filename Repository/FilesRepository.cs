using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Repository
{
    public class FilesRepository : BaseRepository<UserFile>, IFilesRepository
    {

        public FilesRepository(FilesAppDbContext context) : base(context)
        {
        }

        override public UserFile? Get(string userId, string id) =>
         _context.Items
         .OfType<UserFile>()
         .Where(f => f.UserId == userId && f.Id == id)
         .Select(f => new UserFile { Name = f.Name, Content = f.Content })
         .AsNoTracking()
         .FirstOrDefault();

        public List<string> GetExistingFilesHashes(string userId, IEnumerable<string> newHashes) =>
        _context.Items
        .AsNoTracking()
        .OfType<UserFile>()
        .Where(f => f.UserId == userId && newHashes.Contains(f.Hash))
        .Select(f => f.Hash)
        .ToList();

    }
}