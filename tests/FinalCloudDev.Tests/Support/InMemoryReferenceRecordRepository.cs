using ST10538419_CLDV7112_Practicium.Application.Records;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Tests.Support;

public sealed class InMemoryReferenceRecordRepository : IReferenceRecordRepository
{
    private readonly List<ReferenceRecord> items = [];

    public Task<IReadOnlyList<ReferenceRecord>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        IEnumerable<ReferenceRecord> query = items.Where(record => !record.IsArchived);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(record => record.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyList<ReferenceRecord> result = query.OrderByDescending(record => record.UpdatedAt).ToList();
        return Task.FromResult(result);
    }

    public Task<int> CountActiveAsync(CancellationToken cancellationToken) =>
        Task.FromResult(items.Count(record => !record.IsArchived));

    public Task<ReferenceRecord?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(items.FirstOrDefault(record => record.Id == id));

    public Task AddAsync(ReferenceRecord record, CancellationToken cancellationToken)
    {
        items.Add(record);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
