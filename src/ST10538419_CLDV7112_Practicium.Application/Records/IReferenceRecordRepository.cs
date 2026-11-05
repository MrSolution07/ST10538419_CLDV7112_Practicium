using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Records;

public interface IReferenceRecordRepository
{
    Task<IReadOnlyList<ReferenceRecord>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<int> CountActiveAsync(CancellationToken cancellationToken);

    Task<ReferenceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(ReferenceRecord record, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
