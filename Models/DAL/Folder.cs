using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class Folder
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string? FolderId { get; init; }

    }
}