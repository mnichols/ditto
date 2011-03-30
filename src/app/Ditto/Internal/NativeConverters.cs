using Ditto.Converters;

namespace Ditto.Internal
{
    public class NativeConverters:IConfigureGlobalConventions
    {
        private IGlobalConventionContainer conventions;

        public NativeConverters(IGlobalConventionContainer conventions)
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