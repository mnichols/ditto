using System.Collections.Generic;
using System.Reflection;

namespace Ditto.Internal
{
    public class DefaultReflection:IReflection
    {
        private static Dictionary<CacheKey,PropertyInfo> setters=new Dictionary<CacheKey, PropertyInfo>();
        private static Dictionary<CacheKey,PropertyInfo> getters=new Dictionary<CacheKey, PropertyInfo>();
        public object GetValue(string propertyName, object source)
        {
            var key = new CacheKey(source.GetType(), propertyName);
            PropertyInfo prop;
            if (getters.TryGetValue(key, out prop) == false)
            {
                getters[key] = prop = source.GetType().GetProperty(propertyName);
            }
            return prop.GetValue(source, null);
        }

        public void SetValue(AssignableValue assignableValue, object destination)
        {
            var key = new CacheKey(destination.GetType(), assignableValue.PropertyName);
            PropertyInfo prop;
            if (setters.TryGetValue(key, out prop) == false)
            {
                setters[key] = prop = destination.GetType().GetProperty(assignableValue.PropertyName);
            }
            prop.SetValue(destination, assignableValue.DestinationValue, null);
        }

        public object Copy(object source)
        {
            //really..i don't want to implement a deep cloning here
            return source;
        }

        
    }
}