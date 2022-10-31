using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerudoBot.Database.Migrations
{
    public partial class AddAuctionDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Auctions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Auctions");
        }
    }
}
