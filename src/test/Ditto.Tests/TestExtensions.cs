using System;
using System.Linq;
using Ditto.Internal;

namespace Ditto.Tests
{
    public static class TestExtensions
    {
        public static IExecuteMapping ToExecutable(this ITakeDestinationConfigurationSnapshot cfg,Type sourceType,params ITakeDestinationConfigurationSnapshot[] otherCfgs)
        {
            var bindable = new TestDestinationConfigurationFactory().CreateBindableConfiguration(cfg.ToSnapshot());
            return bindable.CreateExecutableMapping(sourceType);    
        }
        public static BindingDestinationConfigurationContainer ToBindable(this DestinationConfigurationContainer cfg)
        {
            return new BindingDestinationConfigurationContainer(new BinderFactory(new Fasterflection()),new TestContextualizer(),cfg);
        }
    }
}