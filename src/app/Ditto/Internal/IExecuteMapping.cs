using System;

namespace Ditto.Internal
{
    public interface IExecuteMapping
    {
        Type DestinationType { get; }
        void Execute(IResolutionContext context);
    }
}