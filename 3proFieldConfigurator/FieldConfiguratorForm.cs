using _3proFieldConfigurator.Classes;
using _3proFieldConfigurator.Properties;
using RestSharp;
using RestSharp.Authenticators;
using Serilog;
using System.ComponentModel;
using System.Text.Json;
using Thingsboard.Auth;
using Thingsboard.DeviceController;
using Thingsboard.DeviceEntry;
using Thingsboard.Enums;
using Thingsboard.SimpleDeviceEntry;
using Thingsboard.TelemetryController;
using Thingsboard.Tools;
using _3proFieldConfigurator.Extensions;
using _3proFieldConfigurator.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace _3proFieldConfigurator;

public partial class FieldConfigurator : Form
{
    private static string m_BaseUrl = string.Empty;
    private static bool m_ClientInitialized = false;
    private static RestClient? m_RestClient = null;
    private static readonly List<DeviceEntry> m_DeviceListForDataGridViews = new();
    private const string m_ApiSuffix = "/api";
    private static string m_CustomerId = string.Empty;
    private List<SensorArray> m_SensorArray;

    public FieldConfigurator()
    {
        m_SensorArray = new();
        InitializeComponent();
        SetNumUpDownLimits();
        toolStripStatusLabel1.ForeColor = Color.Red;
        toolStripStatusLabel1.Text = "Not Connected";
        foreach (var dateTimePicker in Tools.Control.GetAllControlsOfSpecificType(cropCoefficientsGroupBox, typeof(DateTimePicker)))
        {
            SetDateTimePickerFormat((DateTimePicker)dateTimePicker);
        }

        InitializeSensorsDataGridView();
    }

    private void InitializeSensorsDataGridView()
    {
        DataGridView dgv = soilMoistureSensorsDataGridView;
        m_SensorArray.Add(
            new SensorArray()
            {
                name = string.Empty,
                meas = string.Empty,
                soil_thickness = string.Empty,
                weight = string.Empty,
            }
        );

        dgv.DataSource = new BindingList<SensorArray>(m_SensorArray);

        dgv.AllowUserToAddRows = true;
    }

