using ST10538419_CLDV7112_Practicium.Application.Records;
using ST10538419_CLDV7112_Practicium.Tests.Support;

namespace ST10538419_CLDV7112_Practicium.Tests;

public class ReferenceRecordServiceTests
{
    private readonly ReferenceRecordService service = new(new InMemoryReferenceRecordRepository());

    [Fact]
    public async Task Create_rejects_blank_title()
    {
        var result = await service.CreateAsync(new ReferenceRecordInput("  ", null), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Title is required.", result.Error);
    }

    [Fact]
    public async Task Create_rejects_long_title()
    {
        var title = new string('a', ReferenceRecordService.TitleMaxLength + 1);
        var result = await service.CreateAsync(new ReferenceRecordInput(title, null), CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Create_trims_title_and_blank_notes()
    {
        var result = await service.CreateAsync(new ReferenceRecordInput("  Lab note  ", "   "), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal("Lab note", result.Value!.Title);
        Assert.Null(result.Value.Notes);
    }

    [Fact]
    public async Task Archive_hides_record_from_list()
    {
        var created = await service.CreateAsync(new ReferenceRecordInput("Keep", "notes"), CancellationToken.None);

        var archived = await service.ArchiveAsync(created.Value!.Id, CancellationToken.None);
        var listed = await service.ListAsync(null, CancellationToken.None);

        Assert.True(archived.Succeeded);
        Assert.Empty(listed);
    }

    [Fact]
    public async Task Update_missing_record_fails()
    {
        var result = await service.UpdateAsync(Guid.NewGuid(), new ReferenceRecordInput("Title", null), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("Record was not found.", result.Error);
    }
}
