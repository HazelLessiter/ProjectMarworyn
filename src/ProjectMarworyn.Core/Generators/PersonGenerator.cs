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

            //Fail at startup rather than mid-run: a hand-edited weight table with a missing,
            //duplicated or misweighted entry would silently skew every birth
            if (_appSettings.OrientationWeights == null ||
                Enum.GetValues<Orientation>().Any(x => _appSettings.OrientationWeights.Count(w => w.Orientation == x) != 1) ||
                Math.Abs(_appSettings.OrientationWeights.Sum(x => x.Weight) - 100) > 0.001)
            {
                throw new InvalidOperationException("OrientationWeights must contain exactly one entry per Orientation value, with weights summing to 100");
            }
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

                var birthMonth = _diceGenerator.Next(dice,
                    1,
                    13);
                //Capped at 28 so the birthday is valid in every month
                var birthDay = _diceGenerator.Next(dice,
                    1,
                    29);
                var yearsFromLastChild = _diceGenerator.Next(dice,
                    0,
                    3);
                //Day-of-year offset staggers cooldowns so they don't all expire on the same day
                var dayOffset = new DateTime(1,
                        birthMonth,
                        birthDay)
                    .DayOfYear - 1;

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
                    Orientation = initialPerson.Orientation,
                    Age = age,
                    IsAlive = true,
                    BirthMonth = birthMonth,
                    BirthDay = birthDay,
                    WillHaveChildren = CalcWillHaveChildren(dice),
                    WillPair = initialPerson.WillPair,
                    IsFertile = initialPerson.IsFertile,
                    DaysSinceLastChild = yearsFromLastChild * SimulationConstants.DaysPerYear + dayOffset,
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

            var children = new List<Person>();
            List<Person> peopleToUpdate = new List<Person>();

            foreach (var pair in pairs)
            {
                //Reproduction is strictly biological (issue #15 stage 4): one partner must
                //supply the egg side and one the sperm side, so same-sex pairs don't conceive.
                //Adoption (later) will redistribute children orphaned in the simulation instead -
                //people never appear from nowhere
                //Asexual partners pair romantically, but a child here means sexual reproduction,
                //so they wait for the adoption system too
                if (pair.PersonA.Orientation == Orientation.Asexual ||
                    pair.PersonB.Orientation == Orientation.Asexual)
                {
                    continue;
                }

                if (!pair.PersonA.IsFertile ||
                    !pair.PersonB.IsFertile)
                {
                    continue;
                }

                var mother = SelectParent(pair,
                    Biosex.Female);
                var father = SelectParent(pair,
                    Biosex.Male,
                    mother);

                if (mother == null ||
                    father == null)
                {
                    continue;
                }

                if (!mother.IsAlive ||
                    !father.IsAlive ||
                    mother.Age < 18 ||
                    mother.Age > 45 ||
                    father.Age < 18 ||
                    !mother.WillHaveChildren ||
                    !father.WillHaveChildren ||
                    mother.DaysSinceLastChild < _appSettings.FertilityCooldownYears * SimulationConstants.DaysPerYear ||
                    father.DaysSinceLastChild < _appSettings.FertilityCooldownYears * SimulationConstants.DaysPerYear)
                {
                    continue;
                }

                var childChance = dice.Next(1,
                    101);

                if (childChance < 40)
                {
                    var biosex = RandomBiosex(dice);
                    var gender = CalculateGender(dice,
                        biosex);

                    var name = CalculateName(dice,
                        gender,
                        mother,
                        father);

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
                        DaysSinceLastChild = 0,
                        BirthMonth = currentTime.Month,
                        BirthDay = currentTime.Day,
                        WillHaveChildren = CalcWillHaveChildren(dice),
                        Orientation = CalculateOrientation(dice),
                        WillPair = CalcWillPair(dice),
                        IsFertile = CalcIsFertile(dice,
                            biosex)
                    };

                    children.Add(child);
                    _gameState.Text.Add($"Child {child.Name.FullName} was born to {mother.Name.FullName} and {father.Name.FullName}");

                    peopleToUpdate.Add(mother);
                    peopleToUpdate.Add(father);

                    people.Remove(mother);
                    people.Remove(father);
                }
            }

            foreach (var person in peopleToUpdate)
            {
                person.DaysSinceLastChild = 0;
                people.Add(person);
            }

            return new ChildGenerationResult
            {
                Children = children,
                People = people
            };
        }

        //The pair's biosex roles never change mid-simulation, so the mother/father naming
        //conventions survive the Pair model losing its FPerson/MPerson shape
        private Name CalculateName(Random dice,
            Gender gender,
            Person mother,
            Person father)
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
                            mother.Name.Prefix,
                            father.Name.Prefix);
                    case 2:
                        return RandomOrderName(dice,
                            mother.Name.Suffix,
                            father.Name.Suffix);
                    default:
                        throw new InvalidOperationException("Error randomising naming route");
                }
            }

            if (namingGender == Gender.Male)
            {
                return new Name
                {
                    FullName = mother.Name.Prefix + father.Name.Suffix,
                    Prefix = mother.Name.Prefix,
                    Suffix = father.Name.Suffix,
                };
            }

            return new Name
            {
                FullName = father.Name.Prefix + mother.Name.Suffix,
                Prefix = father.Name.Prefix,
                Suffix = mother.Name.Suffix,
            };
        }

        private Name RandomOrderName(Random dice,
            string motherPart,
            string fatherPart)
        {
            var fatherFirst = _diceGenerator.Next(dice, 0, 2) == 1;

            var firstPart = fatherFirst ?
                fatherPart :
                motherPart;
            var secondPart = fatherFirst ?
                motherPart :
                fatherPart;

            return new Name
            {
                FullName = firstPart + secondPart,
                Prefix = firstPart,
                Suffix = secondPart,
            };
        }

        private static Person SelectParent(Pair pair,
            Biosex role,
            Person exclude = null)
        {
            //The exact biosex match takes the role first, leaving a flexible intersex
            //partner free to cover the other side
            if (pair.PersonA != exclude &&
                pair.PersonA.Biosex == role)
            {
                return pair.PersonA;
            }

            if (pair.PersonB != exclude &&
                pair.PersonB.Biosex == role)
            {
                return pair.PersonB;
            }

            if (pair.PersonA != exclude &&
                CanFillRole(pair.PersonA,
                    role))
            {
                return pair.PersonA;
            }

            if (pair.PersonB != exclude &&
                CanFillRole(pair.PersonB,
                    role))
            {
                return pair.PersonB;
            }

            return null;
        }

        //A fertile intersex person reproduces in the direction of their gender -
        //female-gendered supplies the egg side, male-gendered the sperm side, and
        //non-binary leaves both directions open
        private static bool CanFillRole(Person person,
            Biosex role)
        {
            //Unreachable via SelectParent (its exact-biosex branches run first), but kept
            //so the method answers truthfully if ever called on its own
            if (person.Biosex == role)
            {
                return true;
            }

            if (person.Biosex != Biosex.Intersex)
            {
                return false;
            }

            var roleAlignedGender = role == Biosex.Female ?
                Gender.Female :
                Gender.Male;

            return person.Gender == roleAlignedGender ||
                person.Gender == Gender.NonBinary;
        }

        private bool CalcIsFertile(Random dice,
            Biosex biosex)
        {
            if (biosex != Biosex.Intersex)
            {
                return true;
            }

            var fertileRoll = _diceGenerator.NextDouble(dice) * 100;

            return fertileRoll < _appSettings.IntersexFertileProbability;
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
            //NonBinaryProbability and TransgenderProbability are independent rolls,
            //with the non-binary roll taking precedence
            var nonBinaryRoll = _diceGenerator.NextDouble(dice) * 100;

            if (nonBinaryRoll < _appSettings.NonBinaryProbability)
            {
                return Gender.NonBinary;
            }

            //Intersex children have no biosex-aligned gender, so they are assigned a random
            //binary one, then roll for trans like everyone else
            var alignedGender = biosex
                switch
                {
                    Biosex.Female => Gender.Female,
                    Biosex.Male => Gender.Male,
                    _ => RandomGender(dice)
                };

            var transgenderRoll = _diceGenerator.NextDouble(dice) * 100;

            if (transgenderRoll < _appSettings.TransgenderProbability)
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

        private Orientation CalculateOrientation(Random dice)
        {
            var roll = _diceGenerator.NextDouble(dice) * 100;

            var cumulative = 0.0;
            foreach (var orientationWeight in _appSettings.OrientationWeights)
            {
                cumulative += orientationWeight.Weight;

                if (roll < cumulative)
                {
                    return orientationWeight.Orientation;
                }
            }

            //Floating point rounding can leave the cumulative total fractionally short of 100
            return _appSettings.OrientationWeights.Last().Orientation;
        }

        //Independent of orientation: a proportion of the population never pairs at all,
        //whoever they are attracted to
        private bool CalcWillPair(Random dice)
        {
            var neverPairRoll = _diceGenerator.NextDouble(dice) * 100;

            return neverPairRoll >= _appSettings.NeverPairProbability;
        }
    }
}