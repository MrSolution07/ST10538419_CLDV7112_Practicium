using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ReferenceRecord> ReferenceRecords => Set<ReferenceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var record = modelBuilder.Entity<ReferenceRecord>();
        record.ToTable("ReferenceRecords");
        record.HasKey(entity => entity.Id);
        record.Property(entity => entity.Title).HasMaxLength(120).IsRequired();
        record.Property(entity => entity.Notes).HasMaxLength(2000);
        record.Property(entity => entity.CreatedAt).IsRequired();
        record.Property(entity => entity.UpdatedAt).IsRequired();
        record.HasIndex(entity => entity.Title);
    }
}
