using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class FolderAndFilesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RootKeySalt",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "RootKeySalt",
                table: "Files");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentFolderId",
                table: "Files",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RootKeySalt",
                table: "Folders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentFolderId",
                table: "Files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootKeySalt",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
