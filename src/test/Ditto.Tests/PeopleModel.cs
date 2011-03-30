using System;

namespace Ditto.Tests
{

    public class Parents
    {
        public string MothersName { get; set; }
    }

    public class PersonalInfo
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public class SystemPerson
    {
        public virtual int Age { get; set; }
        public string MothersName { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public Guid SystemId { get; set; }
    }

    public class Person
    {
        public virtual int Age { get; set; }
        public DateTime Birthdate { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string Name { get; set; }
    }
}