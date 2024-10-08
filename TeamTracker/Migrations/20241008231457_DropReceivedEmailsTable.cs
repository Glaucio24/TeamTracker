using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamTracker.Migrations
{
    /// <inheritdoc />
    public partial class DropReceivedEmailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the ReceivedEmails table
            migrationBuilder.DropTable(
                name: "ReceivedEmails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate the ReceivedEmails table in case of rollback
            migrationBuilder.CreateTable(
                name: "ReceivedEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderEmail = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivedEmails", x => x.Id);
                });
        }
    }
}
