using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreAnalysis.Migrations
{
    /// <inheritdoc />
    public partial class adjustsaledatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "ItemStorageId",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Remove invalid data
            migrationBuilder.Sql(
                "DELETE FROM Sales WHERE ItemStorageId NOT IN (SELECT Id FROM ItemsStorage);"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ItemStorageId",
                table: "Sales",
                column: "ItemStorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_ItemsStorage_ItemStorageId",
                table: "Sales",
                column: "ItemStorageId",
                principalTable: "ItemsStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_ItemsStorage_ItemStorageId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ItemStorageId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ItemStorageId",
                table: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

    }
}
