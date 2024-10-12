using Battleships.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Battleships.DAL.Context
{
    public class BattleshipDbContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<Ship> Ships { get; set; }

        public BattleshipDbContext(DbContextOptions<BattleshipDbContext> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Board>()
        //        .HasMany(b => b.Ships)
        //        .WithOne()
        //        .OnDelete(DeleteBehavior.Cascade);

        //    base.OnModelCreating(modelBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships and constraints between entities.

            // Configure the Board entity.
            modelBuilder.Entity<Board>(entity =>
            {
                // Define primary key.
                entity.HasKey(b => b.Id);
                          
                // Define a required field for SerializedGrid.
                entity.Property(b => b.SerializedGrid);

                // Configure relationships with Ships (one-to-many).
                entity.HasMany(b => b.Ships)
                    .WithOne()
                    .HasForeignKey(s => s.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure the Ship entity.
            modelBuilder.Entity<Ship>(entity =>
            {
                // Define primary key.
                entity.HasKey(s => s.Id);         

                // Define required fields for Ship.
                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Size)
                    .IsRequired();

                entity.Property(s => s.Hits)
                    .IsRequired();

                // Configure the relationship to Board (many-to-one).
                entity.HasOne<Board>()
                    .WithMany(b => b.Ships)
                    .HasForeignKey(s => s.BoardId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

