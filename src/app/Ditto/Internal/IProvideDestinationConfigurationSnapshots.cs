using System.Collections.Generic;

namespace Ditto.Internal
{
    public interface IProvideDestinationConfigurationSnapshots
    {
        ICollection<DestinationConfigurationMemento> TakeSnapshots();
    }
}