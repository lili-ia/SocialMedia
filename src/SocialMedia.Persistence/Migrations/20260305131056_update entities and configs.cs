using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateentitiesandconfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_IsRead",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastActivityAt",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "ProfilePics");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PostViews");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PostFiles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PendingEmails");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Follows");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Blocks");

            migrationBuilder.RenameColumn(
                name: "OriginalStorageKey",
                table: "ProfilePics",
                newName: "StorageKey");

            migrationBuilder.RenameColumn(
                name: "OriginalFileSize",
                table: "ProfilePics",
                newName: "FileSizeBytes");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "ProfilePics",
                newName: "FileName");

            migrationBuilder.RenameIndex(
                name: "IX_ProfilePics_OriginalStorageKey",
                table: "ProfilePics",
                newName: "IX_ProfilePics_StorageKey");

            migrationBuilder.RenameColumn(
                name: "OriginalStorageKey",
                table: "PostFiles",
                newName: "StorageKey");

            migrationBuilder.RenameColumn(
                name: "OriginalFileSize",
                table: "PostFiles",
                newName: "FileSizeBytes");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "PostFiles",
                newName: "FileName");

            migrationBuilder.RenameIndex(
                name: "IX_PostFiles_OriginalStorageKey",
                table: "PostFiles",
                newName: "IX_PostFiles_StorageKey");

            migrationBuilder.RenameColumn(
                name: "OriginalStorageKey",
                table: "MessageAttachments",
                newName: "StorageKey");

            migrationBuilder.RenameColumn(
                name: "OriginalFileSize",
                table: "MessageAttachments",
                newName: "FileSizeBytes");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "MessageAttachments",
                newName: "FileName");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachments_OriginalStorageKey",
                table: "MessageAttachments",
                newName: "IX_MessageAttachments_StorageKey");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "PasswordResetTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ActorId",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "Notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "EmailConfirmationTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Chats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LastMessageId1",
                table: "Chats",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Chats",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ChatParticipants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadAt",
                table: "ChatParticipants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_CreatorId",
                table: "Chats",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId1",
                table: "Chats",
                column: "LastMessageId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId1",
                table: "Chats",
                column: "LastMessageId1",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_CreatorId",
                table: "Chats",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId1",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_CreatorId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_CreatorId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastMessageId1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "LastReadAt",
                table: "ChatParticipants");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "ProfilePics",
                newName: "OriginalStorageKey");

            migrationBuilder.RenameColumn(
                name: "FileSizeBytes",
                table: "ProfilePics",
                newName: "OriginalFileSize");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "ProfilePics",
                newName: "OriginalFileName");

            migrationBuilder.RenameIndex(
                name: "IX_ProfilePics_StorageKey",
                table: "ProfilePics",
                newName: "IX_ProfilePics_OriginalStorageKey");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "PostFiles",
                newName: "OriginalStorageKey");

            migrationBuilder.RenameColumn(
                name: "FileSizeBytes",
                table: "PostFiles",
                newName: "OriginalFileSize");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "PostFiles",
                newName: "OriginalFileName");

            migrationBuilder.RenameIndex(
                name: "IX_PostFiles_StorageKey",
                table: "PostFiles",
                newName: "IX_PostFiles_OriginalStorageKey");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "MessageAttachments",
                newName: "OriginalStorageKey");

            migrationBuilder.RenameColumn(
                name: "FileSizeBytes",
                table: "MessageAttachments",
                newName: "OriginalFileSize");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "MessageAttachments",
                newName: "OriginalFileName");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachments_StorageKey",
                table: "MessageAttachments",
                newName: "IX_MessageAttachments_OriginalStorageKey");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Users",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "RefreshTokens",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "ProfilePics",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PostViews",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Posts",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PostLikes",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PostFiles",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "PendingEmails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PasswordResetTokens",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Notifications",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Messages",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "MessageAttachments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Follows",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "EmailConfirmationTokens",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Comments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Chats",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Chats",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "ChatParticipants",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Blocks",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_IsRead",
                table: "Messages",
                columns: new[] { "ChatId", "IsRead" },
                filter: "\"IsRead\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastActivityAt",
                table: "Chats",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
