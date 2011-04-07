## {''} - a small framework for mapping objects to objects (aka 'Object-Object Mapper') in .NET.

### Why?
_ditto_ was conceived to address a specific need for aggregating multiple source objects into a single instance. This is common for event handlers which all contribute their data to a single view model (ie, an event Denormalizer).

### Goals (in no particular order):
* Convention-based assumptions to minimize configuration as much as possible
* Aggregation of source objects into single destination
* Deep validation checking
* Garrulous
* Fluent configuration without typedsturbating generic parameters
* Fast enough
* IoC, container-based

### Assumptions
_ditto_ assumes the objects you are mapping have the same property names. This simple configuration assumes the two objects have the same names:
    cfg.Map<MyViewModel>().From<MyEvent>();
	

### Containers
_ditto_ leans heavily on an IoC container to do its work. This lets us easily register resolvers and other bits for easy configuration. Castle Windsor integration is the only container integration provided right now, but _ditto_ doesn't really care how services are getting resolved.	

### Logging
_ditto_ likes to talk alot for Debug builds, so be sure you use a Release build (the default if you use the provided psake script). log4net is the only logging implementation provider right now (baked into Castle Windsor Integration), but it'd be easy to swap that out since _ditto_ doesn't care who is listening.

### Configuration
To let _ditto_ know what objects you need to map and their respective sources for data, you need to get the singleton implementation of IContainerDestinationConfiguration. The AbstractMappingConfiguration base class can be inherited from to save a few keystrokes. This assumes you can let your hair down about required services being provided without constructor injection...
    public class MyViewModelConfiguration : AbstractMappingConfiguration 
	{
	    public override void Configure()
		{
		    //'Cfg' is injected by your container
		    Cfg.Map<MyViewModel>().From<MyEvent>();
		}
	}

### Conventions
Conventions may be applied globally. Conventions may impact your destination objects in one of two ways:
* The resolution of a value based on the context or metadata of associated objects
* The conversion of a value for across-the-board metamorphosis; ie, conversion of all DateTime's into UTC

Global conventions are applied to every mapping _ditto_ knows about.
To configure Global conventions, you need to add them to IContainGlobalConventions. For this, you may inherit from AbstractGlobalConventionConfiguration:
    public class WebAppGlobalMappingConventions : AbstractGlobalConventionConfiguration
	{
	    private readonly MyCustomResolver myCustomResolver;

        public WebAppGlobalMappingConventions(MyCustomResolver myCustomResolver)
        {
            this.myCustomResolver = myCustomResolver;
        }

        public override void Configure()
        {
            Conventions.AddConverter(new EntityIdentifierConverter()); 
            Conventions.AddConverter(new DateTimeUtcConverter());
            Conventions.AddResolver(new PropertyNameCriterion("ConversationId"),new IgnoreResolver());
            Conventions.AddResolver(new PropertyNameCriterion("Id"),myCustomResolver);
        }
	}


### Validation
_ditto_ is uptight, so if there are properties on _destination_ objects that are not mapped after everything is bound together, it can let you know. This is done by default if you are using the provided `WindsorMappingInitializer`. This will throw an exception if there any properties for which _ditto_ has not been configured, either via global conventions or local mappings.