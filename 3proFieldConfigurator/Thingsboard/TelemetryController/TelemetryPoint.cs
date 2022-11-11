namespace Thingsboard.TelemetryController;

/// <summary>
/// Telemetry Point the same way that it is transmitted by thingsboard. 
/// </summary>
public class TelemetryPoint
{
#pragma warning disable IDE1006 // Naming Styles
    public long ts { get; set; }
    public string value { get; set; }
    #region ctors
    public TelemetryPoint(long ts, string value)
    {
        this.ts = ts;
        this.value = value;
    }
    #endregion
#pragma warning restore IDE1006 // Naming Styles
}