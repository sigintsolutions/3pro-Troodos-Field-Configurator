namespace Thingsboard.DeviceEntry;

/// <summary>
/// This class stores collection of telemetry entries, all of which were transmitted on the same timestamp. 
/// </summary>
public class TelemetryPackage
{
    /// <summary>
    /// this should be in utc
    /// </summary>
    public DateTime DateTime { get; set; }
    public long Timestamp { get; set; } = 0;
    public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
}
