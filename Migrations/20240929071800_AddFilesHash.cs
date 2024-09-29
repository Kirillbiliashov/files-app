using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilesApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

                    migrationBuilder.Sql(@"
                    UPDATE Items
                    SET Hash = LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', Content), 2))
                    WHERE Discriminator = 'UserFile'
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Items");
        }
    }
}
