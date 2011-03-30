namespace Ditto.Internal
{
    public class ValueAssignments : ICreateValueAssignment
    {
        private readonly IValueConverterContainer converters;
        private readonly IReflection reflection;

        public ValueAssignments(IValueConverterContainer converters, IReflection reflection)
        {
            this.converters = converters;
            this.reflection = reflection;
        }

        public IAssignValue Create(object destination, IDescribeMappableProperty destinationProperty)
        {
            return new ValueAssignment(destination, destinationProperty,converters, reflection);
        }
    }
}