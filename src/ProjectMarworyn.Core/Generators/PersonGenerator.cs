using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;
using ProjectMarworyn.Core.Services;

namespace ProjectMarworyn.Core.Generators
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
            var dice = _diceGenerator.Create(worldSeed);

            var id = 0;
            var people = new List<Person>();
            foreach (var initialPerson in initialPeople)
            {
                var age = dice.Next(0,
                    80);

                var year = 1;
                var month = _diceGenerator.Next(dice,
                    1,
                    12);
                var day = _diceGenerator.Next(dice,
                    1,
                    29);
                var yearFromLastChild = _diceGenerator.Next(dice,
                    1,
                    4);

                var person = new Person()
                {
                    Id = id,
                    Name = new Name()
                    {
                        FullName = initialPerson.FullName,
                        Prefix = initialPerson.Prefix,
                        Suffix = initialPerson.Suffix
                    },
                    Biosex = initialPerson.Biosex,
                    Gender = initialPerson.Biosex
                        switch
                        {
                            Biosex.Female => Gender.Female,
                            Biosex.Male => Gender.Male,
                            _ => (Gender)RandomGender(dice)
                        },
                    Age = age,
                    IsAlive = true,
                    TimeLived = new DateTime(year,
                            month,
                            day)
                        .AddYears(age),
                    WillHaveChildren = CalcWillHaveChildren(dice),
                    TimeFromLastChild = new DateTime(yearFromLastChild,
                        month,
                        day),
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
                    x.FPerson.TimeFromLastChild.Year >= 3 &&
                    x.MPerson.TimeFromLastChild.Year >= 3)
                .ToList();
            var dice = _diceGenerator.Create(worldSeed);
            var children = new List<Person>();
            List<Person> peopleToUpdate = new List<Person>();

            foreach (var pair in aliveFurtilePairs)
            {
                var childChance = dice.Next(1,
                    101);

                if (childChance < 40)
                {
                    var biosex = RandomBiosex(dice);
                    var gender = Gender.Female;

                    if (biosex == Biosex.Intersex)
                        gender = RandomGender(dice);
                    else
                        gender = (Gender)biosex;

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
                        Biosex = biosex,
                        Gender = gender,
                        Name = name,
                        HasPair = false,
                        TimeFromLastChild = new DateTime(1, 1, 1),
                        TimeLived = new DateTime(1, 1, 1),
                        WillHaveChildren = CalcWillHaveChildren(dice)
                    };

                    children.Add(person);
                    _consoleService.WriteLine($"Child {person.Name.FullName} was born to {pair.FPerson.Name.FullName} and {pair.MPerson.Name.FullName}",
                        ConsoleColor.Green);
                }

                peopleToUpdate.Add(pair.FPerson);
                peopleToUpdate.Add(pair.MPerson);
                people.Remove(pair.FPerson);
                people.Remove(pair.MPerson);
            }

            foreach (var person in peopleToUpdate)
            {
                person.TimeFromLastChild = new DateTime(1, 1, 1);
                people.Add(person);
            }

            return (children, people);
        }

        private Gender RandomGender(Random random)
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

            return gender;
        }

        private Biosex RandomBiosex(Random dice)
        {
            var femaleChance = (int)BiosexModifier.Female / 100.0;
            var maleChance = (int)BiosexModifier.Male / 100.0;

            var diceRoll = _diceGenerator.NextDouble(dice) * 100;

            if (diceRoll <= femaleChance)
                return Biosex.Female;

            if (diceRoll <= femaleChance + maleChance)
                return Biosex.Male;

            return Biosex.Intersex;
        }

        private bool CalcWillHaveChildren(Random random)
        {
            var willHaveChildrenModifier = _diceGenerator.Next(random,
                1,
                101);
            var willHaveChildren = willHaveChildrenModifier >= 7;

            return willHaveChildren;
        }
    }
}