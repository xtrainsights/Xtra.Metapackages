using Serilog.Core;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog;

/// <summary>
/// <see cref="T:Xtra.MetaPackages.Serilog.NullSink"/> completely ignores any events written to it. It can be useful when
/// benchmarking to ensure events we don't want to actually log are still processed by the rest of the logging pipeline.
/// </summary>
public class NullSink : ILogEventSink
{
    public void Emit(LogEvent logEvent) { }
}