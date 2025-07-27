using LegalVibes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LegalVibes.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.HasMany(e => e.Projects)
                  .WithOne(p => p.User)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Clients)
                  .WithOne(c => c.User)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Client entity
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.HasMany(e => e.Projects)
                  .WithOne(p => p.Client)
                  .HasForeignKey(p => p.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.TrademarkName).HasMaxLength(200);
            entity.Property(e => e.TrademarkDescription).HasMaxLength(1000);
            entity.Property(e => e.GoodsAndServices).HasMaxLength(2000);
            entity.Property(e => e.SpecialConsiderations).HasMaxLength(1000);
            entity.HasMany(e => e.Documents)
                  .WithOne(d => d.Project)
                  .HasForeignKey(d => d.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Document entity
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Version).HasMaxLength(50);
            entity.Property(e => e.GenerationPrompt).HasMaxLength(2000);
            entity.Property(e => e.AIModel).HasMaxLength(100);
        });
    }
} 