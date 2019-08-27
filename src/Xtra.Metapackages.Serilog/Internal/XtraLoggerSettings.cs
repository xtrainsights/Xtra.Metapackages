using Destructurama;

using Serilog;
using Serilog.Configuration;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog.Internal
{

    internal class XtraLoggerSettings : ILoggerSettings
    {
        public XtraLoggerSettings(XtraLogSettings settings)
        {
            _settings = settings;
        }


        public void Configure(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithApplicationName()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProcessName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Filter.ByExcluding(_settings.Filter ?? LogFilter.Default)
                .Destructure.UsingAttributes()
                .Destructure.JsonNetTypes();

            if (_settings.UseConsoleSink) {
                loggerConfiguration.WriteTo.Console(
                    LogEventLevel.Information,
                    "{Message:lj}{NewLine}{Exception}"
                );
            }

            if (_settings.UseAzureAppSink) {
                loggerConfiguration.WriteTo.AzureApp();
            }

            if (_settings.UseSeqSink) {
                loggerConfiguration.WriteTo.Seq("http://localhost:5341", compact: true);
            }

            if (_settings.Configuration != null) {
                loggerConfiguration.ReadFrom.Configuration(_settings.Configuration);
            }
        }


        private readonly XtraLogSettings _settings;
    }

}
