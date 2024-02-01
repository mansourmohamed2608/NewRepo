using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBase.EF.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friend_UserAccounts_FirstUserId",
                table: "Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Friend_UserAccounts_SecondUserId",
                table: "Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_UserAccounts_ReceiverId",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_UserAccounts_RequestorId",
                table: "FriendRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequest",
                table: "FriendRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friend",
                table: "Friend");

            migrationBuilder.RenameTable(
                name: "FriendRequest",
                newName: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Friend",
                newName: "Friends");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_RequestorId",
                table: "FriendRequests",
                newName: "IX_FriendRequests_RequestorId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_ReceiverId",
                table: "FriendRequests",
                newName: "IX_FriendRequests_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Friend_SecondUserId",
                table: "Friends",
                newName: "IX_Friends_SecondUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friend_FirstUserId",
                table: "Friends",
                newName: "IX_Friends_FirstUserId");

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "FriendRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friends",
                table: "Friends",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotifiedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificatinType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_UserAccounts_ActionedUserId",
                        column: x => x.ActionedUserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ActionedUserId",
                table: "Notification",
                column: "ActionedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_UserAccounts_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_UserAccounts_RequestorId",
                table: "FriendRequests",
                column: "RequestorId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_UserAccounts_FirstUserId",
                table: "Friends",
                column: "FirstUserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_UserAccounts_SecondUserId",
                table: "Friends",
                column: "SecondUserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_UserAccounts_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_UserAccounts_RequestorId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_UserAccounts_FirstUserId",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_UserAccounts_SecondUserId",
                table: "Friends");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friends",
                table: "Friends");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Friends",
                newName: "Friend");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                newName: "FriendRequest");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_SecondUserId",
                table: "Friend",
                newName: "IX_Friend_SecondUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_FirstUserId",
                table: "Friend",
                newName: "IX_Friend_FirstUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_RequestorId",
                table: "FriendRequest",
                newName: "IX_FriendRequest_RequestorId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequest",
                newName: "IX_FriendRequest_ReceiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friend",
                table: "Friend",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequest",
                table: "FriendRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_UserAccounts_FirstUserId",
                table: "Friend",
                column: "FirstUserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friend_UserAccounts_SecondUserId",
                table: "Friend",
                column: "SecondUserId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_UserAccounts_ReceiverId",
                table: "FriendRequest",
                column: "ReceiverId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_UserAccounts_RequestorId",
                table: "FriendRequest",
                column: "RequestorId",
                principalTable: "UserAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
