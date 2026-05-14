using ProjectMarworyn.Models;
using ProjectMarworyn.Models.Enums;
using ProjectMarworyn.Services;

namespace ProjectMarworyn.Generators
{
    internal class PersonGenerator : IPersonGenerator
    {
        private readonly IDiceGenerator _diceGenerator;
        private readonly IConsoleService _consoleService;

        public PersonGenerator(IDiceGenerator diceGenerator,
            IConsoleService consoleService)
        {
            _diceGenerator = diceGenerator;
            _consoleService = consoleService;
        }

        public List<Person> Initialise(List<InitialPerson> initialPeople,
            int worldSeed)
        {
            var random = _diceGenerator.Create(worldSeed);

            var id = 0;
            var people = new List<Person>();
            foreach (var initialPerson in initialPeople)
            {
                var age = random.Next(0,
                    80);

                var person = new Person()
                {
                    Id = id,
                    Name = new Name()
                    {
                        FullName = initialPerson.FullName,
                        Prefix = initialPerson.Prefix,
                        Suffix = initialPerson.Suffix
                    },
                    Gender = initialPerson.Gender,
                    Age = age,
                    IsAlive = true,
                    TimeLived = new DateTime(1, 1, 1)
                        .AddYears(age),
                    WillHaveChildren = CalcWillHaveChildren(random),
                    TimeFromLastChild = 2,//This is set to 2 to allow people to have children in the first iteration, lastChildThreshold could be a const instead?
                    HasPair = false
                };

                id++;

                people.Add(person);
            }

            return people;
        }

        public (List<Person>, List<Person>) GenerateChildren(List<Pair> pairs,
            int worldSeed,
            int personId,
            List<Person> people)
        {
            //For each pair
            var aliveFurtilePairs = pairs.Where(x => x.FPerson.IsAlive &&
                    x.MPerson.IsAlive &&
                    x.FPerson.Age >= 18 &&
                    x.FPerson.Age <= 45 &&
                    x.MPerson.Age >= 18 &&
                    x.FPerson.WillHaveChildren &&
                    x.MPerson.WillHaveChildren &&
                    x.FPerson.TimeFromLastChild >= 2 &&
                    x.MPerson.TimeFromLastChild >= 2)
                .ToList();
            var random = _diceGenerator.Create(worldSeed);
            var children = new List<Person>();
            List<Person> peopleToUpdate = new List<Person>();

            foreach (var pair in aliveFurtilePairs)
            {
                var childChance = random.Next(1,
                    101);

                if (childChance < 15)
                {
                    var gender = new Gender();

                    switch (random.Next(0, 2))
                    {
                        case 0:
                            gender = Gender.Female;
                            break;
                        case 1:
                            gender = Gender.Male;
                            break;
                        default:
                            throw new InvalidOperationException("Error randomising gender");
                    }

                    var name = new Name();

                    if (gender == Gender.Male)
                    {
                        name = new Name
                        {
                            FullName = pair.FPerson.Name.Prefix + pair.MPerson.Name.Suffix,
                            Prefix = pair.FPerson.Name.Prefix,
                            Suffix = pair.MPerson.Name.Suffix,
                        };
                    }
                    if (gender == Gender.Female)
                    {
                        name = new Name
                        {
                            FullName = pair.MPerson.Name.Prefix + pair.FPerson.Name.Suffix,
                            Prefix = pair.MPerson.Name.Prefix,
                            Suffix = pair.FPerson.Name.Suffix,
                        };
                    }

                    personId++;
                    var person = new Person()
                    {
                        Id = personId,
                        Age = 0,
                        IsAlive = true,
                        Gender = gender,
                        Name = name,
                        HasPair = false,
                        TimeFromLastChild = 0,
                        TimeLived = new DateTime(1, 1, 1),
                        WillHaveChildren = CalcWillHaveChildren(random)
                    };

                    children.Add(person);
                    _consoleService.WriteLine($"Child {person.Name.FullName} was born to {pair.FPerson.Name.FullName} and {pair.MPerson.Name.FullName}");
                }

                peopleToUpdate.Add(pair.FPerson);
                peopleToUpdate.Add(pair.MPerson);
                people.Remove(pair.FPerson);
                people.Remove(pair.MPerson);
            }

            foreach (var person in peopleToUpdate)
            {
                person.TimeFromLastChild = 0;
                people.Add(person);
            }

            return (children, people);
        }

        private bool CalcWillHaveChildren(Random random)
        {
            var willHaveChildrenModifier = random.Next(1, 101);
            var willHaveChildren = willHaveChildrenModifier >= 14;
            return willHaveChildren;
        }
    }
}