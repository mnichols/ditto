using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Ditto.WindsorIntegration
{
    public static class WindsorMappingExtensions
    {
        public static void RegisterAllConfigurationsIn(this IWindsorContainer container,IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                container.Register(AllTypes.FromAssembly(assembly).BasedOn<IConfigureMapping>().WithService.Select(new[]{typeof(IConfigureMapping)}).Configure(c => c.LifeStyle.Transient));
            }
        }
        public static void RegisterAllGlobalConventionsIn(this IWindsorContainer container, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                container.Register(AllTypes.FromAssembly(assembly).BasedOn<IConfigureGlobalConventions>().WithService.Base().Configure(c => c.LifeStyle.Transient));       
            }
            
        }
    }
}