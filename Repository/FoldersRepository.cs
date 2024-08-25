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
    public class FoldersRepository : BaseRepository<Folder>, IFoldersRepository
    {
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

        SELECT COALESCE(SUM(Size), 0) AS TotalSize FROM RecursiveFolders;
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

        SELECT COALESCE(MAX(LastModified), 0) AS LastModified FROM RecursiveFolders;
        ";

        public FoldersRepository(FilesAppDbContext context) : base(context)
        {
        }

        public bool ExistsByName(string name) => _context.Items.OfType<Folder>().Any(i => i.Name == name);

        override public Folder? Get(string id) =>
        _context.Items.Where(i => i.Id == id).OfType<Folder>().Include(f => f.Items).FirstOrDefault();

        public int GetCount(string name) => _context.Items.OfType<Folder>().Where(f => f.Name == name).Count();

        public string? GetFolderIdByName(string name, bool isFolderAdded)
        {
            var folders = isFolderAdded ?
            _context.ChangeTracker.Entries<Folder>().Select(e => e.Entity) :
            _context.Items.OfType<Folder>();

            return folders
            .Where(f => f.Name == name)
            .Select(f => f.Id)
            .FirstOrDefault();
        }

        public string? GetFolderName(string folderId) => _context.Items
        .OfType<Folder>()
        .Where(f => f.Id == folderId)
        .Select(f => f.Name)
        .FirstOrDefault();

        public long? GetLastModified(string id)
        {
            var paramsDict = new Dictionary<string, object>
            {
                {"FolderId", id}
            };

            return ExecuteRawQuery<long?>(_folderModifiedQuery, paramsDict, r =>
            {
                while (r.Read())
                {
                    return r.GetInt64(0);
                }
                return null;
            });
        }

        public long GetSize(string id)
        {
            var paramsDict = new Dictionary<string, object>
            {
                {"FolderId", id}
            };

            return ExecuteRawQuery(_folderSizeQuery, paramsDict, r =>
            {
                while (r.Read())
                {
                    return r.GetInt64(0);
                }
                return 0;
            });
        }

        public bool IsTrackedByName(string name) =>
        _context.ChangeTracker.Entries<Folder>()
                .Where(f => f.Entity.Name == name)
                .Any();

    }
}