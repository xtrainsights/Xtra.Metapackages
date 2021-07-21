using System.Collections.Generic;
using System.Linq;

using Serilog.Core;
using Serilog.Events;


namespace Xtra.Metapackages.Serilog
{

    public class LookupDestructuringPolicy<TKey, TValue> : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (value is IEnumerable<IGrouping<TKey, TValue>> lookup) {
                var properties = new List<LogEventProperty>();
                foreach (var grouping in lookup) {
                    var values = new SequenceValue(
                        grouping
                            .AsEnumerable()
                            .Select(x => propertyValueFactory.CreatePropertyValue(x, true))
                    );

                    properties.Add(new LogEventProperty(grouping.Key.ToString(), values));
                }

                result = new StructureValue(properties);
                return true;
            }

            result = null;
            return false;
        }
    }

}
