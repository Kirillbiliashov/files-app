using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FilesApp.Models.DAL;

namespace FilesApp.Repository
{
    public interface IRepository<T> where T: IEntity
    {
        void Delete(T entity);

        void Add(T entity);

         List<T> GetAll();

        List<T> GetAll(List<string> ids);

        T? Get(string id);

        void Update<TProperty>(T entity, Expression<Func<T, TProperty>> property, TProperty newValue);

        Task<int> SaveAsync();

        TReturn? ExecuteRawQuery<TReturn>(string queryStr, Dictionary<string, object> paramsDict, Func<DbDataReader, TReturn> readerCallback);
    }
}