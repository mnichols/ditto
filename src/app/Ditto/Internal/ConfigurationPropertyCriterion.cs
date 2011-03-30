namespace Ditto.Internal
{
    public class ConfigurationPropertyCriterion:IPropertyCriterion
    {
        private readonly ICreateExecutableMapping anotherConfiguration;

        public ConfigurationPropertyCriterion(ICreateExecutableMapping anotherConfiguration)
        {
            this.anotherConfiguration = anotherConfiguration;
        }

        public bool IsSatisfiedBy(IDescribeMappableProperty property)
        {
            return anotherConfiguration.DestinationType == property.PropertyType;
        }
    }
}