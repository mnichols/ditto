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

### Examples
    //TODO : check tests for now