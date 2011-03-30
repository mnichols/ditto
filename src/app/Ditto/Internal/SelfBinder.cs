using System;

namespace Ditto.Internal
{
    public class SelfBinder:IBinder
    {
        public void Bind(BindableConfiguration bindableConfiguration, params ICreateExecutableMapping[] configurations)
        {
            Array.ForEach(bindableConfiguration.SourceContexts,ctx=>ctx.Bind(configurations));
            Array.ForEach(bindableConfiguration.SourcedConventions,cnv=>cnv.Bind(configurations));
        }
    }
}