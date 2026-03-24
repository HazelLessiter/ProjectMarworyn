using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class Initialiser
    {
        public IFileManager _fileManager;
        public INameProcessor _nameProcessor;
        public IGenerationManager _generationManager;
        public IConsoleService _consoleService;
        public ISeedGenerator _seedGenerator;
        public IHeartbeat _heartbeat;
        public IPersonGenerator _personGenerator;

        public Initialiser(IFileManager fileManager,
            INameProcessor nameProcessor,
            IGenerationManager generationManager,
            IConsoleService consoleService,
            ISeedGenerator seedGenerator,
            IHeartbeat heartbeat,
            IPersonGenerator personGenerator)
        {
            _fileManager = fileManager;
            _nameProcessor = nameProcessor;
            _generationManager = generationManager;
            _consoleService = consoleService;
            _seedGenerator = seedGenerator;
            _heartbeat = heartbeat;
            _personGenerator = personGenerator;
        }

        public void Start()
        {
            var names = _fileManager.ReadNameFile();
            var people = _personGenerator.Initialise(names);
            var currentGeneration = _generationManager.Initialise(names);
            var worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator
                .GetThreeWords());

            _heartbeat.Start();

            var random = new Random();
            var pairs = new List<Pair>();
            var exit = false;
            var currentGenNum = 0;

            while (!exit)
            {
                _consoleService.Delay();
                _heartbeat.Tick();
                var currentTime = _heartbeat.GetCurrentTime();

                // Generation tracking
                if (people.Count(p => p.IsAlive) < 2)
                {
                    exit = true;
                    _consoleService.WriteLine("The population has gone extinct. Less than 2 people remain");
                    break;
                }

                if (currentTime.Year % 20 == 0 && currentTime.DayOfYear == 1)
                {
                    currentGenNum++;
                    _consoleService.WriteLine($"Generation milestone reached: {currentGenNum}");
                }

                // Age increment
                foreach (var person in people.Where(p => p.IsAlive))
                {
                    var previousYear = person.TimeLived.Year;
                    person.TimeLived = person.TimeLived.AddDays(1);

                    if (person.TimeLived.Year > previousYear)
                    {
                        person.Age++;
                    }

                    if (person.TimeFromLastChild < 2)
                    {
                        person.TimeFromLastChild++;
                    }
                }

                // Death processing
                foreach (var person in people.Where(p => p.IsAlive))
                {
                    var deathChance = person.Age switch
                    {
                        >= 0 and <= 9 => 0.20,
                        >= 10 and <= 19 => 0.01,
                        >= 20 and <= 29 => 0.05,
                        >= 30 and <= 39 => 0.10,
                        >= 40 and <= 49 => 0.20,
                        >= 50 and <= 59 => 0.30,
                        >= 60 and <= 69 => 0.40,
                        >= 70 and <= 79 => 0.50,
                        >= 80 and <= 89 => 1.00,
                        >= 90 and <= 99 => 2.00,
                        _ => 5.00
                    };

                    if (random.NextDouble() * 100 < deathChance)
                    {
                        person.IsAlive = false;
                        _consoleService.WriteLine($"{person.Name.FullName} has died at age {person.Age}");
                    }
                }

                // Pairing
                var unpairedFemales = people
                    .Where(p => p.IsAlive && p.Age >= 18 && p.Name.Gender == Gender.Female)
                    .Where(p => !pairs.Any(pair => pair.Person1 == p || pair.Person2 == p))
                    .ToList();

                var unpairedMales = people
                    .Where(p => p.IsAlive && p.Age >= 18 && p.Name.Gender == Gender.Male)
                    .Where(p => !pairs.Any(pair => pair.Person1 == p || pair.Person2 == p))
                    .ToList();

                foreach (var female in unpairedFemales)
                {
                    if (unpairedMales.Count == 0) break;

                    var male = unpairedMales[random.Next(unpairedMales.Count)];
                    unpairedMales.Remove(male);

                    var newPair = new Pair { Person1 = female, Person2 = male };
                    pairs.Add(newPair);
                    _consoleService.WriteLine($"{female.Name.FullName} and {male.Name.FullName} are a pair");
                }

                // Generate Children
                foreach (var pair in pairs.Where(p => p.Person1.IsAlive && p.Person2.IsAlive))
                {
                    var person1 = pair.Person1;
                    var person2 = pair.Person2;

                    if (person1.Age >= 18 && person1.Age <= 45 && person2.Age >= 18 &&
                        person1.WillHaveChildren && person2.WillHaveChildren &&
                        person1.TimeFromLastChild >= 2 && person2.TimeFromLastChild >= 2)
                    {
                        if (random.NextDouble() * 100 < 0.25)
                        {
                            var childGender = random.Next(0, 2) == 0 ? Gender.Male : Gender.Female;
                            var femaleName = person1.Name.Gender == Gender.Female ? person1.Name : person2.Name;
                            var maleName = person1.Name.Gender == Gender.Male ? person1.Name : person2.Name;

                            var childName = childGender == Gender.Female ?
                                new Name
                                {
                                    FullName = maleName.Prefix + femaleName.Suffix,
                                    Prefix = maleName.Prefix,
                                    Suffix = femaleName.Suffix,
                                    Gender = Gender.Female
                                }
                                : new Name
                                {
                                    FullName = femaleName.Prefix + maleName.Suffix,
                                    Prefix = femaleName.Prefix,
                                    Suffix = maleName.Suffix,
                                    Gender = Gender.Male
                                };

                            var willHaveChildren = random.Next(1, 101) > 14;

                            var child = new Person
                            {
                                Id = people.Count,
                                Name = childName,
                                Age = 0,
                                WillHaveChildren = willHaveChildren,
                                IsAlive = true,
                                TimeLived = new DateTime(1, 1, 1),
                                TimeFromLastChild = 0
                            };

                            people.Add(child);
                            currentGeneration.Names.Add(childName);
                            person1.TimeFromLastChild = 0;
                            person2.TimeFromLastChild = 0;

                            _consoleService.WriteLine($"{person1.Name.FullName} and {person2.Name.FullName} have had a child named {child.Name.FullName}");
                        }
                    }
                }
            }

            _consoleService.Delay();
        }
    }
}