using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace indie_hub_server.Migrations
{
    /// <inheritdoc />
    public partial class PlaytestGameFileUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameFileUrl",
                table: "Playtests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameFileUrl",
                table: "Playtests");
        }
    }
}
