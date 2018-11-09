using System;

using Serilog.Configuration;


namespace Serilog.Enrichers.ApplicationName
{

    public static class ExtendLoggerEnrichmentConfiguration
    {
        public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrichmentConfig)
            => enrichmentConfig == null
                ? throw new ArgumentNullException(nameof(enrichmentConfig))
                : enrichmentConfig.With<ApplicationNameEnricher>();
    }

}
