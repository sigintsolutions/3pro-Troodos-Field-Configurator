using Thingsboard.Enums;
using Thingsboard.Requests;
using RestSharp;
using Serilog;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Thingsboard.Constants.Constants;

namespace Thingsboard.TelemetryController;

public class TelemetryController
{
    public static async Task<string[]?> GetTimeseriesKeysAsync(string deviceID, RestClient client)
    {
        string url = $"/plugins/telemetry/DEVICE/{deviceID}/keys/timeseries";
        RestRequest request = new RestRequest(url)
            .AddHeader("Accept", "application/json");

        RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.GET);
        if (response == null)
        {
            return null;
        }

        var telemetryListResponse = JsonSerializer.Deserialize<string[]>(response.Content!);
        return telemetryListResponse;
    }

    public static async Task<TelemetryOfDevice?> GetAllTelemetryOfDeviceAsync(string deviceID,
                                                                    string[] telemetryKeys,
                                                                    long startingTimeMs,
                                                                    long endingTimeMs,
                                                                    RestClient client)
    {
        const int datapointLimit = 200;
        TelemetryOfDevice deviceTelemetry = new();

        foreach (var telemetryKey in telemetryKeys)
        {
            //progressBar.PerformStep();
            List<TelemetryPoint> allTelemetryDataPoints = new();
            Log.Information($"Getting data from telemetry Key: {telemetryKey}");

            List<TelemetryPoint>? telemetryDataPointList;

            long newEndingTimestamp = endingTimeMs;
            do
            {
                Log.Information($"Last timestamp {newEndingTimestamp}");
                telemetryDataPointList = await GetPartialTelemetryOfAKeyOfADeviceAsync(deviceID, telemetryKey, datapointLimit, startingTimeMs, newEndingTimestamp - 1, client);
                if (telemetryDataPointList == null)
                {
                    break;
                }
                allTelemetryDataPoints.AddRange(telemetryDataPointList);
                newEndingTimestamp = telemetryDataPointList.Last().ts;
                await Task.Delay(DELAY_BETWEEN_TRANSMISSIONS_MS); 
            } while (telemetryDataPointList.Count == datapointLimit);

            Log.Information($"Got {allTelemetryDataPoints.Count} data points total");
            //finaly add all together  on a complete structure
            deviceTelemetry.AllTelemetry.Add(new TelemetryOfSensor(telemetryKey, allTelemetryDataPoints));
        }
        return deviceTelemetry;
    }

    private static async Task<List<TelemetryPoint>?> GetPartialTelemetryOfAKeyOfADeviceAsync(string deviceID,
                                                                                             string telemetryKey,
                                                                                             int datapointLimit,
                                                                                             long startTimeMs,
                                                                                             long endTimeMs,
                                                                                             RestClient client)
    {
        string url = $"/plugins/telemetry/DEVICE/{deviceID}/values/timeseries?keys={telemetryKey}&startTs={startTimeMs}&endTs={endTimeMs}&limit={datapointLimit}";

        RestRequest request = new RestRequest(url)
            .AddHeader("Accept", "application/json");

        RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.GET);

        if (response == null)
        {
            return null;
        }

        //dynamic because it is unknown of exactly what we would get back from the response
        var parsedJSON = JsonNode.Parse(response.Content!);

        if (parsedJSON == null)
        {
            return null;
        }

        //it needs to be cast as objects...for reasons...Figure out why to write a correct comment. 
        var telemetryEntry = parsedJSON.AsObject().FirstOrDefault();

        if (telemetryEntry.Key == null)
        {
            return null;
        }

        //now that we know the name of the property get the array of the measurement which would be an array of these structures
        //with properties "ts" (ie timestamp) and "value"
        //[{
        //    "ts": 1657283096999,
        //    "value": "35.110492"
        //}]
        //use the name to fetch the datapoints
        if (parsedJSON[telemetryEntry.Key] == null)
        {
            return null;
        }

        var telemetryDataPointJSONArray = parsedJSON[telemetryEntry.Key]!.AsArray();

        //create an array to hold the data point structures. 
        List<TelemetryPoint> telemetryDataPoint = new();

        //for each telemetry data point copy the values from the json to the array.
        for (int i = 0; i < telemetryDataPointJSONArray.Count; i++)
        {
            long ts = long.Parse(telemetryDataPointJSONArray[i]!["ts"]!.ToString());
            string value = telemetryDataPointJSONArray[i]!["value"]!.ToString();
            telemetryDataPoint.Add(new TelemetryPoint(ts, value));
        }

        Log.Information($"Got {telemetryDataPointJSONArray.Count} data points");
        return telemetryDataPoint;
    }

    public static async Task PostDeviceAttributes(SimpleDeviceEntry.DeviceEntry deviceEntry,
                                                  AttributesScope scope,
                                                  RestClient client)
    {
        string url = $"/plugins/telemetry/{deviceEntry.DeviceId}/{scope}";
        RestRequest request = new RestRequest(url)
            .AddHeader("Accept", "application/json")
            .AddJsonBody(deviceEntry.DeviceAttributes); //add json body serializes the object automatically

        await RequestHelper.ExecuteRestRequest(request, client, RestType.POST);
    }
}
