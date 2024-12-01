using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreAnalysis.Migrations
{
    /// <inheritdoc />
    public partial class removequantitysale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TotalPrice",
                table: "Sales",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
