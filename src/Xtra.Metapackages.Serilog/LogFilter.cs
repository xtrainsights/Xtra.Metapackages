using System;

using Serilog.Events;
using Serilog.Filters;


namespace Xtra.Metapackages.Serilog
{

    public static class LogFilter
    {
        /// <summary>
        /// Excludes all NHibernate logging by default
        /// </summary>
        public static readonly Func<LogEvent, bool> Default
            = Matching.WithProperty<string>(
                "SourceContext",
                p => p.StartsWith("NHibernate.") || p.StartsWith("Airtime.Database.Sql.Caches.")
            );

        /// <summary>
        /// Excludes all NHibernate logging except for SQL queries and statements
        /// </summary>
        public static readonly Func<LogEvent, bool> IncludeSql
            = Matching.WithProperty<string>(
                "SourceContext",
                p => (p.StartsWith("NHibernate.") && p != "NHibernate.SQL") || p.StartsWith("Airtime.Database.Sql.Caches.")
            );

        /// <summary>
        /// Excludes nothing!
        /// </summary>
        public static readonly Func<LogEvent, bool> IncludeAll
            = _ => false;
    }

}
