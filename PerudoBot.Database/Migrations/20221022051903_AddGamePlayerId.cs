using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerudoBot.Database.Migrations
{
    public partial class AddGamePlayerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GamePlayerId",
                table: "AuctionPlayers",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GamePlayerId",
                table: "AuctionPlayers");
        }
    }
}
