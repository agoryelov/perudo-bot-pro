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
    [Migration("20220605212228_InitialMigration")]
    partial class InitialMigration
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

            modelBuilder.Entity("PerudoBot.Database.Data.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ActivePlayerId")
                        .HasColumnType("INTEGER");

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

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Elo")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

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

            modelBuilder.Entity("PerudoBot.Database.Data.UserLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
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

                    b.ToTable("UserLog");

                    b.HasDiscriminator<string>("UserLogType").HasValue("UserLog");
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

            modelBuilder.Entity("PerudoBot.Database.Data.EloLog", b =>
                {
                    b.HasBaseType("PerudoBot.Database.Data.UserLog");

                    b.Property<int>("EloChange")
                        .HasColumnType("INTEGER");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

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

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.HasDiscriminator().HasValue("PointsLog");
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

            modelBuilder.Entity("PerudoBot.Database.Data.Game", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Season", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Season");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Player", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany("Players")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("PerudoBot.Database.Data.UserAchievement", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Achievement");

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

            modelBuilder.Entity("PerudoBot.Database.Data.EloLog", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.PointsLog", b =>
                {
                    b.HasOne("PerudoBot.Database.Data.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId");

                    b.HasOne("PerudoBot.Database.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PerudoBot.Database.Data.Game", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Rounds");
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
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
