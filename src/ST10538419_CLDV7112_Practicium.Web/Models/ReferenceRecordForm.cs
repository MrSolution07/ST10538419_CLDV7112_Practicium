using System.ComponentModel.DataAnnotations;
using ST10538419_CLDV7112_Practicium.Application.Records;

namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class ReferenceRecordForm
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(ReferenceRecordService.TitleMaxLength)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(ReferenceRecordService.NotesMaxLength)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public ReferenceRecordInput ToInput() => new(Title, Notes);

    public static ReferenceRecordForm FromRecord(Guid id, string title, string? notes) => new()
    {
        Id = id,
        Title = title,
        Notes = notes
    };
}
