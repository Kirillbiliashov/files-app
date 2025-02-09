﻿// <auto-generated />
using FilesApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FilesApp.Migrations
{
    [DbContext(typeof(FilesAppDbContext))]
    [Migration("20240817142810_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FilesApp.Models.DAL.Item", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FolderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsStarred")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Items");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Item");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("FilesApp.Models.DAL.Folder", b =>
                {
                    b.HasBaseType("FilesApp.Models.DAL.Item");

                    b.HasDiscriminator().HasValue("Folder");
                });

            modelBuilder.Entity("FilesApp.Models.DAL.UserFile", b =>
                {
                    b.HasBaseType("FilesApp.Models.DAL.Item");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<long>("LastModified")
                        .HasColumnType("bigint");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasDiscriminator().HasValue("UserFile");
                });

            modelBuilder.Entity("FilesApp.Models.DAL.Item", b =>
                {
                    b.HasOne("FilesApp.Models.DAL.Folder", "Folder")
                        .WithMany()
                        .HasForeignKey("FolderId");

                    b.Navigation("Folder");
                });
#pragma warning restore 612, 618
        }
    }
}
