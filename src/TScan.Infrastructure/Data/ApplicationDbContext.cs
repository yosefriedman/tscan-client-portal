using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TScan.Web.Models;

namespace TScan.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Firm> Firms { get; set; }
    public DbSet<Matter> Matters { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Firm configuration
        modelBuilder.Entity<Firm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.HasMany(e => e.Users).WithOne(u => u.Firm).HasForeignKey(u => u.FirmId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Matters).WithOne(m => m.Firm).HasForeignKey(m => m.FirmId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Orders).WithOne(o => o.Firm).HasForeignKey(o => o.FirmId).OnDelete(DeleteBehavior.Cascade);
        });

        // Matter configuration
        modelBuilder.Entity<Matter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MatterName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SmokballMatterId).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.FirmId, e.SmokballMatterId }).IsUnique();
            entity.HasMany(e => e.Orders).WithOne(o => o.Matter).HasForeignKey(o => o.MatterId).OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrderType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProviderName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValue("Submitted");
            entity.HasMany(e => e.Documents).WithOne(d => d.Order).HasForeignKey(d => d.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        // Document configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired();
            entity.Property(e => e.IsEncrypted).HasDefaultValue(true);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
            entity.HasIndex(e => new { e.FirmId, e.CreatedAt });
        });
    }
}
