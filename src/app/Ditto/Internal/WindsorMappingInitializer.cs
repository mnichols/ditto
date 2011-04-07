using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace Ditto.Internal
{
    public class WindsorMappingInitializer:IInitializeMappingEngine
    {
        private IKernel kernel;

        public WindsorMappingInitializer(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public void Initialize()
        {
            ConfigureConventions();
            ConfigureDestinations();
            IBindConfigurations configurationContainer = null;
            try
            {
                configurationContainer = kernel.Resolve<IBindConfigurations>();
                TryBind(configurationContainer);
                TryCache(configurationContainer as ICacheable);
                Assert(configurationContainer as IValidatable);
            }
            finally
            {
                if(configurationContainer!=null)
                    kernel.ReleaseComponent(configurationContainer);
            }

        }

        private void ConfigureConventions()
        {
            var configurations = ResolveAssert<IConfigureGlobalConventions>();
            foreach (var configuration in configurations)
            {
                try
                {
                    configuration.Configure();
                }
                finally
                {
                    kernel.ReleaseComponent(configuration);
                }
            }
        }
        /// <summary>
        /// Resolves component but will throw if any of <paramref name="T"/> cannot be resolved
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<T> ResolveAssert<T>()
        {
            var handlers = kernel.GetAssignableHandlers(typeof(T));

            return handlers.Select(h => h.Resolve(CreationContext.Empty)).Cast<T>().ToArray();
        }
        private void TryBind(IBindConfigurations bindConfigurations)
        {
            if(bindConfigurations==null)
                return;
            bindConfigurations.Bind();
        }

        private void Assert(IValidatable validatable)
        {
            if (validatable == null)
                return;
            validatable.Assert();
        }

        private void TryCache(ICacheable cacheable)
        {
            if (cacheable == null)
                return;
            var cacheModels = kernel.GetAssignableHandlers(typeof (IVisitCacheable));
            foreach (var cache in cacheModels)
            {
                IVisitCacheable visitor = null;
                try
                {
                    visitor = cache.Resolve(CreationContext.Empty) as IVisitCacheable;
                    if (visitor == null)
                    {
                        throw new MappingConfigurationException("Problem with resolution of {0}",cache.ComponentModel.Implementation);
                    }
                    cacheable.Accept(visitor);
                }
                finally
                {
                    if (visitor != null)
                        kernel.ReleaseComponent(visitor);
                }


            }
        }

        private void ConfigureDestinations()
        {
            var configurations = ResolveAssert<IConfigureMapping>();
            foreach (var configuration in configurations)
            {
                try
                {
                    configuration.Configure();
                }
                finally
                {
                    kernel.ReleaseComponent(configuration);
                }
            }
        }
    }
}