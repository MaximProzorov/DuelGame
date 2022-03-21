using Game.Models;
using Microsoft.EntityFrameworkCore;

namespace Game.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserModel>()
                .HasIndex(t => t.Username)
                .IsUnique();
            builder.Entity<UserModel>()
                .HasKey(t => t.PlayerId);

            builder.Entity<RoomModel>()
                .HasKey(t => t.RoomId);
            builder.Entity<RoomModel>()
                .HasMany(t => t.Users)
                .WithOne(t => t.Room)
                .HasForeignKey(t => t.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
