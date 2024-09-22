using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IItemsRepository: IRepository<Item>
    {

        void DeleteMany(List<Item> items);

        List<Item> GetTopLevelItems(string userId);

        List<Item> GetByFolderIds(string userId, IEnumerable<string> ids);

        List<Item> GetAllByFolderIds(string userId, IEnumerable<string> ids);
    }
}