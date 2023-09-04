using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPProject.Migrations
{
    /// <inheritdoc />
    public partial class Token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "asp",
                table: "Sections",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "asp",
                table: "Sections",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "asp",
                columns: table => new
                {
                    Id = table.Column<string>(type: "char(32)", fixedLength: true, maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId",
                schema: "asp",
                table: "Topics",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_AuthorId",
                schema: "asp",
                table: "Themes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AuthorId",
                schema: "asp",
                table: "Sections",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                schema: "asp",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ThemeId",
                schema: "asp",
                table: "Comments",
                column: "ThemeId");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "asp");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId",
                schema: "asp",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Themes_AuthorId",
                schema: "asp",
                table: "Themes");

            migrationBuilder.DropIndex(
                name: "IX_Sections_AuthorId",
                schema: "asp",
                table: "Sections");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId",
                schema: "asp",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ThemeId",
                schema: "asp",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "asp",
                table: "Sections",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
