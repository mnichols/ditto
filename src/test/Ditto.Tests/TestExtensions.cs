using System;
using System.Linq;
using Ditto.Internal;

namespace Ditto.Tests
{
    public static class TestExtensions
    {
        public static IExecuteMapping ToExecutable(this ITakeDestinationConfigurationSnapshot cfg,Type sourceType,params ITakeDestinationConfigurationSnapshot[] otherCfgs)
        {
            var bindable = new TestConfigurationFactory().CreateBindableConfiguration(cfg.TakeSnapshot());
            return bindable.CreateExecutableMapping(sourceType);    
        }
        
        public static BindingDestinationConfigurationContainer ToBinding(this DestinationConfigurationContainer cfg)
        {
            return new BindingDestinationConfigurationContainer(new BinderFactory(new Fasterflection()),new TestContextualizer(),cfg,new TestConfigurationFactory());
        }
    }
}