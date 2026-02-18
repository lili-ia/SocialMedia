using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateprofilepicuserrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfilePics_UserId",
                table: "ProfilePics");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentProfilePicId",
                table: "Users",
                column: "CurrentProfilePicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePics_UserId",
                table: "ProfilePics",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ProfilePics_CurrentProfilePicId",
                table: "Users",
                column: "CurrentProfilePicId",
                principalTable: "ProfilePics",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ProfilePics_CurrentProfilePicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurrentProfilePicId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ProfilePics_UserId",
                table: "ProfilePics");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePics_UserId",
                table: "ProfilePics",
                column: "UserId",
                unique: true);
        }
    }
}
