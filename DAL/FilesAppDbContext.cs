using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilesApp.Models.DAL;
using Microsoft.EntityFrameworkCore;

namespace FilesApp.DAL
{
    public class FilesAppDbContext : DbContext
    {

        public FilesAppDbContext(DbContextOptions<FilesAppDbContext> options): base(options) {}

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFile>();
            modelBuilder.Entity<Folder>();
        }

    }
}