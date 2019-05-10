using System;

using Microsoft.Extensions.DependencyModel;

using Serilog;
using Serilog.Configuration;
using Serilog.Enrichers.ApplicationName;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog
{

    internal class DefaultLoggerSettings : ILoggerSettings
    {
        public DefaultLoggerSettings(bool forAzureAppService, bool includeSeqSink, Func<LogEvent, bool> filter, DependencyContext dependencyContext)
        {
            _forAzureAppService = forAzureAppService;
            _includeSeqSink = includeSeqSink;
            _filter = filter;
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
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Filter.ByExcluding(_filter ?? LoggingFilter.Default)
                .WriteTo.Console(LogEventLevel.Information, "{Message:lj}{NewLine}{Exception}");

            if (_forAzureAppService) {
                loggerConfiguration
                    .WriteTo.File(
                        @"D:\Home\LogFiles\Application\log.txt",
                        shared: true,
                        rollingInterval: RollingInterval.Hour,
                        rollOnFileSizeLimit: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1)
                    );
            }

            if (_includeSeqSink) {
                loggerConfiguration
                    .WriteTo.Seq("http://localhost:5341", compact: true);
            }
        }


        private readonly bool _forAzureAppService;
        private readonly bool _includeSeqSink;
        private readonly Func<LogEvent, bool> _filter;
    }

}
