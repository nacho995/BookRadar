using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookRadar.App.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenLibraryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenLibraryKey",
                table: "Books",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenLibraryKey",
                table: "Books");
        }
    }
}
