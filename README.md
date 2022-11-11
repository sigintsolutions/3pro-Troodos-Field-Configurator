# 3 Pro Introduction. 

![](img/3pro_Logo.png)

The Troodos Mountains are the green heart of Cyprus. The mountains capture and manage the rains that form our natural water resources. Agricultural dry-stone terraces, rural communities and forests shape the diverse landscape of Troodos. The agricultural and food processing sectors in the Troodos Mountains have high potentials but also face many challenges.

The 3PRO-TROODOS Project is an Integrated Research Project (2019-2022), funded by the Research and Innovation Foundation of Cyprus, and coordinated by The Cyprus Institute. The project aims to improve agricultural production and food processing in the Troodos Mountains of Cyprus, through social innovation, sustainable natural resource management and climate change adaptation.

The 3PRO-TROODOS Consortium conducts research, in cooperation with the agri-food producers and processors of the Troodos Mountains, to achieve:

- A quality certification label for Troodos food products
- Improved fruit tree production with protective nets
- New products and practices for local herbs
- Maintenance and rehabilitation of agricultural dry-stone terraces
- Improved irrigation water use with wireless sensors and mobile Apps
- Guidelines for sustainable water use under current and future climate conditions
- Insights in the socioeconomic impact of quality labelling on Troodos

# General Information. 

To perform the necessary research a number of wireless sensors had to be used to take various measurements of the state of the soil as well as the environmental conditions in an area. To keep all these data easily accessible, a sensor observation IoT platform had to be developed. 

On the Sensor Platform, device entities of type "3Pro Device" can be added. This device entity will record the sensor data that is transmitted from the device, and will post process the data accordingly to give some predictions. But for the data to be post processed successfully some specific parameters about a device must be set correctly. 

For more information about the platform and well as installation instructions refer to the relevant repository. 

