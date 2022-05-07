using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBook.Migrations
{
    public partial class FixDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Catogogy",
                table: "Book",
                newName: "Categogy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Categogy",
                table: "Book",
                newName: "Catogogy");
        }
    }
}
