namespace Ditto.Internal
{
    public interface ITakeDestinationConfigurationSnapshot
    {
        DestinationConfigurationMemento TakeSnapshot();
    }
}