        private void SetNumUpDownLimits()
    {
        elevationNumericUpDown.SetNumLimits(digits: 0, min: 0, max: 9000);
        dischargeRateNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 10000000);
        wettedAreaNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 10000000);
        fieldAreaNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 10000000);
        fieldCapacityNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 10000000);
        wiltingPointNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 10000000);
        soilMoistureThresholdNumericUpDown.SetNumLimits(digits: 1, min: 0, max: 100);
        measurementFrequencyNumericUpDown.SetNumLimits(digits: 0, min: 1, max: (60 * 24 * 7 * 12) + 10);
        inactivityTimeoutNumericUpDown.SetNumLimits(digits: 0, min: 1, max: (60 * 24 * 7 * 12) + 10);
        batteryThresholdNumericUpDown.SetNumLimits(digits: 0, min: 1, max: 90);
        latitudeNumericUpDown.SetNumLimits(digits: 7, min: -90, max: 90);
        longitudeNumericUpDown.SetNumLimits(digits: 7, min: -180, max: 180);
        rainPerTicknumericUpDown.SetNumLimits(digits: 2, min: 0, max: 5);
        cropCoefficientInitialNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 1);
        cropCoefficientMidNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 1);
        cropCoefficientEndNumericUpDown.SetNumLimits(digits: 2, min: 0, max: 1);
        validTempMinNumericUpDown.SetNumLimits(digits: 2, min: -100, max: 200);
        validTempMaxNumericUpDown.SetNumLimits(digits: 2, min: -100, max: 200);
        validHumidityMinNumericUpDown.SetNumLimits(digits: 2, min: -1, max: 101);
        validHumidityMaxNumericUpDown.SetNumLimits(digits: 2, min: -1, max: 101);
        validSoilTempMinNumericUpDown.SetNumLimits(digits: 2, min: -100, max: 100);
        validSoilTempMaxNumericUpDown.SetNumLimits(digits: 2, min: -100, max: 100);
        validSoilMoistureMinNumericUpDown.SetNumLimits(digits: 2, min: -1, max: 101);
        validSoilMoistureMaxNumericUpDown.SetNumLimits(digits: 2, min: -1, max: 101);
    }

    private void EnableDisableAllButtons(bool enableButtons)
    {
        loginButton.Enabled = enableButtons;
    }

    /// <summary>
    /// Make a dateTimePicker use a custom format of "dd MMMM"
    /// </summary>
    /// <param name="dtp"></param>
    private static void SetDateTimePickerFormat(DateTimePicker dtp)
    {
        dtp.Format = DateTimePickerFormat.Custom;
        dtp.CustomFormat = "dd MMMM";
        dtp.ShowUpDown = true;
    }

    /// <summary>
    /// login to thingsboard.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LoginButtonAsync_ClickAsync(object sender, EventArgs e)
    {
        EnableDisableAllButtons(false);
        m_DeviceListForDataGridViews.Clear();
        UserTypeEnum selection = tenantRadioButton.Checked ? UserTypeEnum.Tenant : UserTypeEnum.Customer;
        bool status = await LoginAndGetAllDevicesAsync(selection);
        if (!status)
        {
            toolStripStatusLabel1.ForeColor = Color.Red;
            toolStripStatusLabel1.Text = "Not Connected";
            MessageBox.Show("Failed to login", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else
        {
            toolStripStatusLabel1.ForeColor = Color.Green;
            toolStripStatusLabel1.Text = $"Connected as {selection}"; //TODO: make this variable. 
        }
        EnableDisableAllButtons(true);
    }

    private static string CreateBaseUrl(string url)
    {
        return url + m_ApiSuffix;
    }

    /// <summary>
    /// logins to thingsboard and fetches all the devices to be displayed on the data grid view.
    /// </summary>
    /// <returns>true on success, false on failure. </returns>
    private async Task<bool> LoginAndGetAllDevicesAsync(UserTypeEnum selection)
    {
        JWTRequest jwtRequest = new()
        {
            username = userNameTextbox.Text,
            password = credentialsTextbox.Text
        };

        m_BaseUrl = CreateBaseUrl(thingsboardURLtextbox.Text);

        if (!m_ClientInitialized) //initialize the client once only. 
        {
            var options = new RestClientOptions(m_BaseUrl)
            {
                ThrowOnAnyError = true,
                MaxTimeout = 2000,
            };
            m_RestClient = new RestClient(options);

            m_ClientInitialized = true;
        }

        if (m_RestClient == null)
        {
            m_ClientInitialized = false;
            return false;
        }

        //get JWT token
        JWTResponse? token = await AuthHelper.GetJWTTokenAsync(jwtRequest, m_RestClient);
        if (token == null)
        {
            return false;
        }

        //use jwt key. ie set them on the authenticator
        m_RestClient.Authenticator = new JwtAuthenticator(token.token!);


        if (selection == UserTypeEnum.Customer)
        {

            //to do any actions as a customer you need the customer id. There is not way to get the customer id without any request a link from the
            //tenant, which defeats the purpose.
            //there is a workaround to extract the customer id from the credentials because the customer id its actually encoded inside the jwt token. 
            // see also. https://github.com/thingsboard/thingsboard/issues/921
            //jwt is encoded as base64

            string parsedJWTJSON = string.Empty;

            try
            {
                parsedJWTJSON = Thingsboard.Tools.Tools.DecodeJWT(token.token!);
                Log.Information(JsonHelper.FormatJson(parsedJWTJSON));
                var parsedJWT = JsonSerializer.Deserialize<ParsedJWTFormat>(parsedJWTJSON);

                if (parsedJWT == null)
                {
                    return false;
                }
                m_CustomerId = parsedJWT.customerId!;
            }
            catch
            {
                MessageBox.Show("incorrect account type");
                return false;
            }
            
            if (parsedJWTJSON == null)
            {
                return false;
            }
        }

        //Get Device List 
        GetTenantDevicesResponse? deviceList = await DeviceController.GetTenantDevicesAsync(selection, m_CustomerId, m_RestClient);
        if ((deviceList == null) || (deviceList.data == null)) //short circuit
        {
            return false;
        }

        //add the device list on the data structure that will be used on datagridview
        foreach (var device in deviceList.data)
        {
            Log.Information("Device id " + device.id!.id);
            m_DeviceListForDataGridViews.Add(new DeviceEntry(device.id.id!, device.name!, device.type!, device.label ?? string.Empty));
        }

        //get available telemetryKeys and add them on the same data structure
        for (int i = 0; i < m_DeviceListForDataGridViews.Count; i++)
        {
            string[]? timeseriesList = await TelemetryController.GetTimeseriesKeysAsync(m_DeviceListForDataGridViews[i].DeviceId!, m_RestClient);
            if (timeseriesList == null) //failed to get meaningful data
            {
                continue;
            }

            m_DeviceListForDataGridViews[i].TelemetryKeys = new List<string>();
            m_DeviceListForDataGridViews[i].TelemetryKeys.AddRange(timeseriesList);
        }
        InitializeDataGridView(deviceListDgv);
        return true;
    }

    /// <summary>
    /// initializes the sensors datagrid view.
    /// </summary>
    /// <param name="dgv"></param>
    private static void InitializeDataGridView(DataGridView dgv)
    {
        dgv.DataSource = null;
        dgv.DataSource = new BindingList<DeviceEntry>(m_DeviceListForDataGridViews);
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.AutoGenerateColumns = true;
        dgv.Refresh();
    }

    //save updated application settings on form close. 
    private void FieldConfigurator_FormClosing(object sender, FormClosingEventArgs e)
    {
        Settings.Default.Url = this.thingsboardURLtextbox.Text;
        Settings.Default.User = this.userNameTextbox.Text;
        Settings.Default.Password = this.credentialsTextbox.Text;
        Settings.Default.Tenant = this.tenantRadioButton.Checked;
        // Save settings
        Settings.Default.Save();
    }

    /// <summary>
    /// load application settings on form load.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FieldConfigurator_Load(object sender, EventArgs e)
    {
        if (Settings.Default.Url != null)
        {
            this.thingsboardURLtextbox.Text = Settings.Default.Url;
        }

        if (Settings.Default.User != null)
        {
            this.userNameTextbox.Text = Settings.Default.User;
        }

        if (Settings.Default.Password != null)
        {
            this.credentialsTextbox.Text = Settings.Default.Password;
        }

        if (Settings.Default.Tenant != null)
        {
            this.tenantRadioButton.Checked = (bool)Settings.Default.Tenant;
            this.customerRadioButton.Checked = (bool)!Settings.Default.Tenant;
        }
    }

    /// <summary>
    /// loads selected file.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadSettingToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open Json file. 
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = Directory.GetCurrentDirectory(),
            Title = "Browse Json Files",

            CheckFileExists = true,
            CheckPathExists = true,

            Filter = "Json files (*.json)|*.json",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        // Get the path of specified file
        _ = openFileDialog.FileName;

        // Read the contents of the file into a stream
        Stream fileStream = openFileDialog.OpenFile();
        using StreamReader file = new(fileStream);
        string? fileContent = file.ReadToEnd();
        if (fileContent == null)
        {
            MessageBox.Show("invalid file");
            return;
        }

        Models.SettingsFormat? settings = JsonSerializer.Deserialize<Models.SettingsFormat>(fileContent);
        if ((settings == null) || (settings.fieldConfiguration?.groupsArray?.First().Crop1.sensorArray == null))
        {
            MessageBox.Show("invalid file");
            return;
        }
        PopulateControlsFromJson(settings);
    }

    /// <summary>
    /// Given a settings structure, reads that structure and populates the control with the values.
    /// </summary>
    /// <param name="settings"></param>
    private void PopulateControlsFromJson(Models.SettingsFormat settings)
    {
        fieldNameTextBox.Text = settings.fieldName;
        elevationNumericUpDown.Value = settings.elevation;
        dischargeRateNumericUpDown.Value = (decimal)settings.dischargeRate;
        wettedAreaNumericUpDown.Value = (decimal)settings.wettedArea;
        fieldAreaNumericUpDown.Value = (decimal)settings.fieldArea;
        fieldCapacityNumericUpDown.Value = (decimal)settings.fieldCapacity;
        wiltingPointNumericUpDown.Value = (decimal)settings.wiltingPoint;
        measurementFrequencyNumericUpDown.Value = settings.transmissionDuration_m;
        enableBatteryAlarmCheckBox.Checked = settings.enableBatteryAlarm;
        enableInactivityAlarmCheckBox.Checked = settings.enableInactivityAlarm;
        inactivityTimeoutNumericUpDown.Value = settings.inactivityTimeoutMinutes;
        batteryThresholdNumericUpDown.Value = (decimal)settings.batteryLimit;
        latitudeNumericUpDown.Value = (decimal)settings.latitude;
        longitudeNumericUpDown.Value = (decimal)settings.longitude;
        cropCoefficientInitialNumericUpDown.Value = (decimal)settings.cropCoefficientInitial;
        cropCoefficientMidNumericUpDown.Value = (decimal)settings.cropCoefficientMid;
        cropCoefficientEndNumericUpDown.Value = (decimal)settings.cropCoefficientEnd;
        initDayDateTimePicker.Value = Tools.DateTime.DateTimeFromUnixTime_Ms(settings.dayStartInitialStage);
        devDayDimePicker.Value = Tools.DateTime.DateTimeFromUnixTime_Ms(settings.dayStartDevelopmentStage);
        midDayStartDayTimePicker.Value = Tools.DateTime.DateTimeFromUnixTime_Ms(settings.dayStartMidSeason);
        midDayEndDayTimePicker.Value = Tools.DateTime.DateTimeFromUnixTime_Ms(settings.dayEndMidSeason);
        endDayEndDayTimePicker.Value = Tools.DateTime.DateTimeFromUnixTime_Ms(settings.dayEndLateSeason);
        nameOfTemperatureKeyTextBox.Text = settings.nameOfTemperatureKey;
        nameOfHumidityKeyTextBox.Text= settings.nameOfHumidityKey;
        nameOfRainTicksKeyTextBox.Text = settings.nameOfRainTicksKey;
        rainPerTicknumericUpDown.Value = (decimal)settings.rainPerTick;

        validTempMinNumericUpDown.Value = (decimal)settings.tempLowLimit;
        validTempMaxNumericUpDown.Value = (decimal)settings.tempHighLimit;
        validHumidityMinNumericUpDown.Value = (decimal)settings.humidityLowLimit;
        validHumidityMaxNumericUpDown.Value = (decimal)settings.humidityHighLimit;
        validSoilTempMinNumericUpDown.Value = (decimal)settings.soilTemperatureLowLimit;
        validSoilTempMaxNumericUpDown.Value = (decimal)settings.soilTemperatureHighLimit;
        validSoilMoistureMinNumericUpDown.Value = (decimal)settings.soilMoistureLowLimit;
        validSoilMoistureMaxNumericUpDown.Value = (decimal)settings.soilMoistureHighLimit;

        var crop = settings!.fieldConfiguration!.groupsArray!.First().Crop1;
        irrigationTextBox.Text = crop.irrigation ?? string.Empty;
        speciesTextBox.Text = crop.species ?? string.Empty;
        m_SensorArray.Clear();
        m_SensorArray.AddRange(crop!.sensorArray!.ToList());
        soilMoistureSensorsDataGridView.DataSource = new BindingList<SensorArray>(m_SensorArray);
    }

    private void SaveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Models.SettingsFormat settings = CreateSettingsFromControlls();
        string result = JsonSerializer.Serialize(settings);
        SaveFile(result, "json files (*.json)|*.json", "save settings file");
    }

    /// <summary>
    /// saves the file on the selected filename.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileType"></param>
    /// <param name="dialogText"></param>
    private static void SaveFile(string stream, string fileType, string dialogText)
    {
        Stream myStream;
        //object creation i guess
        SaveFileDialog saveFileDialog1 = new()
        {
            //file types
            Filter = fileType,
            Title = dialogText,
            //?
            FilterIndex = 1,
            //?
            RestoreDirectory = true
        };

        //if the dialog opens
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
            //?
            if ((myStream = saveFileDialog1.OpenFile()) != null)
            {
                //streamwriter object creation
                StreamWriter wText = new(myStream);
                //write string to the file
                wText.Write(stream);
                // flush and close the text 
                wText.Flush();
                wText.Close();
                myStream.Close();
            }
        }
    }

    /// <summary>
    /// reads all the controls and saves their values in the settings structure so that this can be serialized.
    /// </summary>
    /// <returns></returns>
    private Models.SettingsFormat CreateSettingsFromControlls()
    {
        Models.SettingsFormat settings = new()
        {
            fieldName = fieldNameTextBox.Text,
            elevation = (int)elevationNumericUpDown.Value,
            dischargeRate = (float)dischargeRateNumericUpDown.Value,
            wettedArea = (float)wettedAreaNumericUpDown.Value,
            fieldArea = (float)fieldAreaNumericUpDown.Value,
            fieldCapacity = (float)fieldCapacityNumericUpDown.Value,
            wiltingPoint = (float)wiltingPointNumericUpDown.Value,
            transmissionDuration_m = (int)measurementFrequencyNumericUpDown.Value,
            enableBatteryAlarm = enableBatteryAlarmCheckBox.Checked,
            enableInactivityAlarm = enableInactivityAlarmCheckBox.Checked,
            inactivityTimeoutMinutes = (int)inactivityTimeoutNumericUpDown.Value,
            batteryLimit = (float)batteryThresholdNumericUpDown.Value,
            latitude = (float)latitudeNumericUpDown.Value,
            longitude = (float)longitudeNumericUpDown.Value,
            cropCoefficientInitial = (float)cropCoefficientInitialNumericUpDown.Value,
            cropCoefficientMid = (float)cropCoefficientMidNumericUpDown.Value,
            cropCoefficientEnd = (float)cropCoefficientEndNumericUpDown.Value,
            dayStartInitialStage = Tools.DateTime.DateTimeToUnixTime_ms(initDayDateTimePicker.Value),
            dayStartDevelopmentStage = Tools.DateTime.DateTimeToUnixTime_ms(devDayDimePicker.Value),
            dayStartMidSeason = Tools.DateTime.DateTimeToUnixTime_ms(midDayStartDayTimePicker.Value),
            dayEndMidSeason = Tools.DateTime.DateTimeToUnixTime_ms(midDayEndDayTimePicker.Value),
            dayEndLateSeason = Tools.DateTime.DateTimeToUnixTime_ms(endDayEndDayTimePicker.Value),
            nameOfTemperatureKey = nameOfTemperatureKeyTextBox.Text,
            nameOfRainTicksKey = nameOfRainTicksKeyTextBox.Text,
            nameOfHumidityKey = nameOfHumidityKeyTextBox.Text,
            tempLowLimit = (float)validTempMinNumericUpDown.Value,
            tempHighLimit = (float)validTempMaxNumericUpDown.Value,
            humidityLowLimit = (float)validHumidityMinNumericUpDown.Value,
            humidityHighLimit = (float)validHumidityMaxNumericUpDown.Value
        };
        settings.humidityHighLimit = (float)validHumidityMaxNumericUpDown.Value;
        settings.soilTemperatureLowLimit = (float)validSoilTempMinNumericUpDown.Value;
        settings.soilTemperatureHighLimit = (float)validSoilTempMaxNumericUpDown.Value;
        settings.soilMoistureLowLimit = (float)validSoilMoistureMinNumericUpDown.Value;
        settings.soilMoistureHighLimit = (float)validSoilMoistureMaxNumericUpDown.Value;

        settings.rainPerTick = (float)rainPerTicknumericUpDown.Value;
        if (soilMoistureSensorsDataGridView.Rows.Count > 0)
        {
            settings.fieldConfiguration = new()
            {
                groupsArray = new Groupsarray[1],
            };
            settings.fieldConfiguration.groupsArray[0] = new()
            {
                Crop1 = new()
                {
                    irrigation = irrigationTextBox.Text,
                    species = speciesTextBox.Text,
                    sensorArray = m_SensorArray.ToArray()
                }
            };
        }
        return settings;
    }

    /// <summary>
    /// formats the data grid view when a new data source is change.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SoilMoistureSensorsDataGridView_DataSourceChanged(object sender, EventArgs e)
    {
        DataGridView dgv = (DataGridView)sender;
        if (dgv.DataSource == null)
            return;
        Tools.DataGridView.AutoResizeDataGridView(dgv);
    }

    private async void TransmitToDevicesButton_Click(object sender, EventArgs e)
    {
        DialogResult dialogResult = MessageBox.Show("Are you sure? This will override the server attributes for the selected device",
                                            "",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Exclamation);
        if (dialogResult == DialogResult.No)
        {
            return;
        }

        Models.SettingsFormat settings = CreateSettingsFromControlls();
        foreach (DataGridViewRow row in deviceListDgv.SelectedRows)
        {
            if (row.DataBoundItem is DeviceEntry device)
            {
                PropertyInfo[] properties = typeof(SettingsFormat).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (properties == null)
                {
                    continue;
                }

                foreach (var property in properties)
                {
                    if (property.CanRead)
                    {
                        device.DeviceAttributes.Add(property.Name, property.GetValue(settings, null));
                    }
                }

                Task.Delay(100).Wait();
                //use device id to POST device attributes
                await TelemetryController.PostDeviceAttributes(device, AttributesScope.SERVER_SCOPE, m_RestClient);
                Log.Information($"Posted server attributes for device {device.DeviceName}");
            }
        }
    }
}
