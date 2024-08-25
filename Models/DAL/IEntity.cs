using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public interface IEntity
    {
        string Id { get; init; }
    }
}