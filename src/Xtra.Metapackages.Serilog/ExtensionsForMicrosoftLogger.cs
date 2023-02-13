using System;
using System.Collections.Generic;
using System.Linq;


// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Logging;

public static class ExtensionsForMicrosoftLogger
{
    /// <summary>Begins a logical operation scope. Log entries will be enriched with the provided properties.</summary>
    /// <param name="key">The property name to enrich the scope's log entries with. Prepend the key name with an "@" if you need its value de-structured.</param>
    /// <param name="value">The property value to enrich the scope's log entries with.</param>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public static IDisposable BeginScope(this ILogger logger, string key, object value)
        => logger.BeginScope(new Dictionary<string, object> { { key, value } });


    /// <summary>Begins a logical operation scope. Log entries will be enriched with the provided properties.</summary>
    /// <param name="properties">A collection of key-value-pair tuples to enrich the scope's log entries with. Prepend any property's key name with an "@" if you need its value de-structured.</param>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public static IDisposable BeginScope(this ILogger logger, params ValueTuple<string, object>[] properties)
        => logger.BeginScope(properties.ToDictionary(p => p.Item1, p => p.Item2));
}