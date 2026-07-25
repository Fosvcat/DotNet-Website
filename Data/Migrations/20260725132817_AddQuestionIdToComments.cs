using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geekspace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionIdToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "ResourceComments",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "ResourceComments");
        }
    }
}
