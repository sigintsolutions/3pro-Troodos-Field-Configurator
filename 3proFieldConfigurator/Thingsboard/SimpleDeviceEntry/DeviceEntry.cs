using Thingsboard.TelemetryController;
using System.ComponentModel;
using Thingsboard.DeviceEntry;
using Serilog;

namespace Thingsboard.SimpleDeviceEntry;

public class DeviceEntry
{
    [Browsable(false)]
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    [Browsable(false)]
    public string? DeviceType { get; set; }

    [Browsable(false)]
    public string? DeviceLabel { get; set; }

    [Browsable(false)]
    public string? AccessToken { get; set; } = string.Empty;

    [Browsable(false)]
    public Dictionary<string, object> DeviceAttributes { get; set; } = new Dictionary<string, object>();

    [Browsable(false)]
    public List<string> TelemetryKeys { get; set; } = new List<string>();

    [Browsable(false)]
    public TelemetryOfDevice TelemetryData { get; set; } = new TelemetryOfDevice();

    #region ctors
    public DeviceEntry() { }

    public DeviceEntry(string deviceName)
    {
        DeviceName = deviceName;
    }

    //used for generating device
    public DeviceEntry(string deviceName, Dictionary<string, object> deviceAttributes)
    {
        DeviceName = deviceName;
        DeviceAttributes = deviceAttributes;
    }

    public DeviceEntry(string deviceId, string deviceName, string deviceType, string deviceLabel)
    {
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        DeviceLabel = deviceLabel;
    }
    #endregion

    public SortedSet<long> ExtractUniqueTimestamps()
    {
        SortedSet<long> timestamps = new();
        foreach (var telemetryEntry in TelemetryData.AllTelemetry)
        {
            foreach (var telemetryValue in telemetryEntry.Telemetry.Value)
            {
                timestamps.Add(telemetryValue.ts);
            }
        }
        return timestamps;
    }

    /// <summary>
    /// This function uses a list of timestamps, and searches on all the telemetry entries to find the telemetries that
    /// were transmitted at the same time. 
    /// </summary>
    /// <param name="device">The device which contains the telemetry to process.</param>
    /// <param name="timestamps">The list of unique timestamps to group by.</param>
    /// <returns>Grouped telemetry data.</returns>
    public PackedTelemetryOfDevice ExtractAndGroupTelemetryPackages(SortedSet<long> timestamps)
    {
        //for each timestamps entry try to find all the data point in each telemetry key and store it on a telemetryPackageInstance
        PackedTelemetryOfDevice packedTelemetryOfDevice = new();

        // This variable is used to count the number of loops used to group the data.
        long loops = 0;

        //now for each of the distinct time stamp
        foreach (var timestamp in timestamps)
        {
            //make a new telemetry Package entry, that would have only the timestamp and ideally
            //at the end all the telemetry keys that were white-listed. 
            TelemetryPackage telemetryPackage = new()
            {
                Timestamp = timestamp, //and store the timestamp. 
                DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp)
            };

            //now for each telemetry entry, try to find a data point with that timestamp.
            foreach (var telemetryEntry in TelemetryData.AllTelemetry)
            {
                bool valueFound = false;
                foreach (var value in telemetryEntry.Telemetry.Value)
                {
                    loops++;

                    if (value.ts == timestamp) //that entry is found. Save it in the dictionary. 
                    {
                        telemetryPackage.Values.Add(telemetryEntry.Telemetry.Key, value.value);
                        //and remove that entry, so that the next iterations don't have to pass that data point again and speed up the subsequent searches. 
                        telemetryEntry.Telemetry.Value.Remove(value);
                        valueFound = true;
                        break;
                    }
                }
                if (!valueFound) //that value doesn't exist for that timestamp, so just use an empty string
                {
                    telemetryPackage.Values.Add(telemetryEntry.Telemetry.Key, string.Empty);
                }
            }

            //if we found all the values, then keep it in a list of telemetry packages. 
            packedTelemetryOfDevice.AllTelemetry.Add(telemetryPackage);
            string formattedDate = telemetryPackage.DateTime.ToString("yyyy/MM/dd HH:mm");
            Log.Information($"Found package at timestamp {timestamp}, {formattedDate}");
        }

        Log.Information($"Finished on {loops} loops");
        return packedTelemetryOfDevice;
    }

    /// <summary>
    /// given a string array blacklist, this function removes the blacklisted entries from the measurements of the device. 
    /// </summary>
    /// <param name="device">device structure that contains all the telemetry that was imported. </param>
    /// <param name="blacklist">blacklisted telemetry keys to be removed from the device structure.</param>
    public void RemoveBlacklistedTelemetryEntriesFromDevice(string[] blacklist)
    {
        //double loops, but performance loss is negligible. we are talking about <100 entries. 
        _ = TelemetryKeys.RemoveAll(s => blacklist.Contains(s));

        //do the same for the actual measurements. 
        for (int i = TelemetryData.AllTelemetry.Count - 1; i >= 0; i--)
        {
            //check if it is in the entry
            foreach (var blacklistedEntry in blacklist)
            {
                //and remove it. 
                if (TelemetryData.AllTelemetry[i].Telemetry.Key == blacklistedEntry)
                {
                    TelemetryData.AllTelemetry.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
