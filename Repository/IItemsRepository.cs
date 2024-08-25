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

        List<Item> GetTopLevelItems();

        List<Item> GetByFolderIds(IEnumerable<string> ids);

        List<Item> GetAllByFolderIds(IEnumerable<string> ids);
    }
}