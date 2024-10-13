using Battleships.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Battleships.DAL.Context
{
    public class BattleshipDbContext : DbContext
    {
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<Ship> Ships { get; set; } = null!;
        public DbSet<Fleet> Fleets { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!; 

        public BattleshipDbContext(DbContextOptions<BattleshipDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Player entity
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.IsComputer)
                      .IsRequired();
                entity.Property(p => p.Score)  // Configure Score property
                      .IsRequired()
                      .HasDefaultValue(0);  // Set default score to 0

                entity.HasOne(p => p.Board)
                      .WithOne()
                      .HasForeignKey<Player>(p => p.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Fleet)
                      .WithOne()
                      .HasForeignKey<Player>(p => p.FleetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure the Board entity
            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.SerializedGrid)
                      .IsRequired(false);
                entity.Property(b => b.UserId)
                      .IsRequired();
            });

            // Configure the Fleet entity
            modelBuilder.Entity<Fleet>(entity =>
            {
                entity.HasKey(f => f.Id);

                // One-to-Many relationship between Fleet and Ships
                entity.HasMany(f => f.Ships)
                      .WithOne(s => s.Fleet)
                      .HasForeignKey(s => s.FleetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure the Ship entity
            modelBuilder.Entity<Ship>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(s => s.Size)
                      .IsRequired();
                entity.Property(s => s.Hits)
                      .IsRequired();

                // Ensure the FleetId is properly mapped to avoid shadow property creation
                entity.HasOne(s => s.Fleet)
                      .WithMany(f => f.Ships)
                      .HasForeignKey(s => s.FleetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

