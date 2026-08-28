using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStock.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleAttributeValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttributeValues",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeValues",
                table: "Articles");
        }
    }
}
