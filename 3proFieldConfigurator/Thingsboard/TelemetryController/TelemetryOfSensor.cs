namespace Thingsboard.TelemetryController;

public class TelemetryOfSensor
{
    public KeyValuePair<string, List<TelemetryPoint>> Telemetry { get; set; } = new KeyValuePair<string, List<TelemetryPoint>>();

    #region Ctors
    public TelemetryOfSensor() { }
    public TelemetryOfSensor(string telemetryName, List<TelemetryPoint> telemetryList)
    {
        Telemetry = new KeyValuePair<string, List<TelemetryPoint>>(telemetryName, telemetryList);
    }
    #endregion
}
