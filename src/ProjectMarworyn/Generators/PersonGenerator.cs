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
    }
}