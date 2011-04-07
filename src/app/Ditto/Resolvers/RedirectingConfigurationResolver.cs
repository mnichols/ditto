using Ditto.Internal;

namespace Ditto.Resolvers
{
    public class RedirectingConfigurationResolver : IResolveValue,IBindable,IValidatable,IRedirected
    {
        private readonly ICreateExecutableMapping executor;
        private readonly IDescribeMappableProperty sourceProperty;

        public RedirectingConfigurationResolver(IDescribeMappableProperty sourceProperty, ICreateExecutableMapping executor)
        {
            this.sourceProperty = sourceProperty;
            this.executor = executor;
        }

        public IDescribeMappableProperty SourceProperty
        {
            get { return sourceProperty; }
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            var nested = context.Nested(destinationProperty,SourceProperty);
            var executable = executor.CreateExecutableMapping(nested.SourceType);
            executable.Execute(nested);
            return new Result(true, nested.Destination);
        }

        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var bindable = executor as IBindable;
            if(bindable!=null)
                bindable.Bind(configurations);
        }

        public MissingProperties Validate()
        {
            var validatable = executor as IValidatable;
            if (validatable == null)
                return new MissingProperties();
            return validatable.Validate();
        }

        public void Assert()
        {
            var validatable = executor as IValidatable;
            if (validatable == null)
                return;
            validatable.Assert();
        }
    }
}