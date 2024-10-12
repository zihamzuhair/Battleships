using Battleships.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Battleships.DAL.Context
{
    public class BattleshipDbContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Fleet> Fleets { get; set; }

        public BattleshipDbContext(DbContextOptions<BattleshipDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         
            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.SerializedGrid);

                // One-to-One relationship between Board and Fleet.
                entity.HasOne(b => b.Fleet)
                      .WithOne(f => f.Board)
                      .HasForeignKey<Board>(b => b.FleetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Fleet>(entity =>
            {
                entity.HasKey(f => f.Id);

                // One-to-Many relationship between Fleet and Ships.
                entity.HasMany(f => f.Ships)
                      .WithOne(s => s.Fleet)
                      .HasForeignKey(s => s.FleetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ship>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Size).IsRequired();
                entity.Property(s => s.Hits).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }    
    }
}

