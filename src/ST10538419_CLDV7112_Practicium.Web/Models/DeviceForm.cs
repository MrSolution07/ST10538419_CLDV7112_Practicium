using System.ComponentModel.DataAnnotations;
using ST10538419_CLDV7112_Practicium.Application.Devices;
using ST10538419_CLDV7112_Practicium.Domain;
using ST10538419_CLDV7112_Practicium.Domain.Entities;

namespace ST10538419_CLDV7112_Practicium.Web.Models;

public sealed class DeviceForm
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(DeviceService.NameMaxLength)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Field name is required.")]
    [StringLength(DeviceService.FieldMaxLength)]
    [Display(Name = "Field")]
    public string FieldName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Operational status")]
    public string OperationalStatus { get; set; } = DeviceStatuses.Idle;

    public DeviceInput ToInput() => new(Name, FieldName, OperationalStatus);

    public static DeviceForm FromDevice(Device device) => new()
    {
        Id = device.Id,
        Name = device.Name,
        FieldName = device.FieldName,
        OperationalStatus = device.OperationalStatus
    };
}
