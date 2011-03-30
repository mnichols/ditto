using System;
using Ditto.Internal;
using Ditto.Resolvers;
using Xunit;

namespace Ditto.Tests
{
    public class value_resolver_tests
    {
        private TestContextualizer contextualizer;

        public value_resolver_tests()
        {
            contextualizer = new TestContextualizer();
        }
        [Fact]
        public void it_should_resolve_by_property_name_of_source()
        {
            var resolver = new PropertyNameResolver("Name");
            var src = new TestClass() {Name = "mikey"};
            var result = resolver.TryResolve(contextualizer.CreateContext(src, typeof(object)));
            result.IsResolved.should_be_true();
            result.Value.should_be_equal_to("mikey");
        }
        [Fact]
        public void it_should_resolve_with_a_static_value()
        {
            var resolver = new StaticValueResolver("monkey");
            var src = new TestClass() { Name = "mikey" };
            var result = resolver.TryResolve(contextualizer.CreateContext(src, typeof(object)));
            result.IsResolved.should_be_true();
            result.Value.should_be_equal_to("monkey");
        }
        private void UseImmutableResolver(string name,LotsOTypes destination,ImmutableDestinationResolver resolver,Action<Result> assert)
        {
            var source = new LotsOTypes()
            {
                AGuid = Guid.NewGuid(),
                AInt = 5,
                ANguid = Guid.NewGuid(),
                ANint = 9,
                AReferenceType = new TestClass(){Name = "monster"},
                AString = "x3"
            };
            
            var context = contextualizer.CreateContext(source, destination);
            var destProp= new MappableProperty(typeof (LotsOTypes).GetProperty(name));
            var result = resolver.ResolveBasedOn(context,destProp);
            assert(result);

        }
        [Fact]
        public void immutable_resolver_should_not_resolve_when_destination_has_value()
        {
            Action<Result> isUnresolved = r => r.IsResolved.should_be_false();
            UseImmutableResolver("AGuid", new LotsOTypes(){AGuid = Guid.NewGuid()}, new ImmutableDestinationResolver(new ThrowingResolver()),isUnresolved);
            UseImmutableResolver("ANguid", new LotsOTypes() { ANguid = Guid.NewGuid() }, new ImmutableDestinationResolver(new ThrowingResolver()), isUnresolved);
            UseImmutableResolver("AString", new LotsOTypes { AString = "s1" }, new ImmutableDestinationResolver(new ThrowingResolver()), isUnresolved);
            UseImmutableResolver("AInt", new LotsOTypes { AInt = 1 }, new ImmutableDestinationResolver(new ThrowingResolver()), isUnresolved);
            UseImmutableResolver("ANint", new LotsOTypes { ANint = 3 }, new ImmutableDestinationResolver(new ThrowingResolver()), isUnresolved);
            UseImmutableResolver("AReferenceType", new LotsOTypes { AReferenceType = new TestClass() { Name = "mikey" } }, new ImmutableDestinationResolver(new ThrowingResolver()), isUnresolved);
        }
        private class ThrowingResolver : IResolveValue
        {
            public Result TryResolve(IResolutionContext context)
            {
                throw new InvalidOperationException("Should not resolve");
            }
        }
        [Fact]
        public void immutable_resolver_should_resolve_when_destination_is_default_value()
        {
            Action<Result> isResolved = r => r.IsResolved.should_be_true();
            UseImmutableResolver("AGuid", new LotsOTypes() { AGuid = Guid.Empty }, new ImmutableDestinationResolver(new PropertyNameResolver("AGuid")), isResolved);
            UseImmutableResolver("ANguid", new LotsOTypes() { ANguid = null }, new ImmutableDestinationResolver(new PropertyNameResolver("ANguid")), isResolved);
            UseImmutableResolver("AString", new LotsOTypes { AString = "" }, new ImmutableDestinationResolver(new PropertyNameResolver("AString")), isResolved);
            UseImmutableResolver("AInt", new LotsOTypes { AInt = 0 }, new ImmutableDestinationResolver(new PropertyNameResolver("AInt")), isResolved);
            UseImmutableResolver("ANint", new LotsOTypes { ANint = null }, new ImmutableDestinationResolver(new PropertyNameResolver("ANint")), isResolved);
            UseImmutableResolver("AReferenceType", new LotsOTypes { AReferenceType = null }, new ImmutableDestinationResolver(new PropertyNameResolver("AReferenceType")), isResolved);
        }
        public class LotsOTypes
        {
            public string AString { get; set; }
            public Guid AGuid { get; set; }
            public int AInt { get; set; }
            public int? ANint { get; set; }
            public Guid? ANguid { get; set; }
            public TestClass AReferenceType { get; set; }
        }
        public class TestClass
        {
            public string Name { get; set; }
        }
    }
}