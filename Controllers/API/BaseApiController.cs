using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.Controllers.API
{
    public class BaseApiController : ControllerBase
    {
        protected readonly FilesAppDbContext _context;

        private const string _folderSizeQuery = @"
        WITH RecursiveFolders AS (
        SELECT Id, FolderId, Size
        FROM Items
        WHERE Id = @FolderId
    
        UNION ALL
    
        SELECT i.Id, i.FolderId, i.Size
        FROM Items i
        INNER JOIN RecursiveFolders rf ON i.FolderId = rf.Id
        )

        SELECT SUM(Size) AS TotalSize FROM RecursiveFolders;
        ";

        private const string _folderModifiedQuery = @"
        WITH RecursiveFolders AS (
        SELECT Id, FolderId, LastModified
        FROM Items
        WHERE Id = @FolderId
    
        UNION ALL
    
        SELECT i.Id, i.FolderId, i.LastModified
        FROM Items i
        INNER JOIN RecursiveFolders rf ON i.FolderId = rf.Id
        )

        SELECT MAX(LastModified) AS LastModified FROM RecursiveFolders;
        ";

        public BaseApiController(FilesAppDbContext context) => _context = context;

        protected long GetFolderSize(string folderId)
        {
            var conn = _context.Database.GetDbConnection();
            DbDataReader reader = null;
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = _folderSizeQuery;
                    var folderIdParam = command.CreateParameter();
                    folderIdParam.ParameterName = "@FolderId";
                    folderIdParam.Value = folderId;
                    command.Parameters.Add(folderIdParam);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt64(0);
                        }
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

            return 0;
        }

        protected long? GetFolderLastModified(string folderId)
        {
            var conn = _context.Database.GetDbConnection();
            DbDataReader reader = null;
            try
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = _folderModifiedQuery;
                    var folderIdParam = command.CreateParameter();
                    folderIdParam.ParameterName = "@FolderId";
                    folderIdParam.Value = folderId;
                    command.Parameters.Add(folderIdParam);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt64(0);
                        }
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

            return null;

        }
    }
}