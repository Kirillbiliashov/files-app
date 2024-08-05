using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class UserFile
    {
        public string Id { get; init; }

        public string Filename { get; init; }

         public string? Folder { get; init; }

        public long Length { get; init; }

        public long Modified { get; init; }

        [JsonIgnore]
        public byte[] Content { get; init; }

        public string? FolderId { get; init; }
    }
}