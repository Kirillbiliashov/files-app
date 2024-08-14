using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class UserFile: Item
    {
        public long Size { get; init; }

        public long LastModified { get; init; }

        [JsonIgnore]
        public byte[] Content { get; init; }
    }
}