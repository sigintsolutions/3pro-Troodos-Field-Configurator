using AdvancedThingsboardEmulator.Thingsboard.DeviceController;
using Thingsboard.Enums;
using Thingsboard.Requests;
using RestSharp;
using System.Text.Json;
using _3proFieldConfigurator.Extensions;

namespace Thingsboard.DeviceController;

public class DeviceController
{
    public static async Task<GetDeviceCredentialsByDeviceIdResponse?> GetDeviceCrentialsByDeviceId(string deviceId, RestClient client)
    {
        string url = $"device/{deviceId}/credentials";

        RestRequest request = new RestRequest(url)
            .AddHeader("Accept", "application/json");

        RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.GET);

        GetDeviceCredentialsByDeviceIdResponse? deviceCredentialsResponse;

        if (response == null)
        {
            return null;
        }
        else
        {
            deviceCredentialsResponse = JsonSerializer.Deserialize<GetDeviceCredentialsByDeviceIdResponse?>(response.Content!);
        }
        return deviceCredentialsResponse;
    }

    public static async Task<TBDeviceEntry?> GetTenantDevice(SimpleDeviceEntry.DeviceEntry deviceEntry, RestClient client)
    {
        string url = $"/tenant/devices?deviceName={deviceEntry.DeviceName}";
        RestRequest request = new RestRequest(url)
           .AddHeader("Accept", "application/json");

        RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.GET);

        if (response != null)
        {
            var tbDeviceEntry = JsonSerializer.Deserialize<TBDeviceEntry>(response.Content!);
            return tbDeviceEntry;
        }

        return null;
    }

    public static async Task<GetTenantDevicesResponse?> GetTenantDevicesAsync(UserTypeEnum userType, string customerId, RestClient client)
    {
        string url;
        string? userTypeStr = userType.GetEnumDisplayName();

        if (userType == UserTypeEnum.Tenant)
        {
            //TODO this needs fixing in case there are more than 100 devices. 
            url = $"/{userTypeStr}/deviceInfos?pageSize=100&page=0";
        }
        else
        {
            url = $"/{userTypeStr}/{customerId}/deviceInfos?pageSize=100&page=0";
        }

        RestRequest request = new RestRequest(url)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("Accept", "application/json");

        RestResponse? response = await RequestHelper.ExecuteRestRequest(request, client, RestType.GET);

        GetTenantDevicesResponse? deviceListResponse;

        if (response == null)
        {
            return null;
        }
        else
        {
            deviceListResponse = JsonSerializer.Deserialize<GetTenantDevicesResponse?>(response.Content!);
        }
        return deviceListResponse;
    }
}
