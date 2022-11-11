namespace Thingsboard.TelemetryTransmission;

#pragma warning disable IDE1006 // Naming Styles
public class TelemetryPackage
{
    public long ts { get; set; } = 0;
    public Dictionary<string, string> values { get; set; } = new Dictionary<string, string>();
}
#pragma warning restore IDE1006 // Naming Style

