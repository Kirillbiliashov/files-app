using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Repository
{
    public class ItemsRepository : BaseRepository<Item>, IItemsRepository
    {
        private const string _allItemsQuery = "SELECT Id, Discriminator, FolderId, IsStarred, Name, NULL AS Content, LastModified, Size FROM Items WHERE FolderId IS NULL";

        public ItemsRepository(FilesAppDbContext context) : base(context)
        {
        }

        public void DeleteMany(List<Item> items)
        {
            items.ForEach(i => _context.Entry(i).State = EntityState.Deleted);
        }

        public List<Item> GetByFolderIds(IEnumerable<string> ids) => _context.Items
            .Where(i => i.FolderId != null && ids.Contains(i.FolderId))
            .Select(i =>  new Item { Id = i.Id, FolderId = i.FolderId })
            .ToList();

        public List<Item> GetAllByFolderIds(IEnumerable<string> ids) => 
        _context.Items.Where(i => i.FolderId != null && ids.Contains(i.FolderId)).ToList();

        public List<Item> GetTopLevelItems() => _context.Items.FromSqlRaw(_allItemsQuery).ToList();

    }
}