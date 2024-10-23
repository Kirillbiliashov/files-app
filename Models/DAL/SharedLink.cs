using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesApp.Models.DAL
{
    public class SharedLink : IEntity
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
        public string UserId { get; init; }

        public string ItemId { get; init; }

        public UserFile Item { get; init; }
    }
}