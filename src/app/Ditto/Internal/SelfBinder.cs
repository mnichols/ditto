using System;

namespace Ditto.Internal
{
    /// <summary>
    /// This walks the source contexts and conventions for the configuration and binds them.
    /// </summary>
    public class SelfBinder:IBinder
    {
        public void Bind(BindableConfiguration bindableConfiguration, params ICreateExecutableMapping[] configurations)
        {
            Array.ForEach(bindableConfiguration.SourceContexts,ctx=>ctx.Bind(configurations));
            Array.ForEach(bindableConfiguration.SourcedConventions,cnv=>cnv.Bind(configurations));
        }
    }
}