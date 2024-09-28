using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class Folder : Item
    {

        public int NameIdx { get; set; } = 0;

        public ICollection<Item> Items { get; set; }

    }
}