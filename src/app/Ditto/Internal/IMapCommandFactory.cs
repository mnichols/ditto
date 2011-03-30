namespace Ditto.Internal
{
    public interface IMapCommandFactory
    {
        IMapCommand Create(IExecuteMapping executableMapping);
    }
}