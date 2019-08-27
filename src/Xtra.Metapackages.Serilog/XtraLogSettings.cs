using System;

using Microsoft.Extensions.Configuration;

using Serilog.Events;


namespace Xtra.Metapackages.Serilog
{

    public class XtraLogSettings
    {
        public bool UseConsoleSink { get; set; } = true;
        public bool UseAzureAppSink { get; set; } = false;
        public bool UseSeqSink { get; set; } = false;
        public IConfiguration Configuration { get; set; }
        public Func<LogEvent, bool> Filter { get; set; } = LogFilter.Default;
    }

}
