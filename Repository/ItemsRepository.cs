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
    public class ItemsRepository : IItemsRepository
    {
        private const string _allItemsQuery = "SELECT Id, Discriminator, FolderId, IsStarred, Name, NULL AS Content, LastModified, Size FROM Items WHERE FolderId IS NULL";

        private readonly FilesAppDbContext _context;

        public ItemsRepository(FilesAppDbContext context) => _context = context;

        public void Add(Item item)
        {
            _context.Items.Add(item);
        }

        public void Delete(Item item)
        {
            _context.Entry(item).State = EntityState.Deleted;
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

        public Task<int> SaveAsync() => _context.SaveChangesAsync();

        public void Update<T>(Item item, Expression<Func<Item, T>> property, T newValue)
        {
            _context.Items.Attach(item);
            _context.Entry(item).Property(property).CurrentValue = newValue;
            _context.Entry(item).Property(property).IsModified = true;
        }
    }
}