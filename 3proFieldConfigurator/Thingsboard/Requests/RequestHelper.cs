using Thingsboard.Enums;
using RestSharp;
using Serilog;

namespace Thingsboard.Requests
{
    public class RequestHelper
    {
        /// <summary>
        /// generalized response handler. 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<RestResponse?> ExecuteRestRequest(RestRequest request, RestClient client, RestType type)
        {
            RestResponse? response;
            try
            {
                if (type == RestType.POST)
                {
                    response = await client.ExecutePostAsync(request);
                }
                else if (type == RestType.GET)
                {
                    response = await client.ExecuteGetAsync(request);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"error performing request, {ex.Message}", ex.Message);
                return null;
            }

            if (response == null)
            {
                return null;
            }

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Log.Error("Failed to complete request");
                return null;
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            if (response.Content == null)
            {
                return null;
            }
            return response;
        }

        public static async Task<bool> ExecuteRestRequestReturnOk(RestRequest request, RestClient client, RestType type, CancellationToken cancellationToken = default)
        {
            RestResponse? response;
            try
            {
                if (type == RestType.POST)
                {
                    response = await client.ExecutePostAsync(request, cancellationToken);
                }
                else if (type == RestType.GET)
                {
                    response = await client.ExecuteGetAsync(request, cancellationToken);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("error performing request, {ex.Message}", ex.Message);
                return false;
            }

            if (response == null)
            {
                return false;
            }

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Log.Error("Failed to complete request");
                return false;
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            if (response.Content == null)
            {
                return false;
            }
            return true;
        }
    }
}
