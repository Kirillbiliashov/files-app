using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IItemsRepository
    {
        void Delete(Item item);

        void Add(Item item);

        void DeleteMany(List<Item> items);

        List<Item> GetTopLevelItems();

        List<Item> GetByFolderIds(IEnumerable<string> ids);

        List<Item> GetAllByFolderIds(IEnumerable<string> ids);

        Task<int> SaveAsync();

        void Update<T>(Item item, Expression<Func<Item, T>> property, T newValue);
    }
}