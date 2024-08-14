using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public abstract class Item
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string? FolderId { get; init; }

        public bool IsStarred { get; set; }
    }
}