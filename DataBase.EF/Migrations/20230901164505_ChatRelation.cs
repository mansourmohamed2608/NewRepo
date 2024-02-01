using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChatRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "VedioPath",
                table: "Chat");

            migrationBuilder.CreateTable(
                name: "ChatPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatPhoto_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatVedio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VedioPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatVedio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatVedio_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatPhoto_ChatId",
                table: "ChatPhoto",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatVedio_ChatId",
                table: "ChatVedio",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatPhoto");

            migrationBuilder.DropTable(
                name: "ChatVedio");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VedioPath",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
