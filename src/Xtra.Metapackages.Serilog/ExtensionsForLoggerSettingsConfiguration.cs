using Serilog.Configuration;

using Xtra.Metapackages.Serilog;
using Xtra.Metapackages.Serilog.Internal;


// ReSharper disable CheckNamespace
namespace Serilog;

public static class ExtensionsForLoggerSettingsConfiguration
{
    public static LoggerConfiguration XtraDefaults(this LoggerSettingsConfiguration settingConfiguration, XtraLogSettings? settings = null)
        => settingConfiguration.Settings(new XtraLoggerSettings(settings ?? new XtraLogSettings()));
}