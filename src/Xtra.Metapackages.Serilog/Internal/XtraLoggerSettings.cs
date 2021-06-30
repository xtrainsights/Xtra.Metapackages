using Destructurama;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Xtra.Common.Abstractions.Models.Settings;


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

            var seqSettings = _settings.Seq ?? _settings.Configuration?.GetSection("Seq").Get<SeqSettings>();
            if (seqSettings?.Enabled == true) {
                loggerConfiguration.WriteTo.Seq(seqSettings.ServerUrl ?? "http://localhost:5341", apiKey: seqSettings.ApiKey);
            }

            if (_settings.Configuration != null) {
                loggerConfiguration.ReadFrom.Configuration(_settings.Configuration);
            }
        }


        private readonly XtraLogSettings _settings;
    }

}
