using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNameIdx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NameIdx",
                table: "Items",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameIdx",
                table: "Items");
        }
    }
}
