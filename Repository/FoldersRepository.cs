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
        WHERE UserId = @UserId AND Id = @FolderId
    
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
        WHERE  UserId = @UserId AND Id = @FolderId
    
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

        public bool ExistsByName(string userId, string folderId, string name) =>
         _context.Items
         .OfType<Folder>()
         .Any(i => i.UserId == userId && i.FolderId == folderId && i.Name == name);

        override public Folder? Get(string userId, string id) =>
        _context.Items
        .AsNoTracking()
        .Where(i => i.UserId == userId && i.Id == id)
        .OfType<Folder>()
        .Include(f => f.Items)
        .FirstOrDefault();

        public Folder? GetByName(string userId, string name, string? parentId) =>
        _context.Items
        .AsNoTracking()
        .Where(i => i.UserId == userId && i.FolderId == parentId && i.Name == name)
        .OfType<Folder>()
        .OrderByDescending(i => i.NameIdx)
        .FirstOrDefault();

        public string? GetFolderIdByName(string userId, string parentId, string name, bool isFolderAdded)
        {
            var folders = isFolderAdded ?
            _context.ChangeTracker.Entries<Folder>().Select(e => e.Entity) :
            _context.Items.OfType<Folder>();

            return folders
            .Where(f => f.UserId == userId && f.Name == name)
            .Select(f => f.Id)
            .FirstOrDefault();
        }

        public string? GetFolderName(string userId, string folderId) => _context.Items
        .OfType<Folder>()
        .Where(f => f.UserId == userId && f.Id == folderId)
        .Select(f => f.Name)
        .FirstOrDefault();

        public long? GetLastModified(string userId, string id)
        {
            var paramsDict = new Dictionary<string, object>
            {
                {"UserId", userId},
                {"FolderId", id},
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

        public long GetSize(string userId, string id)
        {
            var paramsDict = new Dictionary<string, object>
            {
                {"UserId", userId},
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

        public List<Folder> GetSubfolders(string userId, string folderId) => 
        _context.Items
        .AsNoTracking()
        .OfType<Folder>()
        .Where(f => f.UserId == userId && f.FolderId == folderId)
        .ToList();

        public bool IsTrackedByName(string userId, string name, string folderId) =>
        _context.ChangeTracker.Entries<Folder>()
                .Where(f => f.Entity.UserId == userId && f.Entity.FolderId == folderId && f.Entity.Name == name)
                .Any();

        public override List<Folder> GetAll(string userId) => _context.Items
        .OfType<Folder>()
        .Where(i => i.UserId == userId && i.FolderId == null)
        .AsNoTracking()
        .ToList();

    }
}