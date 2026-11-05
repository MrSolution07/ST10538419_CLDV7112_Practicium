using ST10538419_CLDV7112_Practicium.Application.Records;
using ST10538419_CLDV7112_Practicium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ST10538419_CLDV7112_Practicium.Infrastructure.Data;

public sealed class ReferenceRecordRepository(AppDbContext db) : IReferenceRecordRepository
{
    public async Task<IReadOnlyList<ReferenceRecord>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.ReferenceRecords.AsNoTracking().Where(record => !record.IsArchived);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(record => record.Title.Contains(term));
        }

        return await query
            .OrderByDescending(record => record.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken) =>
        db.ReferenceRecords.CountAsync(record => !record.IsArchived, cancellationToken);

    public Task<ReferenceRecord?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        db.ReferenceRecords.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(ReferenceRecord record, CancellationToken cancellationToken) =>
        await db.ReferenceRecords.AddAsync(record, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
