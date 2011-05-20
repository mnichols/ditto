using System;

namespace Ditto.Tests
{

    public class TypicalViewModel
    {
        public string Name { get; set; }
        public Guid SomeId { get; set; }
    }

    public class TypicalEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ViewModelComponent
    {
        public string Name { get; set; }
    }

    public class EventComponent
    {
        public string Name { get; set; }
    }

    public class ComplexViewModel
    {
        public ViewModelComponent Component { get; set; }
        public string Name { get; set; }
    }

    public class ComplexEventWithDifferentNamedComponent
    {
        public EventComponent DifferentName { get; set; }
        public string Name { get; set; }
    }
    public class ComplexEvent
    {
        public EventComponent Component { get; set; }
        public string Name { get; set; }
    }
    public class FlattenedComponentEvent
    {
        public string DifferentName { get; set; }
    }
}