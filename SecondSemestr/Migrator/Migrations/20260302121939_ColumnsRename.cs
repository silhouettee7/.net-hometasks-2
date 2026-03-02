using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailMessages_Users_UserId",
                table: "EmailMessages");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_email");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "EmailMessages",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "EmailMessages",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "EmailMessages",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EmailMessages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EmailMessages",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "EmailMessages",
                newName: "sent_at");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "EmailMessages",
                newName: "error_message");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "EmailMessages",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_EmailMessages_UserId",
                table: "EmailMessages",
                newName: "IX_EmailMessages_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailMessages_Users_user_id",
                table: "EmailMessages",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailMessages_Users_user_id",
                table: "EmailMessages");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "EmailMessages",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "EmailMessages",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "EmailMessages",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "EmailMessages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "EmailMessages",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "sent_at",
                table: "EmailMessages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "error_message",
                table: "EmailMessages",
                newName: "ErrorMessage");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "EmailMessages",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_EmailMessages_user_id",
                table: "EmailMessages",
                newName: "IX_EmailMessages_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailMessages_Users_UserId",
                table: "EmailMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
