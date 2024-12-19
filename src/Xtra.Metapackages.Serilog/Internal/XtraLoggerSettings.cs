using System.Linq;

using Destructurama;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Configuration;
using Serilog.Events;

using Xtra.Models.Settings;


namespace Xtra.Metapackages.Serilog.Internal;

internal class XtraLoggerSettings(XtraLogSettings settings) : ILoggerSettings
{
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
            .Destructure.UsingAttributes()
            .Destructure.JsonNetTypes()
            .Destructure.With<LookupDestructuringPolicy<string, string>>()
            .Destructure.With<LookupDestructuringPolicy>();

        if (settings.Filters != null && settings.Filters.Any()) {
            loggerConfiguration.Filter.With(settings.Filters);
        }

        if (settings.UseConsoleSink) {
            loggerConfiguration.WriteTo.Console(
                LogEventLevel.Information,
                "{Message:lj}{NewLine}{Exception}"
            );
        }

        if (settings.UseAzureAppSink) {
            loggerConfiguration.WriteTo.AzureApp();
        }

        var seqSettings = settings.Seq ?? settings.Configuration?.GetSection("Seq").Get<SeqSettings>();
        if (seqSettings?.Enabled == true) {
            loggerConfiguration.WriteTo.Seq(seqSettings.ServerUrl ?? "http://localhost:5341", apiKey: seqSettings.ApiKey);
        }

        if (settings.Configuration != null) {
            loggerConfiguration.ReadFrom.Configuration(settings.Configuration);
        }
    }
}