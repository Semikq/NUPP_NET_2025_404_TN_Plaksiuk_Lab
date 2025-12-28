using Microsoft.EntityFrameworkCore;
using Zoo.Infrastructure.Models;

namespace Zoo.Infrastructure
{
    public class ZooContext : DbContext
    {
        public DbSet<AnimalModel> Animals { get; set; }
        public DbSet<BirdModel> Birds { get; set; }
        public DbSet<MammalModel> Mammals { get; set; }
        public DbSet<ZooKeeperModel> ZooKeepers { get; set; }
        public DbSet<FoodModel> Foods { get; set; }

        public ZooContext(DbContextOptions<ZooContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnimalModel>().ToTable("Animals");
            modelBuilder.Entity<BirdModel>().ToTable("Birds");
            modelBuilder.Entity<MammalModel>().ToTable("Mammals");

            modelBuilder.Entity<AnimalModel>()
                .HasOne(a => a.Food)
                .WithOne(f => f.Animal)
                .HasForeignKey<FoodModel>(f => f.AnimalModelId);

            modelBuilder.Entity<ZooKeeperModel>()
                .HasMany(z => z.Animals)
                .WithOne(a => a.ZooKeeper)
                .HasForeignKey(a => a.ZooKeeperModelId);
        }
    }
}