using System;
using System.Collections.Generic;
using System.Linq;

using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;


namespace Xtra.Metapackages.Serilog;

public class LogFilter : ILogEventFilter
{
    public LogFilter(params Func<LogEvent, bool>[] isEnabled)
        : this(isEnabled.AsEnumerable()) { }


    public LogFilter(IEnumerable<Func<LogEvent, bool>> areEnabled)
        => _conditions = areEnabled.ToList();


    public bool IsEnabled(LogEvent logEvent)
        => logEvent == null
            ? throw new ArgumentNullException(nameof(logEvent))
            : _conditions.All(isEnabled => isEnabled(logEvent));


    public static LogFilter operator +(LogFilter x, LogFilter y)
        => new LogFilter(x._conditions.Union(y._conditions));


    public static implicit operator LogFilter[](LogFilter f)
        => new [] { f };


    /// <summary>
    /// Excludes nothing!
    /// </summary>
    public static readonly LogFilter ExcludeNothing
        = new LogFilter(_ => true);


    /// <summary>
    /// Excludes all Airtime SQL Cache logging
    /// </summary>
    public static readonly LogFilter ExcludeSqlCaching
        = new LogFilter(x => !Matching.FromSource("Airtime.Database.Sql.Caches")(x));


    /// <summary>
    /// Excludes all NHibernate logging
    /// </summary>
    public static readonly LogFilter ExcludeNHibernate
        = new LogFilter(x => !Matching.FromSource("NHibernate")(x));


    /// <summary>
    /// Excludes all NHibernate logging except for SQL queries and statements
    /// </summary>
    public static readonly LogFilter ExcludeNHibernateNonSql
        = new LogFilter(x => !Matching.WithProperty<string>("SourceContext",
            p => {
                var source = "NHibernate".AsSpan();
                return
                    p != null
                    && p.AsSpan().StartsWith(source)
                    && (p.Length == source.Length || p[source.Length] == '.')
                    && p != "NHibernate.SQL";
            })(x));


    /// <summary>
    /// Excludes all Airtime SQL Cache logging and NHibernate logging
    /// </summary>
    public static readonly LogFilter Default
        = ExcludeNHibernate + ExcludeSqlCaching;


    /// <summary>
    /// Excludes all Airtime SQL Cache logging and NHibernate logging except for SQL queries and statements
    /// </summary>
    public static readonly LogFilter DefaultWithSql
        = ExcludeNHibernateNonSql + ExcludeSqlCaching;


    private readonly List<Func<LogEvent, bool>> _conditions;
}