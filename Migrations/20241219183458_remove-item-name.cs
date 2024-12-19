using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreAnalysis.Migrations
{
    /// <inheritdoc />
    public partial class removeitemname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "ItemsStorage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "ItemsStorage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
