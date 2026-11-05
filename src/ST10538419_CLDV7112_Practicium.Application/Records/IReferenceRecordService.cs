using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Records;

public interface IReferenceRecordService
{
    Task<IReadOnlyList<ReferenceRecord>> ListAsync(string? search, CancellationToken cancellationToken);

    Task<int> CountActiveAsync(CancellationToken cancellationToken);

    Task<ReferenceRecord?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ServiceResult<ReferenceRecord>> CreateAsync(ReferenceRecordInput input, CancellationToken cancellationToken);

    Task<ServiceResult<ReferenceRecord>> UpdateAsync(Guid id, ReferenceRecordInput input, CancellationToken cancellationToken);

    Task<ServiceResult<ReferenceRecord>> ArchiveAsync(Guid id, CancellationToken cancellationToken);
}
