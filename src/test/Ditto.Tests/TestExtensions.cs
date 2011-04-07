using System;
using System.Linq;
using Ditto.Internal;

namespace Ditto.Tests
{
    public static class TestExtensions
    {
        public static IExecuteMapping ToExecutable(this ICreateBindableConfiguration cfg,Type sourceType,params ICreateBindableConfiguration[] otherCfgs)
        {
            var bindable= cfg.CreateBindableConfiguration();
            return bindable.CreateExecutableMapping(sourceType);    
        }
        public static BindingDestinationConfigurationContainer ToBindable(this DestinationConfigurationContainer cfg)
        {
            return new BindingDestinationConfigurationContainer(new BinderFactory(new Fasterflection()),new TestContextualizer(),cfg);
        }
    }
}