// ReSharper disable CheckNamespace

using System.Diagnostics;

using Microsoft.Extensions.PlatformAbstractions;

using Serilog.Core;
using Serilog.Events;


namespace Serilog.Enrichers.ApplicationName
{

    internal class ApplicationNameEnricher : ILogEventEnricher
    {

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            _applicationName = _applicationName
                ?? propertyFactory.CreateProperty(
                    nameof(ApplicationName),
                    PlatformServices.Default.Application.ApplicationName ?? Process.GetCurrentProcess().ProcessName
                );

            logEvent.AddPropertyIfAbsent(_applicationName);
        }


        private LogEventProperty _applicationName;

    }

}
