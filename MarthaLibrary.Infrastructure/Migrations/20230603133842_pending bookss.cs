using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarthaLibrary.Infrastructure.Migrations
{
    public partial class pendingbookss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingBooks_Books_BookId",
                table: "PendingBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingBooks",
                table: "PendingBooks");

            migrationBuilder.RenameTable(
                name: "PendingBooks",
                newName: "PendingBookss");

            migrationBuilder.RenameIndex(
                name: "IX_PendingBooks_BookId",
                table: "PendingBookss",
                newName: "IX_PendingBookss_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingBookss",
                table: "PendingBookss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingBookss_Books_BookId",
                table: "PendingBookss",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingBookss_Books_BookId",
                table: "PendingBookss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingBookss",
                table: "PendingBookss");

            migrationBuilder.RenameTable(
                name: "PendingBookss",
                newName: "PendingBooks");

            migrationBuilder.RenameIndex(
                name: "IX_PendingBookss_BookId",
                table: "PendingBooks",
                newName: "IX_PendingBooks_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingBooks",
                table: "PendingBooks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingBooks_Books_BookId",
                table: "PendingBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
