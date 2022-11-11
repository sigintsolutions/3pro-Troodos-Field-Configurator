using Thingsboard.Enums;
using Thingsboard.Requests;
using RestSharp;
using System.Text.Json;

namespace Thingsboard.Auth
{
    public class AuthHelper
    {
        public static async Task<JWTResponse?> GetJWTTokenAsync(JWTRequest jwtRequest, RestClient client)
        {
            RestRequest request = new RestRequest("/auth/login")
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Accept", "application/json")
                .AddJsonBody(JsonSerializer.Serialize(jwtRequest));

            RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.POST);

            JWTResponse? JWTJSONResponse;

            if (response != null)
            {
                JWTJSONResponse = JsonSerializer.Deserialize<JWTResponse>(response.Content!);
            }
            else
            {
                return null;
            }

            if (JWTJSONResponse == null)
            {
                return null;
            }

            if (JWTJSONResponse.token == null)
            {
                return null;
            }
            return JWTJSONResponse;
        }
    }
}
