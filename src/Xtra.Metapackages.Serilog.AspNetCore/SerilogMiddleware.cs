using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

using Serilog;
using Serilog.Events;

using GlobalLog = Serilog.Log;


namespace Xtra.Metapackages.Serilog.AspNetCore;

public class SerilogMiddleware
{
    public SerilogMiddleware(RequestDelegate next)
        => _next = next ?? throw new ArgumentNullException(nameof(next));


    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext == null) {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var start = Stopwatch.GetTimestamp();
        try {
            await _next(httpContext);
            var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

            var statusCode = httpContext.Response?.StatusCode;

            var level = statusCode >= 500
                ? LogEventLevel.Error
                : LogEventLevel.Verbose;

            var log = level == LogEventLevel.Error
                ? LogForErrorContext(httpContext)
                : Log;

            log.Write(level, MessageTemplate, httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs);
        }
        //Never caught, because `LogException()` returns false.
        catch (Exception ex) when (LogException(httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex)) { }
    }


    private static bool LogException(HttpContext httpContext, double elapsedMs, Exception ex)
    {
        LogForErrorContext(httpContext)
            .Error(ex, MessageTemplate, httpContext.Request.Method, GetPath(httpContext), 500, elapsedMs);

        return false;
    }


    private static ILogger LogForErrorContext(HttpContext httpContext)
    {
        var request = httpContext.Request;

        var result = Log
            .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), true)
            .ForContext("RequestHost", request.Host)
            .ForContext("RequestProtocol", request.Protocol);

        if (request.HasFormContentType) {
            result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));
        }

        return result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GetElapsedMilliseconds(long start, long stop)
        => (stop - start) * 1000 / (double)Stopwatch.Frequency;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetPath(HttpContext httpContext)
        => httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();


    private readonly RequestDelegate _next;

    private const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    private static readonly ILogger Log = GlobalLog.ForContext<SerilogMiddleware>();
}