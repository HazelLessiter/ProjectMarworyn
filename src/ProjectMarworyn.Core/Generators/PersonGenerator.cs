using Microsoft.Extensions.Options;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Models.Enums;

namespace ProjectMarworyn.Core.Generators
{
    internal class PersonGenerator : IPersonGenerator
    {
        private readonly IDiceGenerator _diceGenerator;
        private GameState _gameState;
        private readonly AppSettings _appSettings;

        public PersonGenerator(IDiceGenerator diceGenerator,
            GameState gameState,
            IOptions<AppSettings> appSettings)
        {
            _diceGenerator = diceGenerator;
            _gameState = gameState;
            _appSettings = appSettings.Value;
        }

        public List<Person> Initialise(List<InitialPerson> initialPeople,
            int worldSeed)
        {
            var dice = _diceGenerator.Create(worldSeed);

            var id = 0;
            var people = new List<Person>();
            foreach (var initialPerson in initialPeople)
            {
                var age = dice.Next(18,
                    40);

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
                    Gender = initialPerson.Gender,
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

        public ChildGenerationResult GenerateChildren(List<Pair> pairs,
            int worldSeed,
            int personId,
            List<Person> people,
            DateTime currentTime)
        {
            var dice = _diceGenerator.Create(worldSeed,
                currentTime);

            //For each pair
            var aliveFertilePairs = pairs.Where(x => x.FPerson.IsAlive &&
                    x.MPerson.IsAlive &&
                    x.FPerson.Age >= 18 &&
                    x.FPerson.Age <= 45 &&
                    x.MPerson.Age >= 18 &&
                    x.FPerson.WillHaveChildren &&
                    x.MPerson.WillHaveChildren &&
                    x.FPerson.TimeFromLastChild.Year >= _appSettings.FertilityCooldownYears &&
                    x.MPerson.TimeFromLastChild.Year >= _appSettings.FertilityCooldownYears)
                .ToList();

            var children = new List<Person>();
            List<Person> peopleToUpdate = new List<Person>();

            foreach (var pair in aliveFertilePairs)
            {
                var childChance = dice.Next(1,
                    101);

                if (childChance < 40)
                {
                    var biosex = RandomBiosex(dice);
                    var gender = CalculateGender(dice,
                        biosex);

                    var name = CalculateName(dice,
                        gender,
                        pair);

                    personId++;
                    var child = new Person()
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

                    children.Add(child);
                    _gameState.Text.Add($"Child {child.Name.FullName} was born to {pair.FPerson.Name.FullName} and {pair.MPerson.Name.FullName}");

                    peopleToUpdate.Add(pair.FPerson);
                    peopleToUpdate.Add(pair.MPerson);

                    people.Remove(pair.FPerson);
                    people.Remove(pair.MPerson);
                }
            }

            foreach (var person in peopleToUpdate)
            {
                person.TimeFromLastChild = new DateTime(1, 1, 1);
                people.Add(person);
            }

            return new ChildGenerationResult
            {
                Children = children,
                People = people
            };
        }

        private Name CalculateName(Random dice,
            Gender gender,
            Pair pair)
        {
            var namingGender = gender;

            if (gender == Gender.NonBinary)
            {
                //Non-binary children pick the traditional route (either binary convention at random)
                //or one of two dedicated routes: prefix + prefix or suffix + suffix
                switch (_diceGenerator.Next(dice, 0, 3))
                {
                    case 0:
                        namingGender = RandomGender(dice);
                        break;
                    case 1:
                        return RandomOrderName(dice,
                            pair.FPerson.Name.Prefix,
                            pair.MPerson.Name.Prefix);
                    case 2:
                        return RandomOrderName(dice,
                            pair.FPerson.Name.Suffix,
                            pair.MPerson.Name.Suffix);
                    default:
                        throw new InvalidOperationException("Error randomising naming route");
                }
            }

            if (namingGender == Gender.Male)
            {
                return new Name
                {
                    FullName = pair.FPerson.Name.Prefix + pair.MPerson.Name.Suffix,
                    Prefix = pair.FPerson.Name.Prefix,
                    Suffix = pair.MPerson.Name.Suffix,
                };
            }

            return new Name
            {
                FullName = pair.MPerson.Name.Prefix + pair.FPerson.Name.Suffix,
                Prefix = pair.MPerson.Name.Prefix,
                Suffix = pair.FPerson.Name.Suffix,
            };
        }

        private Name RandomOrderName(Random dice,
            string fPersonPart,
            string mPersonPart)
        {
            var mPersonFirst = _diceGenerator.Next(dice, 0, 2) == 1;

            var firstPart = mPersonFirst ?
                mPersonPart :
                fPersonPart;
            var secondPart = mPersonFirst ?
                fPersonPart :
                mPersonPart;

            return new Name
            {
                FullName = firstPart + secondPart,
                Prefix = firstPart,
                Suffix = secondPart,
            };
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

        private Gender CalculateGender(Random dice,
            Biosex biosex)
        {
            //NonBinaryProbability is a slice within the TransgenderProbability umbrella,
            //so a single roll decides both: the non-binary band sits below the binary-flip band
            var diceRoll = _diceGenerator.NextDouble(dice) * 100;

            if (diceRoll < _appSettings.NonBinaryProbability)
            {
                return Gender.NonBinary;
            }

            if (biosex == Biosex.Intersex)
            {
                return RandomGender(dice);
            }

            var alignedGender = biosex == Biosex.Female ?
                Gender.Female :
                Gender.Male;

            if (diceRoll < _appSettings.TransgenderProbability)
            {
                return alignedGender == Gender.Female ?
                    Gender.Male :
                    Gender.Female;
            }

            return alignedGender;
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