using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EncryptionApp.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIdxAndEnumConverter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "SharedItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedKey",
                table: "SharedItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StorageUsage_ContentType_UserId",
                table: "StorageUsage",
                columns: new[] { "ContentType", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StorageUsage_ContentType_UserId",
                table: "StorageUsage");

            migrationBuilder.DropColumn(
                name: "EncryptedKey",
                table: "SharedItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemType",
                table: "SharedItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
