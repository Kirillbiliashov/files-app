using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.Http
{
    public class DeleteFilesBody
    {
        public List<string> files { get; init; }
    }
}