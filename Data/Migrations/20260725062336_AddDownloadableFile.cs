using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geekspace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadableFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DownloadableFilePath",
                table: "LearningResources",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadableFilePath",
                table: "LearningResources");
        }
    }
}
