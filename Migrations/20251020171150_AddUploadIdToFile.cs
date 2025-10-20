using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadIdToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadId",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "Files");
        }
    }
}
