using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTracker.Migrations
{
    /// <inheritdoc />
    public partial class depId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectedLocationId",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedLocationId",
                table: "Departments");
        }
    }
}
