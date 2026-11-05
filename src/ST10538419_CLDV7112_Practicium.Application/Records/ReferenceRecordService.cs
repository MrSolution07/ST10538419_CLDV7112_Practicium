using ST10538419_CLDV7112_Practicium.Application.Common;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Application.Records;

public sealed class ReferenceRecordService(IReferenceRecordRepository repository) : IReferenceRecordService
{
    public const int TitleMaxLength = 120;
    public const int NotesMaxLength = 2000;

    public Task<IReadOnlyList<ReferenceRecord>> ListAsync(string? search, CancellationToken cancellationToken) =>
        repository.ListAsync(search, cancellationToken);

    public Task<int> CountActiveAsync(CancellationToken cancellationToken) =>
        repository.CountActiveAsync(cancellationToken);

    public Task<ReferenceRecord?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetAsync(id, cancellationToken);

    public async Task<ServiceResult<ReferenceRecord>> CreateAsync(
        ReferenceRecordInput input,
        CancellationToken cancellationToken)
    {
        var error = Validate(input);
        if (error is not null)
        {
            return ServiceResult<ReferenceRecord>.Fail(error);
        }

        var record = new ReferenceRecord
        {
            Title = input.Title.Trim(),
            Notes = NormalizeNotes(input.Notes)
        };

        await repository.AddAsync(record, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<ReferenceRecord>.Ok(record);
    }

    public async Task<ServiceResult<ReferenceRecord>> UpdateAsync(
        Guid id,
        ReferenceRecordInput input,
        CancellationToken cancellationToken)
    {
        var error = Validate(input);
        if (error is not null)
        {
            return ServiceResult<ReferenceRecord>.Fail(error);
        }

        var record = await repository.GetAsync(id, cancellationToken);
        if (record is null || record.IsArchived)
        {
            return ServiceResult<ReferenceRecord>.Fail("Record was not found.");
        }

        record.Title = input.Title.Trim();
        record.Notes = NormalizeNotes(input.Notes);
        record.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<ReferenceRecord>.Ok(record);
    }

    public async Task<ServiceResult<ReferenceRecord>> ArchiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await repository.GetAsync(id, cancellationToken);
        if (record is null || record.IsArchived)
        {
            return ServiceResult<ReferenceRecord>.Fail("Record was not found.");
        }

        record.IsArchived = true;
        record.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return ServiceResult<ReferenceRecord>.Ok(record);
    }

    private static string? Validate(ReferenceRecordInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            return "Title is required.";
        }

        if (input.Title.Trim().Length > TitleMaxLength)
        {
            return $"Title must be {TitleMaxLength} characters or fewer.";
        }

        if (input.Notes is not null && input.Notes.Trim().Length > NotesMaxLength)
        {
            return $"Notes must be {NotesMaxLength} characters or fewer.";
        }

        return null;
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return null;
        }

        return notes.Trim();
    }
}
