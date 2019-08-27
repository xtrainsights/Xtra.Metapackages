using System;
using System.Collections.Generic;
using System.Linq;


// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Logging
{

    public static class ExtensionsForMicrosoftLogger
    {
        public static IDisposable BeginScope(this ILogger logger, string key, object value)
            => logger.BeginScope(new Dictionary<string, object> { { key, value } });


        public static IDisposable BeginScope(this ILogger logger, params ValueTuple<string, object>[] properties)
            => logger.BeginScope(properties.ToDictionary(p => p.Item1, p => p.Item2));
    }

}
