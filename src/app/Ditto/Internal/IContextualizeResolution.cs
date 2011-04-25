using System;

namespace Ditto.Internal
{
    public interface IContextualizeResolution
    {
        IResolutionContext CreateContext(object source, object destination);
        IResolutionContext CreateContext(object source, Type destinationType);
        IResolutionContext CreateContext(IDescribeMappableProperty sourceProperty);
    }
}