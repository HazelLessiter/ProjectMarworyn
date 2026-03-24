using ProjectMarworyn.Models;

namespace ProjectMarworyn.Generators
{
    internal class PersonGenerator : IPersonGenerator
    {
        public List<Person> Initialise(List<Name> names)
        {
            var random = new Random();

            var id = 0;
            var people = new List<Person>();
            foreach (var name in names)
            {
                var age = random.Next(0, 80);
                var willHaveChildrenModifier = random.Next(1, 101);
                var willHaveChildren = willHaveChildrenModifier >= 14;

                var person = new Person()
                {
                    Id = id,
                    Name = name,
                    Age = age,
                    IsAlive = true,
                    TimeLived = new DateTime(1, 1, 1)
                        .AddYears(age),
                    WillHaveChildren = willHaveChildren,
                    TimeFromLastChild = 0
                };

                id++;

                people.Add(person);
            }

            return people;
        }

        public Person GenerateChildren(Name name)
        {
            //For each pair
            //Where person1 and person2 are both alive
            //Where person1 age is 18-45 and person2 age is 18+
            //Where person1 WillHaveChildren = true and person2 WillHaveChildren = true
            //Where person1 TimeFromLastChild is 2 and person2 TimeFromLastChild is 2
            //0.25% chance of having a child per tick
            //If child is born
            //Generate child name based on parents names
            //Create new person with name, age 0, WillHaveChildren = 14% chance of false, isAlive = true, TimeLived = (1,1,1), TimeFromLastChild = 0
            //Add child to generation.Names
            //console service writeline $"{person1.Name} and {person2.Name} have had a child named {child.Name}"
        }
    }
}