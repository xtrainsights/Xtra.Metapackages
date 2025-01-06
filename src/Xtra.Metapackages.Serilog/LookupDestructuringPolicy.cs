using System.Collections.Concurrent;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FastMember;

using Serilog.Core;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog;

/// <summary>
/// When destructuring objects, transform <see cref="T:System.Linq.ILookup`2" /> instances into a representation where
/// the keys are not lost.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the <see cref="T:System.Linq.ILookup`2" />.</typeparam>
/// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> sequences that make up the values in the <see cref="T:System.Linq.ILookup`2" /></typeparam>
public class LookupDestructuringPolicy<TKey, TElement> : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        if (value is not IEnumerable<IGrouping<TKey, TElement>> lookup) {
            result = null!;
            return false;
        }

        var properties = new List<LogEventProperty>();
        foreach (var grouping in lookup) {
            var key = grouping.Key!.ToString();
            var array = new SequenceValue(grouping.Select(x => propertyValueFactory.CreatePropertyValue(x, true)));
            properties.Add(new LogEventProperty(key!, array));
        }

        result = new StructureValue(properties);
        return true;
    }
}


/// <summary>
/// When destructuring objects, transform <see cref="T:System.Linq.ILookup`2" /> instances into a representation where
/// the keys are not lost.<br/>
/// <see cref="T:Xtra.Metapackages.Serilog.LookupDestructuringPolicy" /> is roughly 20-30% slower than the generic
/// <see cref="T:Xtra.Metapackages.Serilog.LookupDestructuringPolicy`2" /> variant but has the advantage that it will
/// work with any <see cref="T:System.Linq.ILookup`2" /> object without needing to know the type's generic arguments
/// ahead of time.
/// </summary>
public class LookupDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result) {
        var objectType = value.GetType();
        var groupingType = TypeCache.GetOrAdd(objectType, GetGroupingType);

        if (groupingType == null) {
            result = null!;
            return false;
        }

        var accessor = TypeAccessor.Create(groupingType);
        var properties = new List<LogEventProperty>();

        foreach (var grouping in (IEnumerable)value) {
            var key = accessor[grouping, "Key"];
            var array = new SequenceValue(((IEnumerable<object>)grouping).Select(x => propertyValueFactory.CreatePropertyValue(x, true)));
            properties.Add(new LogEventProperty(key.ToString()!, array));
        }

        result = new StructureValue(properties);
        return true;
    }


    private static Type? GetGroupingType(Type t)
        => t.GetTypeInfo()
            .ImplementedInterfaces
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(i => i.GenericTypeArguments.FirstOrDefault(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IGrouping<,>)))
            .FirstOrDefault();


    private static readonly ConcurrentDictionary<Type, Type?> TypeCache = new();
}