[Main planform repo](https://github.com/sigintsolutions/3ProTroodosV2)

[Platform Configuration](https://github.com/sigintsolutions/3ProTroodosV2-Introduction)


# 3 pro Field Configurator

3 Pro field configurator has been developed to make the initial configuration of a device easy to a user / agent / researcher. 


A new device needs the following parameters. 

## Description of server attributes. 
* `dischargeRate`: Discharge of the irrigation supply system (one or more drippers or sprinklers) over the specified field area in L/h
* `elevation`: The elevation of the of the location of the device. Right now it is not used in any calculation. 
* `enableBatteryAlarm`: Boolean values that controls if the creation of alarms for low battery on the device will be active. 
* `batteryLimit`: The value that will act as a limit to trigger the `batteryAlarm`
* `enableInactivityAlarm`: Boolean value that control the creation of alarm for device inactivity. If `true` inactivity alarm will be triggered when the device doesn`t send any information in a time period larger than `inactivityTimeoutMinutes`
* `fieldCapacity`: Volumetric soil water content after 24-48 hour drainage of saturated soil (cm3_water/cm3_soil), expressed as percentage, e.g., 24
* `fieldName`: name of the field
* `latitude`: The latitude of the device location in decimal degrees(ddg)
* `longitude`: The longitude of the device location in decimal degrees(ddg)
* `soilMoistureThreshold`: The threshold that will be used to display warning message on the soil moisture. 
* `wettedArea`: The area wetted by the irrigation system (m2). This area should be smaller than the field area, unless the whole field is flooded. 
* `wiltingPoint`: ASK ADRIANA
* `fieldArea`: This is the field area (m2) used for the irrigation system discharge. It could be the area of the full field, of a terrace, or of a single tree. 
* `inactivityTimeoutMinutes`: is used with conjuction with `enableInactivityAlarm` to trigger an alarm. 
* `rainPerTick`: in case a device has water tipping bucket, this is used to calculate the actual value of the rain. 
* `cropCoefficientInitial`: Crop coefficient at the start of green up (trees) or between planting and 10% field cover (field crops). The crop coefficients are used for checking the irrigation water needs. 
* `cropCoefficientMid`: Crop coefficient for the full maturity stage, starting from near full canopy cover till the aging of the leafs (drying, yellowing).  
* `cropCoefficientEnd`: Crop coefficient at the end of the growing season when transpiration stops, such as leaf drop (trees) or harvest (field crops).
* `dayStartInitialStage`: For field crops from planting to 10% field cover, can be skipped for trees, so date is same as start development stage
* `dayStartDevelopmentStage`: For field crops at 10% cover, for trees at leaf out
* `dayStartMidSeason`:  Start of effective full cover (70-80%) or heading/flowering for field crops
* `dayEndMidSeason`: Start of crop maturity or leaf drying/coloring
* `dayEndLateSeason`: For field crops at harvest, for trees start of leaf drop, end of irrigation season
* `transmissionDuration_m`: the logger trasmits telemetry in a predetermined period. This value is used to determine that sensors have transmitted at that predetermined period. This is not related to the inactivity alarm, because the logger maybe active, with some of the critical sensors being faulty. 
* `nameOfTemperatureKey`: the name of the telemetry key that is used to calculate the min, max, and mean temperatures. This is used because there is a possibility that the logger has more than one temperature sensors, so a way is needed to differentiate the temperature telemetry kies. 
* `nameOfHumidityKey`. Similarly but for the humidity. 
* `tempLowLimit`: A temperature threshold. Temperature values lower than that threshold will trigger an alarm to the user. Indicating that the sensor may be faulty (for example if the sensor indicates -50 °C), or, depending on the settings, some extreme conditions that the owner of the logger wants to monitor (for example if the limit was set at -0°C) the soil will be damaged by frost. 
* `tempHighLimit`: A high temperature threshold. Similar to the above description. 
* `humidityLowLimit`: Similarly but for low  humidity threshold
* `humidityHighLimit`: Similarly but for high humidity threshold
* `soilMoistureLowLimit`: Similarly but for low soil moisture threshold
* `soilMoistureHighLimit`: similarly but for high soil moisture threshold
* `soilTemperatureLowLimit`: similarly but for low soil temperature threshold. 
* `soilTemperatureHighLimit`: similarly but for high soil temperature threshold
* `fcErrorMargin`: 
* `etcErrorMargin`:
* `fieldConfiguration` field configuration is a JSON structure that holds information about which telemetry keys are used for the calculation of the soil moisture properties. This is used because for the calculation, each sensor needs additional information that needs to be configured and to be used on the calculation, like the sensor depth. that way the sensor names can be used as variables. 


## Instructions for application use

The initial screen of the program consists of the login screen. 

![](Documentation/images/2022-10-31-14-18-35.png)

The user can input the url in the first field `(1)` in the format `https://server.com:port_number`

The user must also select the user type by checkbox `(2)`. For more information about the user types please refer to the main project repository. 

![](Documentation/images/2022-10-31-14-23-44.png)

after successful login on the second tab `(1)` a list of the available devices for modification `(2)` will appear. 

![](Documentation/images/2022-10-31-14-53-58.png)

Additionally on the same tab there are fields for all the settings that were described above and needs to be modified. 


The current settings can be loaded via the `file` menu. Similarly the currently displayed settings can be saved. 


![](Documentation/images/image.png.png)

A sample for a valid file can be found in this repository. 

Additionally a valid file follows the following format. 

```json
{
  "dischargeRate": 1920,
  "elevation": 850,
  "enableBatteryAlarm": true,
  "batteryLimit": 20.0,
  "enableInactivityAlarm": true,
  "fieldCapacity": 28.0,
  "fieldName": "Dymes_Plum",
  "latitude": 34.000000,
  "longitude": 32.000000,
  "soilMoistureThreshold": 23.0,
  "wettedArea": 19,
  "wiltingPoint": 18.0,
  "fieldArea": 28.0,
  "inactivityTimeoutMinutes": 120,
  "rainPerTick": 0.2,
  "cropCoefficientInitial": 0.35,
  "cropCoefficientMid": 0.85,
  "cropCoefficientEnd": 0.5,
  "dayStartInitialStage": 1648798140000,
  "dayStartDevelopmentStage": 1648798140000,
  "dayStartMidSeason": 1654068540000,
  "dayEndMidSeason": 1664609340000,
  "dayEndLateSeason": 1667291340000,
  "fcErrorMargin": 2.0,
  "etcErrorMargin": 0.2,
  "transmissionDuration_m": 70,
  "tempLowLimit": -20.0,
  "tempHighLimit": 50.0,
  "humidityLowLimit": 0.5,
  "humidityHighLimit": 100.0,
  "soilMoistureLowLimit": 0.5,
  "soilMoistureHighLimit": 70.0,
  "soilTemperatureLowLimit": -5.0,
  "soilTemperatureHighLimit": 45.0,
  "nameOfTemperatureKey": "Ambient Temperature",
  "nameOfHumidityKey": "Relative Humidity",
  "nameOfRainTicksKey": "Pulse 0 Ticks",
  "fieldConfiguration": {
    "groupsArray": [
      {
        "Crop1": {
          "species": "Whateverium",
          "irrigation": "100%",
          "sensorArray": [
            {
              "name": "",
              "meas": "Soil Vol Water Content 01",
              "tempKey": "Soil Temperature 01",
              "soil_thickness": "0.225",
              "weight": "0.5"
            },
            {
              "name": "",
              "meas": "Soil Vol Water Content 02",
              "tempKey": "Soil Temperature 02",
              "soil_thickness": "0.20",
              "weight": "0.5"
            },
            {
              "name": "",
              "meas": "Soil Vol Water Content 03",
              "tempKey": "Soil Temperature 03",
              "soil_thickness": "0.175",
              "weight": "0.5"
            },
            {
              "name": "",
              "meas": "Soil Vol Water Content 04",
              "tempKey": "Soil Temperature 04",
              "soil_thickness": "0.225",
              "weight": "0.5"
            },
            {
              "name": "",
              "meas": "Soil Vol Water Content 05",
              "tempKey": "Soil Temperature 05",
              "soil_thickness": "0.20",
              "weight": "0.5"
            },
            {
              "name": "",
              "meas": "Soil Vol Water Content 06",
              "tempKey": "Soil Temperature 06",
              "soil_thickness": "0.175",
              "weight": "0.5"
            }
          ]
        }
      }
    ]
  }
}
```

After loading the settings file, all the relevant fields will be updated with the loaded values. 

![](Documentation/images/2022-10-31-15-03-38.png)

Now the settings can be transmitted to the relevant devices by selecting the devices from the device list `(1)` and pressing the button `(2)`

![](Documentation/images/2022-10-31-15-05-42.png)

Upon pressing of the transmit button, a warning will be displayed

![](Documentation/images/2022-10-31-15-06-11.png)

Care must be taken upon the device selection because any transmission will overwrite any settings on that device with the same attribute name

Before Transmission:

![](Documentation/images/2022-10-31-15-12-23.png)

After Transmission. 

![](Documentation/images/2022-10-31-15-12-58.png)


# Running the application. 

Unzip the latest release and run the executable. This application requires net core runtime to be installed. 

https://dotnet.microsoft.com/en-us/download 

