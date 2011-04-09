using Ditto.Internal;

namespace Ditto.Resolvers
{
    internal class NestingConfigurationResolver: IResolveValue,IBindable,IValidatable
    {
        private readonly ICreateExecutableMapping executor;

        internal NestingConfigurationResolver(ICreateExecutableMapping executor)
        {
            this.executor = executor;
        }

        public Result TryResolve(IResolutionContext context, IDescribeMappableProperty destinationProperty)
        {
            //we need to keep the SOURCE the same, but change the destination
            var nested = context.Nested(destinationProperty);
            var executable = executor.CreateExecutableMapping(nested.SourceType);
            executable.Execute(nested);
            return new Result(true, nested.Destination);
        }
        
        public void Bind(params ICreateExecutableMapping[] configurations)
        {
            var bindable = executor as IBindable;
            if (bindable != null)
                bindable.Bind(configurations);
        }

        public MissingProperties Validate()
        {
            var validatable = executor as IValidatable;
            if(validatable ==null)
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