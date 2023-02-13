using Microsoft.Extensions.Configuration;

using Serilog.Core;

using Xtra.Models.Settings;


namespace Xtra.Metapackages.Serilog;

public class XtraLogSettings
{
    public bool UseConsoleSink { get; set; } = true;
    public bool UseAzureAppSink { get; set; } = false;
    public IConfiguration Configuration { get; set; }
    public ILogEventFilter[] Filters { get; set; } = LogFilter.Default;
    public SeqSettings Seq { get; set; }
}