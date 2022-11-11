using Serilog;

namespace _3proFieldConfigurator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region Setup logger with Serilog
            // First we ensure that the logs folder is created.
            //if (!Directory.Exists(Path.GetDirectoryName(SampleApp.Utilities.ApplicationHelper.LogsBaseFolder)))
            //{
            //    // Uncomment the line to enable the creation of the directory.
            //    //Directory.CreateDirectory(Path.GetDirectoryName(baseLogFile)!);
            //}

            // Then we construct the filename which we will use for the log files, without extension.
            //string logFileWithoutExtension = Path.Combine(SampleApp.Utilities.ApplicationHelper.LogsBaseFolder, "Log");

            // We append the appropriate extensions for the log files we want to use.
            //string logFile = $"{logFileWithoutExtension}.txt";
            //string jsonLogFile = $"{logFileWithoutExtension}.json";

            // Configure and create the Logger.
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        //.WriteTo.File(
                        //    logFile,
                        //    buffered: true,
                        //    flushToDiskInterval: TimeSpan.FromSeconds(5),
                        //    outputTemplate: "{Timestamp:yyyy-MM-dd,HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        //    rollingInterval: RollingInterval.Day)
                        //.WriteTo.File(
                        //    new CompactJsonFormatter(),
                        //    jsonLogFile,
                        //    buffered: true,
                        //    flushToDiskInterval: TimeSpan.FromSeconds(5),
                        //    rollingInterval: RollingInterval.Day)
                        .CreateLogger();
            #endregion
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FieldConfigurator());
        }
    }
}