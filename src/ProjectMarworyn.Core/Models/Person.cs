using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core.Models
{
    public class Person
    {
        public int Id { get; set; }
        public Name Name { get; set; }
        public int Age { get; set; }
        public Biosex Biosex { get; set; }
        public Gender Gender { get; set; }
        public Orientation Orientation { get; set; }
        public bool WillHaveChildren { get; set; }
        public bool WillPair { get; set; }
        public bool IsFertile { get; set; }
        public bool IsAlive { get; set; }
        public int BirthMonth { get; set; }
        public int BirthDay { get; set; }
        public int DaysSinceLastChild { get; set; }
        public bool HasPair { get; set; }
    }
}