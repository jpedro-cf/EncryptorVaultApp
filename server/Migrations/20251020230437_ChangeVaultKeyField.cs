using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EncryptionApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVaultKeyField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VaultKeySalt",
                table: "AspNetUsers",
                newName: "VaultKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VaultKey",
                table: "AspNetUsers",
                newName: "VaultKeySalt");
        }
    }
}
