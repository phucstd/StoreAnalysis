using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreAnalysis.Migrations
{
    /// <inheritdoc />
    public partial class removeforeighkeyinsale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Items_ItemID",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ItemID",
                table: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sales_ItemID",
                table: "Sales",
                column: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Items_ItemID",
                table: "Sales",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
