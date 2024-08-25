using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class Item: IEntity
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string? FolderId { get; set; }

        public bool IsStarred { get; set; }

        public Folder? Folder { get; set; }
    }
}