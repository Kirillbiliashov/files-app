using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.Http
{
    public class CreateFolderBody
    {
        public string Name { get; set; }
        public string? FolderId { get; init; }
    }
}