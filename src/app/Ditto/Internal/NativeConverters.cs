using Ditto.Converters;

namespace Ditto.Internal
{
    public class NativeConverters:IConfigureGlobalConventions
    {
        private IContainGlobalConventions conventions;

        public NativeConverters(IContainGlobalConventions conventions)
        {
            this.conventions = conventions;
        }

        public void Configure()
        {
            conventions.AddConverter(new SystemConverter());
            conventions.AddConverter(new NullableToNonNullableConverter());
            conventions.AddConverter(new NonNullableToNullableConverter());
        }
    }
}