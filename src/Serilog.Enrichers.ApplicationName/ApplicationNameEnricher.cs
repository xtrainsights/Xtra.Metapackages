using Microsoft.Extensions.PlatformAbstractions;

using Serilog.Core;
using Serilog.Events;


namespace Serilog.Enrichers.ApplicationName
{

    internal class ApplicationNameEnricher : ILogEventEnricher
    {

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_applicationName == null) {
                var env = PlatformServices.Default.Application;
                _applicationName = propertyFactory.CreateProperty(nameof(env.ApplicationName), env.ApplicationName);
            }

            logEvent.AddPropertyIfAbsent(_applicationName);
        }


        private LogEventProperty _applicationName;

    }

}
