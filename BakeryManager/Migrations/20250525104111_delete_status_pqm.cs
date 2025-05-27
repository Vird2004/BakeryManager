using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BakeryManager.Migrations
{
    /// <inheritdoc />
    public partial class delete_status_pqm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductQuantities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProductQuantities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
