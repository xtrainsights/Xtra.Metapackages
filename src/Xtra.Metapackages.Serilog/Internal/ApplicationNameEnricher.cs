using System.Diagnostics;
using System.Reflection;

using Serilog.Core;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog.Internal
{

    internal class ApplicationNameEnricher : ILogEventEnricher
    {

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            _applicationName ??= propertyFactory.CreateProperty(
                "ApplicationName",
                ApplicationName ?? Process.GetCurrentProcess().ProcessName
            );
            logEvent.AddPropertyIfAbsent(_applicationName);
        }


        private LogEventProperty _applicationName;


        private static readonly string ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
    }

}
