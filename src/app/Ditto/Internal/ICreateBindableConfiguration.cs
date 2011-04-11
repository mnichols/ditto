namespace Ditto.Internal
{
    public interface ICreateBindableConfiguration
    {
        BindableConfiguration CreateBindableConfiguration(DestinationConfigurationMemento snapshot);
        BindableConfiguration CreateBindableConfiguration();
    }
}