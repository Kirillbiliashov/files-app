using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class UserFile: Item
    {
        public long Size { get; set; }

        public long LastModified { get; set; }

        [JsonIgnore]
        public byte[] Content { get; set; }
    }
}