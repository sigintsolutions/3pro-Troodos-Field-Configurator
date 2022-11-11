using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3proFieldConfigurator.Models;

public class SettingsFormat
{
#pragma warning disable IDE1006 // Naming Styles
    public float dischargeRate { get; set; }
    public int elevation { get; set; }
    public bool enableBatteryAlarm { get; set; }
    public float batteryLimit { get; set; }
    public bool enableInactivityAlarm { get; set; }
    public float fieldCapacity { get; set; }
    public string fieldName { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    public float soilMoistureThreshold { get; set; }
    public float wettedArea { get; set; }
    public float wiltingPoint { get; set; }
    public float fieldArea { get; set; }
    public int inactivityTimeoutMinutes { get; set; }
    public float rainPerTick { get; set; }
    public float cropCoefficientInitial { get; set; }
    public float cropCoefficientMid { get; set; }
    public float cropCoefficientEnd { get; set; }
    public long dayStartInitialStage { get; set; }
    public long dayStartDevelopmentStage { get; set; }
    public long dayStartMidSeason { get; set; }
    public long dayEndMidSeason { get; set; }
    public long dayEndLateSeason { get; set; }
    public float fcErrorMargin { get; set; }
    public float etcErrorMargin { get; set; }
    public int transmissionDuration_m { get; set; }
    public float tempLowLimit { get; set; }
    public float tempHighLimit { get; set; }
    public float humidityLowLimit { get; set; }
    public float humidityHighLimit { get; set; }
    public float soilMoistureLowLimit { get; set; }
    public float soilMoistureHighLimit { get; set; }
    public float soilTemperatureLowLimit { get; set; }
    public float soilTemperatureHighLimit { get; set; }
    public string nameOfTemperatureKey { get; set; } = string.Empty;
    public string nameOfHumidityKey { get; set; } = string.Empty;
    public string nameOfRainTicksKey { get; set; } = string.Empty;
    public Fieldconfiguration fieldConfiguration { get; set; }
}

public class Fieldconfiguration
{
    public Groupsarray[] groupsArray { get; set; }
}

public class Groupsarray
{
    public Crop1 Crop1 { get; set; }
}

public class Crop1
{
    public string species { get; set; }
    public string irrigation { get; set; }
    public SensorArray[] sensorArray { get; set; }
}

public class SensorArray
{
    public string name { get; set; }
    public string meas { get; set; }
    public string tempKey { get; set; }
    public string soil_thickness { get; set; }
    public string weight { get; set; }
}

#pragma warning restore IDE1006 // Naming Styles