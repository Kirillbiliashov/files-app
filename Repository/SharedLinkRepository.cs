using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Repository
{
    public class SharedLinkRepository : BaseRepository<SharedLink>, ISharedLinkRepository
    {
        public SharedLinkRepository(FilesAppDbContext context) : base(context)
        {
        }

        override public SharedLink? Get(string userId, string id) =>
         _context.SharedLinks
         .Where(l => l.Id == id)
         .Include(l => l.Item)
         .FirstOrDefault();

    }
}