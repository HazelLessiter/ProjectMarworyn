using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class GenerationManager : IGenerationManager
    {
        private readonly INameProcessor _nameProcessor;
        private readonly IConsoleService _consoleService;
        private readonly IDiceGenerator _diceGenerator;

        public GenerationManager(INameProcessor nameProcessor,
            IConsoleService consoleService,
            IDiceGenerator diceGenerator)
        {
            _nameProcessor = nameProcessor;
            _consoleService = consoleService;
            _diceGenerator = diceGenerator;
        }

        public Generation Initialise(List<Name> names)
        {
            return new Generation()
            {
                Iteration = 0,
                Names = names
            };
        }

        public Generation GenerateChildren(Generation generation,
            int worldSeed)
        {
            var newGeneration = new Generation()
            {
                Iteration = generation.Iteration + 1,
                Names = new List<Name>()
            };

            if (generation?.Names == null ||
                generation.Names.Count <= 0)
            {
                return newGeneration;
            }

            var pairs = _nameProcessor.PairNames(generation.Names,
                worldSeed);
            _consoleService.WriteLine($"Found {pairs.Count} pairs");

            var random = _diceGenerator.Create(worldSeed);

            foreach (var pair in pairs)
            {
                var numberOfChildren = random.Next(0, 4);

                if (numberOfChildren == 0)
                {
                    _consoleService.WriteLine($"Pair {pair.FName.FullName} + {pair.MName.FullName} had no children");
                    _consoleService.Delay();
                }
                else
                {
                    Gender gender = new Gender();

                    for (int i = 0; i < numberOfChildren; i++)
                    {
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

                        var name = gender == Gender.Female ?
                            new Name
                            {
                                FullName = pair.MName.Prefix + pair.FName.Suffix,
                                Prefix = pair.MName.Prefix,
                                Suffix = pair.FName.Suffix,
                                Gender = Gender.Female
                            }
                            : new Name
                            {
                                FullName = pair.FName.Prefix + pair.MName.Suffix,
                                Prefix = pair.FName.Prefix,
                                Suffix = pair.MName.Suffix,
                                Gender = Gender.Male
                            };

                        newGeneration.Names.Add(name);
                        _consoleService.WriteLine($"Child {name.FullName} was born to {pair.FName.FullName} and {pair.MName.FullName}");
                        _consoleService.Delay();
                    }
                }
            }

            return newGeneration;
        }
    }
}