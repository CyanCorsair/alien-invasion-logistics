using Microsoft.EntityFrameworkCore;
using System;
using Core.Database.Models;

namespace Core.Database.Contexts
{
    public class GameDataContext : DbContext
    {
        public DbSet<GameDataModel> GameData { get; set; }
        public DbSet<GameSaveModel> GameSaves { get; set; }
        public DbSet<GameStateModel> GameState { get; set; }
        public DbSet<GameSettingsModel> GameSettings { get; set; }
        public DbSet<PlayerStateModel> PlayerState { get; set; }
        public DbSet<AIPlayerStateModel> AIPLayerStates { get; set; }
        public DbSet<SolarSystemState> SolarSystemState { get; set; }

        public GameDataContext(DbContextOptions<GameDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure navigation properties for GameDataModel
            modelBuilder.Entity<GameDataModel>()
                .HasOne(g => g.GameSettings)
                .WithMany()
                .HasForeignKey(g => g.GameSettingsForeignKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameDataModel>()
                .HasOne(g => g.GameState)
                .WithMany()
                .HasForeignKey(g => g.GameStateForeignKey)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure navigation properties for GameSaveModel
            modelBuilder.Entity<GameSaveModel>()
                .HasOne(s => s.GameData)
                .WithMany()
                .HasForeignKey(s => s.GameDataForeignKey)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure navigation properties for GameStateModel
            modelBuilder.Entity<GameStateModel>()
                .HasOne(g => g.PlayerState)
                .WithMany()
                .HasForeignKey(g => g.PlayerStateForeignKey)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameStateModel>()
                .HasMany(g => g.AIPlayerStates)
                .WithMany()
                .UsingEntity(j => j.ToTable("GameStateAIPlayers"));

            modelBuilder.Entity<GameStateModel>()
                .HasOne(g => g.SolarSystemState)
                .WithMany()
                .HasForeignKey(g => g.SolarSystemStateForeignKey)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure owned types for PlayerStateModel
            modelBuilder.Entity<PlayerStateModel>()
                .OwnsOne(p => p.ResourcesState, rs =>
                {
                    rs.Property(r => r.Id).IsRequired();
                    rs.Property(r => r.EnergyStored).HasColumnName("EnergyStored");
                    rs.Property(r => r.EnergyIncomeDaily).HasColumnName("EnergyIncomeDaily");
                    rs.Property(r => r.MineralsStored).HasColumnName("MineralsStored");
                    rs.Property(r => r.MineralsIncomeDaily).HasColumnName("MineralsIncomeDaily");
                })
                .Navigation(p => p.ResourcesState)
                .IsRequired();

            modelBuilder.Entity<PlayerStateModel>()
                .OwnsOne(p => p.ResearchState, rs =>
                {
                    rs.Property(r => r.Id).IsRequired();
                    rs.OwnsOne(r => r.CurrentResearch);
                    rs.OwnsMany(r => r.ResearchQueue);
                    rs.OwnsMany(r => r.CompletedResearch);
                })
                .Navigation(p => p.ResearchState)
                .IsRequired();

            // Configure owned types for AIPlayerStateModel
            modelBuilder.Entity<AIPlayerStateModel>()
                .OwnsOne(p => p.ResourcesState, rs =>
                {
                    rs.Property(r => r.Id).IsRequired();
                    rs.Property(r => r.EnergyStored).HasColumnName("EnergyStored");
                    rs.Property(r => r.EnergyIncomeDaily).HasColumnName("EnergyIncomeDaily");
                    rs.Property(r => r.MineralsStored).HasColumnName("MineralsStored");
                    rs.Property(r => r.MineralsIncomeDaily).HasColumnName("MineralsIncomeDaily");
                })
                .Navigation(p => p.ResourcesState)
                .IsRequired();

            modelBuilder.Entity<AIPlayerStateModel>()
                .OwnsOne(p => p.ResearchState, rs =>
                {
                    rs.Property(r => r.Id).IsRequired();
                    rs.OwnsOne(r => r.CurrentResearch);
                    rs.OwnsMany(r => r.ResearchQueue);
                    rs.OwnsMany(r => r.CompletedResearch);
                })
                .Navigation(p => p.ResearchState)
                .IsRequired();

            // Configure navigation properties for SolarSystemState
            modelBuilder.Entity<SolarSystemState>()
                .HasOne(s => s.SystemSun)
                .WithMany()
                .HasForeignKey(s => s.SystemSunId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SolarSystemState>()
                .HasMany(s => s.PlanetarySystems)
                .WithOne(p => p.SolarSystem)
                .HasForeignKey(p => p.SolarSystemStateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SolarSystemState>()
                .HasMany(s => s.AsteroidBelts)
                .WithOne(a => a.SolarSystem)
                .HasForeignKey(a => a.SolarSystemStateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure navigation properties for PlanetarySystemState
            modelBuilder.Entity<PlanetarySystemState>()
                .HasMany(ps => ps.Planets)
                .WithOne(p => p.PlanetarySystem)
                .HasForeignKey(p => p.PlanetarySystemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
