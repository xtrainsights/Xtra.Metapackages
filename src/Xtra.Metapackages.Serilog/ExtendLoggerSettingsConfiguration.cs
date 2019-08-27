using Serilog.Configuration;

using Xtra.Metapackages.Serilog;


// ReSharper disable CheckNamespace
namespace Serilog
{

    public static class ExtendLoggerSettingsConfiguration
    {
        public static LoggerConfiguration XtraDefaults(this LoggerSettingsConfiguration settingConfiguration,
            XtraLogSettings settings = null)
            => settingConfiguration.Settings(new XtraLoggerSettings(settings ?? new XtraLogSettings()));
    }

}
