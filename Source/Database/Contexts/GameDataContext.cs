using AlienInvasionLogistics.Source.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Contexts;

public class GameDataContext : DbContext
{
    public GameDataContext(DbContextOptions<GameDataContext> options) : base(options)
    {
    }

    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Nation> Nations { get; set; }
    public DbSet<StrategicWorldState> StrategicWorldStates { get; set; }
    public DbSet<SolarSystem> SolarSystems { get; set; }
    public DbSet<PlanetarySystem> PlanetarySystems { get; set; }
    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<StaticArtificialObject> StaticArtificialObjects { get; set; }
    public DbSet<MobileArtificialObject> MobileArtificialObjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<Player>()
            .HasOne(p => p.GameSession)
            .WithMany(gs => gs.Players)
            .HasForeignKey(p => p.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Player>()
            .HasOne(p => p.Nation)
            .WithOne(n => n.Player)
            .HasForeignKey<Nation>(n => n.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Shadow property: PlayerId (for owned entities)
        // ResearchState and ResourceState are owned by Player and share the same foreign key column name.
        // String-based configuration ensures both owned entities reference the same "PlayerId" column.
        // Refactoring safety: If Player's key property is renamed, this string must be updated manually.
        var foreignKeyPropertyNames = "PlayerId";
        modelBuilder.Entity<Player>().OwnsOne(p => p.ResearchState, researchBuilder =>
        {
            researchBuilder.WithOwner().HasForeignKey(foreignKeyPropertyNames);
            // Ignore navigation properties to other owned types (ResearchItem)
            // These would need to be stored as JSON or handled separately
            researchBuilder.Ignore(r => r.FinishedResearch);
            researchBuilder.Ignore(r => r.CurrentResearch);
            researchBuilder.Ignore(r => r.ResearchQueue);
            researchBuilder.Ignore(r => r.KnownResearch);
        });

        modelBuilder.Entity<Player>().OwnsOne(p => p.ResourceState, resourceBuilder =>
        {
            resourceBuilder.WithOwner().HasForeignKey(foreignKeyPropertyNames);
            // Ignore navigation property to other owned type (GameResource)
            resourceBuilder.Ignore(r => r.Resources);
        });
        
        modelBuilder
            .Entity<GameSession>()
            .HasOne(gs => gs.StrategicWorldState)
            .WithOne()  // No back-reference
            .HasForeignKey<GameSession>(gs => gs.StrategicWorldStateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<StrategicWorldState>()
            .HasOne(sws => sws.SolarSystem)
            .WithOne()
            .HasForeignKey<StrategicWorldState>(sws => sws.SolarSystemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<StrategicWorldState>()
            .HasMany(sws => sws.Nations)
            .WithOne(n => n.StrategicWorldState)
            .HasForeignKey(n => n.StrategicWorldStateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<SolarSystem>()
            .HasOne(ss => ss.CentralMass)
            .WithOne()
            .HasForeignKey<SolarSystem>(ss => ss.CentralMassId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Shadow property: SolarSystemId
        // EF Core creates this foreign key column in the database without a corresponding C# property.
        // Used here because PlanetarySystem doesn't need to explicitly know its parent SolarSystem in the domain model.
        // Access via: context.Entry(planetarySystem).Property("SolarSystemId").CurrentValue
        modelBuilder
            .Entity<SolarSystem>()
            .HasMany(ss => ss.PlanetarySystems)
            .WithOne()  // No back-reference property in PlanetarySystem
            .HasForeignKey("SolarSystemId")  // Shadow property (not in PlanetarySystem model)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<PlanetarySystem>()
            .HasOne(ps => ps.CentralMass)
            .WithOne()
            .HasForeignKey<PlanetarySystem>(ps => ps.CentralMassId)
            .OnDelete(DeleteBehavior.Restrict);

        // Shadow property: PlanetarySystemId
        // EF Core creates this foreign key column for moons/satellites without requiring a C# property.
        // CelestialBody uses ParentBodyId for orbital hierarchy, but needs separate tracking for planetary system membership.
        // Access via: context.Entry(celestialBody).Property("PlanetarySystemId").CurrentValue
        modelBuilder
            .Entity<PlanetarySystem>()
            .HasMany(ps => ps.CelestialBodies)
            .WithOne()  // No back-reference property
            .HasForeignKey("PlanetarySystemId")  // Shadow property (not in CelestialBody model)
            .OnDelete(DeleteBehavior.Cascade);

        // Shadow property: ParentPlanetarySystemId
        // EF Core creates this foreign key for nested planetary systems (e.g., dwarf planet systems within larger planet systems).
        // Separate from solar system hierarchy to allow flexible system-within-system relationships.
        // Access via: context.Entry(planetarySystem).Property("ParentPlanetarySystemId").CurrentValue
        modelBuilder
            .Entity<PlanetarySystem>()
            .HasMany(ps => ps.PlanetarySystems)
            .WithOne()
            .HasForeignKey("ParentPlanetarySystemId")  // Shadow property (not in PlanetarySystem model)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<CelestialBody>()
            .HasOne(cb => cb.ParentBody)
            .WithMany(cb => cb.ChildBodies)
            .HasForeignKey(cb => cb.ParentBodyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CelestialBody>().OwnsMany(cb => cb.Orbits, orbitBuilder =>
        {
            // Navigation properties to entities are not supported in owned types
            // Use the ID lists (StaticArtificialObjectIds, MobileArtificialObjectIds) for relationships
            orbitBuilder.Ignore(o => o.StaticArtificialObjects);
            orbitBuilder.Ignore(o => o.MobileArtificialObjects);
        });

        modelBuilder.Entity<CelestialBody>().OwnsMany(cb => cb.LandingSites, siteBuilder =>
        {
            // Navigation properties to entities are not supported in owned types
            // Use the ID lists (StaticArtificialObjectIds, MobileArtificialObjectIds) for relationships
            siteBuilder.Ignore(s => s.StaticArtificialObjects);
            siteBuilder.Ignore(s => s.MobileArtificialObjects);
        });

        modelBuilder.Entity<CelestialBody>().OwnsMany(cb => cb.ResourceDeposits);

        modelBuilder
            .Entity<Nation>()
            .HasMany(n => n.OccupiedCelestialBodies)
            .WithOne()
            .HasForeignKey(cb => cb.OccupyingNationId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder
            .Entity<Nation>()
            .HasMany(n => n.OwnedStaticObjects)
            .WithOne(o => o.OwningNation)
            .HasForeignKey(o => o.OwningNationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Nation>()
            .HasMany(n => n.OwnedMobileObjects)
            .WithOne(o => o.OwningNation)
            .HasForeignKey(o => o.OwningNationId)
            .OnDelete(DeleteBehavior.Cascade);
        

        modelBuilder.Entity<Nation>().OwnsOne(n => n.StartingResources);
        modelBuilder.Entity<Nation>().OwnsOne(n => n.StartingResearch, researchBuilder =>
        {
            // Ignore navigation property to other owned type (ResearchItem)
            researchBuilder.Ignore(r => r.startingResearch);
        });
        
        // GameSession indexes for common queries
        modelBuilder.Entity<GameSession>()
            .HasIndex(gs => gs.SaveName)
            .HasDatabaseName("IX_GameSession_SaveName");

        modelBuilder.Entity<GameSession>()
            .HasIndex(gs => gs.SessionName)
            .HasDatabaseName("IX_GameSession_SessionName");

        modelBuilder.Entity<GameSession>()
            .HasIndex(gs => gs.CreatedAt)
            .HasDatabaseName("IX_GameSession_CreatedAt");

        modelBuilder.Entity<GameSession>()
            .HasIndex(gs => new { gs.LastSavedAt, gs.CreatedAt })
            .HasDatabaseName("IX_GameSession_LastSaved_Created");

        // Nation indexes for joins
        modelBuilder.Entity<Nation>()
            .HasIndex(n => n.PlayerId)
            .HasDatabaseName("IX_Nation_PlayerId");

        modelBuilder.Entity<Nation>()
            .HasIndex(n => n.StrategicWorldStateId)
            .HasDatabaseName("IX_Nation_StrategicWorldStateId");

        // CelestialBody indexes for hierarchy traversal and filtering
        modelBuilder.Entity<CelestialBody>()
            .HasIndex(cb => cb.ParentBodyId)
            .HasDatabaseName("IX_CelestialBody_ParentBodyId");

        modelBuilder.Entity<CelestialBody>()
            .HasIndex(cb => cb.BodyType)
            .HasDatabaseName("IX_CelestialBody_BodyType");

        // Player index for game session queries
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.GameSessionId)
            .HasDatabaseName("IX_Player_GameSessionId");
    }
}