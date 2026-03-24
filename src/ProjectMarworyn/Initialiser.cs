using ProjectMarworyn.Generators;
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

            while(exit = false)
            {
                //Consoleservice delay
                _heartbeat.Tick();

                //Generation
                //If people count is < 2, exit = true, console service writeline "The population has gone extinct. Less than 2 people remain"
                //Heartbeat if simulationClock year is divisible by 20, generation +1

                //Age
                //For each person where isAlive = true, age +1 day
                //If TimeLived year has incremented from before, age +1
                //If WillHaveChildren = true and timeFromLastChild < 2, TimeFromLastChild +1

                //Death
                //If person age 0-9, 0.20% chance of death per tick
                //If person age 10-19, 0.01% chance of death per tick
                //If person age 20-29, 0.05% chance of death per tick
                //If person age 30-39, 0.10% chance of death per tick
                //If person age 40-49, 0.20% chance of death per tick
                //If person age 50-59, 0.30% chance of death per tick
                //If person age 60-69, 0.40% chance of death per tick
                //If person age 70-79, 0.50% chance of death per tick
                //If person age 80-89, 1.00% chance of death per tick
                //If person age 90-99, 2.0% chance of death per tick
                //If person age 100+, 5.0% chance of death per tick
                //if death = true
                //Person isAlive = false
                //Console.WriteLine($"{person.Name} has died at age {person.Age}");

                //Pair
                //Foreach female person where age 18+ and not in pair
                //Take random male person where age 18+ and not in pair
                //Pair together, add to list of pairs
                //console service writeline $"{person1.Name} and {person2.Name} are a pair"

                //Generate Children
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
            while (currentGeneration.Names.Count > 1)
            {
                _nameProcessor.ListNumberOfNamesByGender(currentGeneration.Names);
                currentGeneration = _generationManager.GenerateChildren(currentGeneration,
                    worldSeed);
                _consoleService.WriteLine($"New Generation: {currentGeneration.Iteration}");
            }
            if (currentGeneration.Names.Count < 2)
            {
                _consoleService.WriteLine("The population has gone extinct. Less than 2 people remain");
                _consoleService.Delay();
            }
        }
    }
}