using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EncryptionApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFileStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "Failed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Files");
        }
    }
}
