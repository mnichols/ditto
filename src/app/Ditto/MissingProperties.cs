using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditto.Internal;

namespace Ditto
{
    public class MissingProperties:IEnumerable<IDescribeMappableProperty>
    {
        private List<IDescribeMappableProperty> missing=new List<IDescribeMappableProperty>();

        public MissingProperties()
        {
        }

        internal void Add(IDescribeMappableProperty missing)
        {
            this.missing.Add(missing);
        }
        public IEnumerator<IDescribeMappableProperty> GetEnumerator()
        {
            return missing.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void TryThrow()
        {
            if(missing.Count==0)
                return;
            var formatted = GetFormattedMissingProperties();
            
            throw new DittoConfigurationException(formatted);
        }
        /// <summary>
        /// Be certain to replace the reference when calling this method (see Example).
        /// Merges the specified other, returning a new instance.
        /// </summary>
        /// <example>
        /// var missing = missing.Merge(otherMissing);
        /// </example>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public MissingProperties Merge(IEnumerable<IDescribeMappableProperty> other)
        {
            var merged = new MissingProperties();

            foreach (var property in this.Union(other))
            {
                merged.Add(property);
            }
            return merged;
        }
        internal string GetFormattedMissingProperties()
        {
            if (missing.Count == 0)
                return "[NO MISSING PROPERTIES]";
            var grouped = (from property in missing
                           group property by property.DeclaringType
                               into groupedProps
                               select groupedProps).ToDictionary(grp => grp.Key, grp => grp.ToList());
            var sb = new StringBuilder();
            foreach (var kvp in grouped)
            {
                sb.AppendFormat("The following properties are not mapped for '{0}':", kvp.Key.FullName).AppendLine();
                foreach (var property in kvp.Value)
                {
                    sb.Append(property.Name).AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}