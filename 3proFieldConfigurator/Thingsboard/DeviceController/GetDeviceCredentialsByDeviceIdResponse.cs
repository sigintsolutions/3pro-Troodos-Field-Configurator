namespace AdvancedThingsboardEmulator.Thingsboard.DeviceController;
#pragma warning disable IDE1006 // Naming Styles
public class GetDeviceCredentialsByDeviceIdResponse
{
    public Id? id { get; set; }
    public long createdTime { get; set; }
    public Deviceid? deviceId { get; set; }
    public string? credentialsType { get; set; }
    public string? credentialsId { get; set; }
    public object? credentialsValue { get; set; }
    public class Id
    {
        public string? id { get; set; }
    }

    public class Deviceid
    {
        public string? entityType { get; set; }
        public string? id { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles
