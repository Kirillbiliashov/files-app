using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Repository
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly FilesAppDbContext _context;

        public BaseRepository(FilesAppDbContext context) => _context = context;

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public TReturn? ExecuteRawQuery<TReturn>(string queryStr, Dictionary<string, object> paramsDict, Func<DbDataReader, TReturn> readerCallback)
        {
            var conn = _context.Database.GetDbConnection();
            DbDataReader reader = null;
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = queryStr;
                    var folderIdParam = command.CreateParameter();
                    foreach (var pair in paramsDict)
                    {
                        folderIdParam.ParameterName = $"@{pair.Key}";
                        folderIdParam.Value = pair.Value;
                    }
                    command.Parameters.Add(folderIdParam);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        return readerCallback(reader);
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.DisposeAsync();
                }
                conn.Close();
            }

            return default;
        }

        public virtual T? Get(string id) =>
         _context.Set<T>().Where(e => e.Id == id).FirstOrDefault();


        public List<T> GetAll() => _context.Set<T>().ToList();

        public List<T> GetAll(List<string> ids) =>
         _context.Set<T>().Where(e => ids.Contains(e.Id)).ToList();

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public void Update<TProperty>(T entity, Expression<Func<T, TProperty>> property, TProperty newValue)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).Property(property).CurrentValue = newValue;
            _context.Entry(entity).Property(property).IsModified = true;
        }
    }
}