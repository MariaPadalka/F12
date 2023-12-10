namespace ThinkTwice_Context
{
    using Serilog;

    public class LoggerManager
    {
        private static LoggerManager? instance;

        private LoggerManager()
        {
            var logFileName = "../../../../log.txt";

            //if (!File.Exists(logFileName))
            //{
            //    File.Create(logFileName);
            //}

            this.Logger = new LoggerConfiguration()
                            .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
        }

        ~LoggerManager()
        {
            Log.CloseAndFlush();
        }

        public static LoggerManager Instance => instance ??= new LoggerManager();

        public ILogger Logger { get; }
    }
}
