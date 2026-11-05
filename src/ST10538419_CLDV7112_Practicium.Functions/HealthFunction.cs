using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ST10538419_CLDV7112_Practicium.Functions;

public class HealthFunction
{
    [Function("Health")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.WriteString("ok");
        return response;
    }
}
