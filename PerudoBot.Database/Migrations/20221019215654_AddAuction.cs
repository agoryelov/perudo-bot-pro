using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerudoBot.Database.Migrations
{
    public partial class AddAuction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Emotes",
                table: "Items",
                newName: "Content");

            migrationBuilder.AddColumn<int>(
                name: "AuctionId",
                table: "UserLog",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DropEnabled",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    AuctionItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinningPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    FinalPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Items_AuctionItemId",
                        column: x => x.AuctionItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuctionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionPlayers_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionPlayers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActionType = table.Column<string>(type: "TEXT", nullable: false),
                    AuctionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AuctionPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentActionId = table.Column<int>(type: "INTEGER", nullable: true),
                    BidAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionActions_AuctionActions_ParentActionId",
                        column: x => x.ParentActionId,
                        principalTable: "AuctionActions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuctionActions_AuctionPlayers_AuctionPlayerId",
                        column: x => x.AuctionPlayerId,
                        principalTable: "AuctionPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionActions_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_AuctionId",
                table: "UserLog",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionActions_AuctionId",
                table: "AuctionActions",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionActions_AuctionPlayerId",
                table: "AuctionActions",
                column: "AuctionPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionActions_ParentActionId",
                table: "AuctionActions",
                column: "ParentActionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionPlayers_AuctionId",
                table: "AuctionPlayers",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionPlayers_UserId",
                table: "AuctionPlayers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_AuctionItemId",
                table: "Auctions",
                column: "AuctionItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLog_Auctions_AuctionId",
                table: "UserLog",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLog_Auctions_AuctionId",
                table: "UserLog");

            migrationBuilder.DropTable(
                name: "AuctionActions");

            migrationBuilder.DropTable(
                name: "AuctionPlayers");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_UserLog_AuctionId",
                table: "UserLog");

            migrationBuilder.DropColumn(
                name: "AuctionId",
                table: "UserLog");

            migrationBuilder.DropColumn(
                name: "DropEnabled",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Items",
                newName: "Emotes");
        }
    }
}
