// ReSharper disable CheckNamespace

using System;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;

using Serilog.Configuration;
using Serilog.Events;

using Xtra.Metapackages.Serilog;


namespace Serilog
{

    public static class ExtendLoggerSettingsConfiguration
    {
        public static LoggerConfiguration XtraDefaults(this LoggerSettingsConfiguration settingConfiguration,
            bool forAzureAppService = false,
            bool includeSeqSink = false,
            Func<LogEvent, bool> filter = null,
            DependencyContext dependencyContext = null)
        {
            return settingConfiguration.Settings(
                new DefaultLoggerSettings(
                    forAzureAppService,
                    includeSeqSink,
                    filter,
                    dependencyContext ?? (Assembly.GetEntryAssembly() != null ? DependencyContext.Default : null)
                )
            );
        }
    }

}
