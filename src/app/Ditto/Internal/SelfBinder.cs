using System;

namespace Ditto.Internal
{
    /// <summary>
    /// Ensures bindable configuration is bound
    /// </summary>
    public class SelfBinder:IBinder
    {
        public void Bind(BindableConfiguration bindableConfiguration, params ICreateExecutableMapping[] configurations)
        {
            bindableConfiguration.Bind(configurations);
        }
    }
}