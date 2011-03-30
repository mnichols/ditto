using System.Collections.Generic;

namespace Ditto.Internal
{
    public class GlobalConventions : IGlobalConventionContainer,IProvideConventions
    {
        private readonly IValueConverterContainer converters;
        private List<GlobalResolvers> resolvers = new List<GlobalResolvers>();
        public GlobalConventions(IValueConverterContainer converters)
        {
            this.converters = converters;
        }

        public void AddConverter(IConvertValue converter)
        {
            converters.AddConverter(converter);
        }

        public void AddResolver(IPropertyCriterion propertyCriterion, IResolveValue resolver)
        {
            resolvers.Add(new GlobalResolvers(propertyCriterion,resolver));
        }

        public void Inject(IApplyConventions configuration)
        {
            foreach (var resolver in resolvers)
            {
                configuration.Apply(resolver.Criterion,resolver.Resolver);
            }
        }
        private class GlobalResolvers
        {
            public IPropertyCriterion Criterion { get; private set; }
            public IResolveValue Resolver { get; private set; }

            public GlobalResolvers(IPropertyCriterion criterion, IResolveValue resolver)
            {
                Criterion = criterion;
                Resolver = resolver;
            }
        }
    }
}