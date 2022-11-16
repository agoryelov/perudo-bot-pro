﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PerudoBot.Database.Data;

#nullable disable

namespace PerudoBot.Database.Migrations
{
    [DbContext(typeof(PerudoBotDbContext))]
    [Migration("20221116043850_AddItemDrops")]
    partial class AddItemDrops
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("PerudoBot.Database.Data.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Score")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Action", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentActionId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PlayerHandId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentActionId");

                    b.HasIndex("PlayerHandId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoundId");

                    b.ToTable("Action");

                    b.HasDiscriminator<string>("ActionType").HasValue("Action");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Auction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuctionItemId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("Day")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FinalPrice")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinningPlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuctionItemId");

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActionType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("AuctionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuctionPlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentActionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuctionId");

                    b.HasIndex("AuctionPlayerId");

                    b.HasIndex("ParentActionId");

                    b.ToTable("AuctionActions");

                    b.HasDiscriminator<string>("ActionType").HasValue("AuctionAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuctionId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GamePlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuctionId");

                    b.HasIndex("UserId");

                    b.ToTable("AuctionPlayers");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("DefaultRoundType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SeasonId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinningPlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SeasonId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DropEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Price")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tier")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Items");

                    b.HasDiscriminator<string>("ItemType").HasValue("Item");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.ItemDrop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("ItemId");

                    b.HasIndex("PlayerId");

                    b.ToTable("ItemDrops");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Lives")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundEliminated")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TurnOrder")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.PlayerHand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Dice")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoundId");

                    b.ToTable("PlayerHands");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Rattle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<int>("RattleContentType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RattleType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rattles");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActivePlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartingPlayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId", "RoundNumber")
                        .IsUnique();

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.RoundNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.HasIndex("UserId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Season", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WinnerId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AchievementScore")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Elo")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EquippedDiceId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBot")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DiscordId")
                        .IsUnique();

                    b.HasIndex("EquippedDiceId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserAchievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AchievementId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsNew")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AchievementId");

                    b.HasIndex("IsNew");

                    b.HasIndex("UserId", "AchievementId")
                        .IsUnique();

                    b.ToTable("UserAchievements");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("UserId");

                    b.ToTable("UserItem");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AuctionId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserLogType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuctionId");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLog");

                    b.HasDiscriminator<string>("UserLogType").HasValue("UserLog");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionBid", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.AuctionAction");

                    b.Property<int>("BidAmount")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("AuctionBid");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionPass", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.AuctionAction");

                    b.HasDiscriminator().HasValue("AuctionPass");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.BetAction", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.Action");

                    b.Property<int>("BetAmount")
                        .HasColumnType("INTEGER");

                    b.Property<double>("BetOdds")
                        .HasColumnType("REAL");

                    b.Property<int>("BetType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TargetBidId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("TargetBidId");

                    b.HasDiscriminator().HasValue("BetAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.BidAction", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.Action");

                    b.Property<int>("Pips")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("BidAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.DiceItem", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.Item");

                    b.HasDiscriminator().HasValue("DiceItem");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.EloLog", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.UserLog");

                    b.Property<int>("EloChange")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("EloLog");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.LiarAction", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.Action");

                    b.Property<int>("ActualQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LivesLost")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LosingPlayerId")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("LiarAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.PointsLog", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.UserLog");

                    b.Property<int>("PointsChange")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PointsLogTypeId")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("PointsLog");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.ReverseAction", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.Action");

                    b.HasDiscriminator().HasValue("ReverseAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Action", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Action", "ParentAction")
                        .WithMany()
                        .HasForeignKey("ParentActionId");

                    b.HasOne("PerudoBot.Database.Data.PlayerHand", "PlayerHand")
                        .WithMany()
                        .HasForeignKey("PlayerHandId");

                    b.HasOne("PerudoBot.Database.Data.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.Round", "Round")
                        .WithMany("Actions")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentAction");

                    b.Navigation("Player");

                    b.Navigation("PlayerHand");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Auction", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Item", "AuctionItem")
                        .WithMany()
                        .HasForeignKey("AuctionItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AuctionItem");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionAction", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Auction", "Auction")
                        .WithMany("AuctionActions")
                        .HasForeignKey("AuctionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.AuctionPlayer", "AuctionPlayer")
                        .WithMany()
                        .HasForeignKey("AuctionPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.AuctionAction", "ParentAction")
                        .WithMany()
                        .HasForeignKey("ParentActionId");

                    b.Navigation("Auction");

                    b.Navigation("AuctionPlayer");

                    b.Navigation("ParentAction");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.AuctionPlayer", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Auction", "Auction")
                        .WithMany("AuctionPlayers")
                        .HasForeignKey("AuctionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auction");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Game", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Season", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.ItemDrop", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Item");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Player", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.PlayerHand", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Player", "Player")
                        .WithMany("PlayerHands")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.Round", "Round")
                        .WithMany("PlayerHands")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Round");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Rattle", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany("Rattles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Round", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany("Rounds")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.RoundNote", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Season", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.User", "Winner")
                        .WithMany()
                        .HasForeignKey("WinnerId");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.User", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.DiceItem", "EquippedDice")
                        .WithMany()
                        .HasForeignKey("EquippedDiceId");

                    b.Navigation("EquippedDice");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserAchievement", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Achievement", "Achievement")
                        .WithMany("UserAchievements")
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany("UserAchievements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Achievement");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserItem", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Item", "Item")
                        .WithMany("UserItems")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany("UserItems")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.UserLog", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Auction", "Auction")
                        .WithMany()
                        .HasForeignKey("AuctionId");

                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany("UserLogs")
                        .HasForeignKey("GameId");

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Auction");

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.BetAction", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.BidAction", "TargetBid")
                        .WithMany()
                        .HasForeignKey("TargetBidId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TargetBid");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Achievement", b =>
                {
                    b.Navigation("UserAchievements");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Auction", b =>
                {
                    b.Navigation("AuctionActions");

                    b.Navigation("AuctionPlayers");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Game", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Rounds");

                    b.Navigation("UserLogs");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Item", b =>
                {
                    b.Navigation("UserItems");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Player", b =>
                {
                    b.Navigation("PlayerHands");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Round", b =>
                {
                    b.Navigation("Actions");

                    b.Navigation("PlayerHands");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.User", b =>
                {
                    b.Navigation("Rattles");

                    b.Navigation("UserAchievements");

                    b.Navigation("UserItems");

                    b.Navigation("UserLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
