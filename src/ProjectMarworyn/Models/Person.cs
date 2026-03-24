namespace ProjectMarworyn.Models
{
    internal class Person
    {
        public int Id { get; set; }
        public Name Name { get; set; }
        public int Age { get; set; }
        public bool WillHaveChildren { get; set; }
        public bool IsAlive { get; set; }
        public DateTime TimeLived { get; set; }
        public int TimeFromLastChild { get; set; }
    }
}