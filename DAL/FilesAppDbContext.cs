using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FilesApp.DAL
{
    public class FilesAppDbContext : IdentityDbContext<AppUser>
    {

        public DbSet<Item> Items { get; set; }

        public DbSet<SharedLink> SharedLinks { get; set; }

        public FilesAppDbContext(DbContextOptions<FilesAppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserFile>();
            modelBuilder.Entity<Folder>();
        }

    }
}