## {''} - a small framework for mapping objects to objects (aka 'Object-Object Mapper') in .NET.

### Why?
Ditto was conceived to address a specific need for aggregating multiple source objects into a single instance. This is common for event handlers which all contribute their data to a single view model (ie, an event Denormalizer).

### Goals (in no particular order):
* Convention-based assumptions to minimize configuration as much as possible
* Aggregation of source objects into single destination
* Deep validation checking
* Garrulous
* Fluent configuration without typedsturbating generic parameters
* Fast enough
* IoC, container-based

### Assumptions
Ditto assumes the objects you are mapping have the same property names. This simple configuration assumes the two objects have the same names:
    cfg.Map<MyViewModel>().From<MyEvent>();
	

### Eye Oh See	
Ditto leans heavily on an IoC container to do its work. This lets us easily register resolvers and other bits for easy configuration. Castle Windsor integration is the only container integration provided right now, but Ditto doesn't really care how services are getting resolved.	

### Logging
Ditto likes to talk alot for Debug builds, so be sure you use a Release build (the default if you use the provided psake script). log4net is the only logging implementation provider right now (baked into Castle Windsor Integration), but it'd be easy to swap that out since Ditto doesn't care who is listening.

### Getting started
To let ditto know what objects you need to map and their respective sources for data, you need to get the singleton implementation of IContainerDestinationConfiguration. The AbstractMappingConfiguration base class can be inherited from to save a few keystrokes. This assumes you can let your hair down about required services being provided without constructor injection...
    public class MyViewModelConfiguration : AbstractMappingConfiguration 
	{
	    public override void Configure()
		{
		    //'Cfg' is injected by your container
		    Cfg.Map<MyViewModel>().From<MyEvent>();
		}
	}


### Examples
    //TODO : check tests for now