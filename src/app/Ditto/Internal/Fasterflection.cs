using System;
using System.Collections.Generic;
using System.Threading;
using Fasterflect;

namespace Ditto.Internal
{
    public class Fasterflection : IInvoke, ICacheInvocation,IActivate
    {
        private Dictionary<CacheKey, MemberSetter> setters = new Dictionary<CacheKey, MemberSetter>();
        private Dictionary<CacheKey, MemberGetter> getters = new Dictionary<CacheKey, MemberGetter>();
        private readonly ReaderWriterLockSlim locker=new ReaderWriterLockSlim();
        public Fasterflection()
        {
            Logger = new NullLogFactory();
        }

        public ILogFactory Logger { get; set; }

        public GetValue CacheGet(Type targetType, string propertyName)
        {
            return new GetValue(TryCacheGet(targetType, propertyName));
        }
        private bool IsCacheable(Type type)
        {
            return type.IsClass;
        }
        public SetValue CacheSet(Type targetType, string propertyName)
        {
            return new SetValue(TryCacheSet(targetType, propertyName));
        }

        public object GetValue(string propertyName, object source)
        {
            if (source == null)
                return null;
            var getter = TryCacheGet(source.GetType(), propertyName);
            return getter(source);
        }

        public void SetValue(AssignableValue assignableValue, object destination)
        {
            Logger.Create(this).Debug("Setting value on '{0}' using assignable value:{1}{2}", destination,Environment.NewLine, assignableValue);
            
            MemberSetter setter = TryCacheSet(destination.GetType(), assignableValue.PropertyName);

            try
            {
                setter(destination, assignableValue.DestinationValue);
            }
            catch (Exception ex)
            {
                throw new DittoExecutionException(ex, "Failure while mapping '{0}'{1}{2}", assignableValue,
                                                    Environment.NewLine, ex);
            }
        }

        public object Copy(object source)
        {
            return source.DeepClone();
        }

        private MemberGetter TryCacheGet(Type targetType, string propertyName)
        {
            var key = new CacheKey(targetType, propertyName);
            MemberGetter getter;
            if(IsCacheable(targetType))
            {
                //there should only be few writers to this bit at execution time
                // note that this could be locked inside the MemberGetter delegate (below) 
                // todo: cache all these during initialization
                locker.EnterWriteLock();
                if (getters.TryGetValue(key, out getter) == false)
                {
                    getters[key] = getter = targetType.DelegateForGetPropertyValue(propertyName, Flags.InstanceAnyVisibility);
                }
                locker.ExitWriteLock();
                return getter;
            }

            if(getters.TryGetValue(key,out getter)==false)
            {
                getters[key] = getter = new MemberGetter(impl =>
                {
                    var implementationGetter = TryCacheGet(impl.GetType(), propertyName);
                    return implementationGetter(impl);
                });    
            }
            return getter;
        }
        
        private MemberSetter TryCacheSet(Type targetType, string propertyName)
        {
            var key = new CacheKey(targetType, propertyName);

            MemberSetter setter;
            if (setters.TryGetValue(key, out setter) == false)
            {
                setters[key] =
                    setter = targetType.DelegateForSetPropertyValue(propertyName, Flags.InstanceAnyVisibility);
            }
            return setter;
        }

        public object CreateInstance(Type ofType)
        {
            return ofType.CreateInstance(Flags.InstanceAnyVisibility);
        }

        public object CreateCollectionInstance(Type collectionType, int length)
        {
            return collectionType.CreateInstance(length);
        }

        public bool HasCachedGetterFor(Type type, string name)
        {
            var key = new CacheKey(type, name);
            return getters.ContainsKey(key);
        }
    }
}