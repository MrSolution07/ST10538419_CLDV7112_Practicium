using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Task (b): UserAccounts holds the username, the password hash, and the role.
        var account = modelBuilder.Entity<UserAccount>();
        account.ToTable("UserAccounts");
        account.HasKey(entity => entity.Id);
        account.Property(entity => entity.Username).HasMaxLength(40).IsRequired();
        account.Property(entity => entity.PasswordHash).HasMaxLength(200).IsRequired();
        account.Property(entity => entity.Role).HasMaxLength(40).IsRequired();
        account.Property(entity => entity.CreatedAt).IsRequired();
        account.Property(entity => entity.UpdatedAt).IsRequired();
        account.HasIndex(entity => entity.Username).IsUnique();

        var device = modelBuilder.Entity<Device>();
        device.ToTable("Devices");
        device.HasKey(entity => entity.Id);
        device.Property(entity => entity.Name).HasMaxLength(80).IsRequired();
        device.Property(entity => entity.FieldName).HasMaxLength(80).IsRequired();
        device.Property(entity => entity.OperationalStatus).HasMaxLength(40).IsRequired();
        device.Property(entity => entity.CreatedAt).IsRequired();
        device.Property(entity => entity.UpdatedAt).IsRequired();
        device.HasIndex(entity => entity.FieldName);
    }
}
