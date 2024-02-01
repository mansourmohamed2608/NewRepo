using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reacts",
                table: "QuestionReacts",
                newName: "react");

            migrationBuilder.RenameColumn(
                name: "reacts",
                table: "QuestionCommentReacts",
                newName: "react");

            migrationBuilder.RenameColumn(
                name: "reacts",
                table: "PostReacts",
                newName: "react");

            migrationBuilder.RenameColumn(
                name: "reacts",
                table: "PostCommentReacts",
                newName: "react");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "react",
                table: "QuestionReacts",
                newName: "reacts");

            migrationBuilder.RenameColumn(
                name: "react",
                table: "QuestionCommentReacts",
                newName: "reacts");

            migrationBuilder.RenameColumn(
                name: "react",
                table: "PostReacts",
                newName: "reacts");

            migrationBuilder.RenameColumn(
                name: "react",
                table: "PostCommentReacts",
                newName: "reacts");
        }
    }
}
