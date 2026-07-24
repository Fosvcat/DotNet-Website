using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geekspace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "ResourceComments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceComments_ParentCommentId",
                table: "ResourceComments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceComments_ResourceComments_ParentCommentId",
                table: "ResourceComments",
                column: "ParentCommentId",
                principalTable: "ResourceComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceComments_ResourceComments_ParentCommentId",
                table: "ResourceComments");

            migrationBuilder.DropIndex(
                name: "IX_ResourceComments_ParentCommentId",
                table: "ResourceComments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "ResourceComments");
        }
    }
}
