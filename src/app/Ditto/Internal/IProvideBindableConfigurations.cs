using System.Collections.Generic;

namespace Ditto.Internal
{
    public interface IProvideBindableConfigurations
    {
        ICollection<BindableConfiguration> GetBindableConfigurations();
    }
}