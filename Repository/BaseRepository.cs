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
                    foreach (var pair in paramsDict)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = $"@{pair.Key}";
                        parameter.Value = pair.Value;
                        command.Parameters.Add(parameter);
                    }
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

        public virtual T? Get(string userId, string id) =>
         _context.Set<T>().Where(e => e.Id == id).FirstOrDefault();


        public List<T> GetAll(string userId) => _context.Set<T>().Where(i => i.UserId == userId).ToList();

        public List<T> GetAll(string userId, List<string> ids) =>
         _context.Set<T>().Where(e => e.UserId == userId && ids.Contains(e.Id)).ToList();

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public void Update<TProperty>(T entity, Expression<Func<T, TProperty>> property, TProperty newValue)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).Property(property).CurrentValue = newValue;
            _context.Entry(entity).Property(property).IsModified = true;
        }
    }
}