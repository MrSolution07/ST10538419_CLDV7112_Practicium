using System.Text.Json;

namespace ST10538419_CLDV7112_Practicium.Application.Farm;

public static class FarmJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
