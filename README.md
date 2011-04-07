## ditto {''} - a small framework for mapping objects to objects in .NET.

### Why?
There are good mapping tools out there already, but _ditto_ was conceived to address a specific need for aggregating multiple source objects into a single instance. This is common for event handlers which all contribute their data to a single view model (ie, an event Denormalizer). From this simple model, it became real easy to extend its usage in other scenarios like mapping from a Form model in a web app to Commands in MVC controller code. Also, because it wires everything up using a IoC container, it is dead simple write resolvers, converters, or replace its behavior just by registering your implementations on your container.

### Goals (in no particular order):

* Convention-based assumptions to minimize configuration as much as possible
* Aggregation of source objects into single destination
* Deep validation checking
* Garrulous
* Fluent configuration without typedsturbating generic parameters
* Fast _enough_
* IoC, container-based

### Limitations
* Only properties are evaluated by convention right now
* Flattening/Unflattening isn't a concern for me, but it'd be easy to create components to do this without changing Ditto

### Assumptions
_ditto_ assumes the objects you are mapping have the same property names. This simple configuration assumes the two objects have the same names:

    cfg.Map<MyViewModel>().From<MyEvent>();

### Containers
_ditto_ leans heavily on an IoC container to do its work. This lets us easily register resolvers and other bits for easy configuration. Castle Windsor integration is the only container integration provided right now, but _ditto_ doesn't really care how services are getting resolved.	If you decide to use the Castle Windsor integration you can just add the provided Facility (`DittoFacility`). Here's the code installing all mapping services for my view model denormalization daemon:

    container.AddFacility("mapping.facility", new DittoFacility());
	container.RegisterAllConfigurationsIn(new[] { GetType().Assembly });
	container.RegisterAllGlobalConventionsIn(new[]{GetType().Assembly});
	container.Register(AllTypes.FromThisAssembly()
						   .BasedOn<IResolveValue>()
						   .WithService.AllInterfaces()
						   .Configure(c => c.LifeStyle.Transient));
						   
### Logging
_ditto_ likes to talk alot for Debug builds, so be sure you use a Release build (the default if you use the provided psake script). log4net is the only logging implementation provider right now (baked into Castle Windsor Integration), but it'd be easy to swap that out since _ditto_ doesn't care who is listening.

### Configuration
To let _ditto_ know what objects you need to map and their respective sources for data, you need to get the singleton implementation of IContainerDestinationConfiguration. The `AbstractMappingConfiguration` base class can be inherited from to save a few keystrokes. This assumes you can let your hair down about required services being provided without constructor injection.

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
To configure Global conventions, you need to add them to IContainGlobalConventions. For this, you may inherit from `AbstractGlobalConventionConfiguration`:

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
_ditto_ is uptight, so if there are properties on _destination_ objects that are not mapped after everything is bound together, it can let you know. This is done by default if you are using the provided `WindsorMappingInitializer`. This will throw an exception if there are any properties for which _ditto_ has not been configured, either via global conventions or local mappings. 

### Intialization
_ditto_ can be initialized in your bootstrapping code by simply getting your instance of `IInitializeDitto` and calling its Initialize method. Again, there is a `WindsorMappingInitializer` that demonstrates the things that _ditto_ needs to do for starting up. These are done, in order:

1. Configure all global conventions
2. Configure all destination objects from their sources
3. Bind all these configurations together
4. Cache configuration metadata (optional, but madness to do without)
5. Validate/Assert all configuration

### Mapping
Once you have initialized _ditto_ you may now just grab your instance of `IMap` and call it by either having _ditto_ create the destination object for you, or passing in your own instance:

    var myViewModel = mapper.Map<MyViewModel>(mySourceEvent); // => this creates an instance of MyViewModel and maps from your instance of the source
	mapper.Map(mySourceEvent,myViewModel); // => this simply maps from the source to your own instance of MyViewModel
	
Please note that _ditto_ currently assumes a default constructor for its own activation of objects. This would be dead simple to improve upon though since it is container-driven. Just replace `IActivator` and provide your own strategy(s).

### Acknowledgements
* [Fasterflect](http://fasterflect.codeplex.com/) - _ditto_ developer is lazy so relies on  for reflection optimization stuff. It's a great little tool for the toolbox.
* [Castle Windsor](https://github.com/castleproject/Castle.Windsor) - Makes it easy to keep concerns split up. 

