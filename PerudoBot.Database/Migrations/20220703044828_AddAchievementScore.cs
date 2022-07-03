using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerudoBot.Database.Migrations
{
    public partial class AddAchievementScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Achievements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DiscordId",
                table: "Users",
                column: "DiscordId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DiscordId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Achievements");
        }
    }
}
