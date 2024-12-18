using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreAnalysis.Migrations
{
    /// <inheritdoc />
    public partial class addamounttostorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "ItemsStorage",
                newName: "LastUpdatedDate");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "ItemsStorage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ItemsStorage");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "ItemsStorage",
                newName: "AddedDate");
        }
    }
}
