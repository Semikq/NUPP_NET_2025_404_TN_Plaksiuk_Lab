using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Zoo.Infrastructure;

public class ZooDbContext : IdentityDbContext<User>
{
    public ZooDbContext(DbContextOptions<ZooDbContext> options)
        : base(options)
    {
    }

    public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Species).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Age).IsRequired();
        });
    }
}

public class Animal
{
    public Guid Id { get; set; }
    public string Species { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Guid EnclosureId { get; set; }
}