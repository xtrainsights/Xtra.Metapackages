using System;

using Serilog.Configuration;

using Xtra.Metapackages.Serilog.Internal;


// ReSharper disable CheckNamespace
namespace Serilog;

public static class ExtensionsForLoggerEnrichmentConfiguration
{
    public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrichmentConfig)
        => enrichmentConfig == null
            ? throw new ArgumentNullException(nameof(enrichmentConfig))
            : enrichmentConfig.With<ApplicationNameEnricher>();
}