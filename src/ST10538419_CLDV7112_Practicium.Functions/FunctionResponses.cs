using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using ST10538419_CLDV7112_Practicium.Application.Farm;

namespace ST10538419_CLDV7112_Practicium.Functions;

internal static class FunctionResponses
{
    public static bool Authorized(HttpRequestData request, IConfiguration configuration)
    {
        var expected = configuration["Functions:SharedSecret"];
        if (string.IsNullOrWhiteSpace(expected))
        {
            return false;
        }

        if (!request.Headers.TryGetValues("X-ArgiStream-Key", out var values))
        {
            return false;
        }

        var provided = values.FirstOrDefault();
        return provided is not null && SecretMatches(provided, expected);
    }

    public static async Task<HttpResponseData> Json(HttpRequestData request, HttpStatusCode status, object body)
    {
        var response = request.CreateResponse(status);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(body, FarmJson.Options));
        return response;
    }

    public static string? QueryValue(Uri url, string key)
    {
        var query = url.Query.TrimStart('?');
        foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var bits = part.Split('=', 2);
            if (bits.Length == 2 && string.Equals(Uri.UnescapeDataString(bits[0]), key, StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(bits[1]);
            }
        }

        return null;
    }

    private static bool SecretMatches(string provided, string expected)
    {
        var left = System.Text.Encoding.UTF8.GetBytes(provided);
        var right = System.Text.Encoding.UTF8.GetBytes(expected);
        return left.Length == right.Length && System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(left, right);
    }
}
