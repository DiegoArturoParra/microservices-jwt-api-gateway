using Catalog.Gateway.Dtos;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Catalog.Gateway.Core
{
    public class ApiKeyHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!ValidateKey(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                var errorData = new { message = "Unauthorized API key", status = 401 };
                var json = JsonSerializer.Serialize(errorData);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return response;
            }
            return await base.SendAsync(request, cancellationToken);
        }

        private bool ValidateKey(HttpRequestMessage request)
        {
            if (request.Headers.TryGetValues("ApiKey", out IEnumerable<string> myHeaderList))
            {
                var myHeader = myHeaderList.First();

                var json = JsonSerializer.Deserialize<ApiKeyDto>(myHeader);

                // buscar en bd la key de la aplicacion por la que se esta haciendo la peticion y aplicacion dentro del json 

                if (json == null)
                    return false;


                switch (json.Application)
                {

                    case "MVC_GDM":
                        return json.Key == "123456";

                    case "MVC_DIM":
                        return json.Key == "654321";

                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
