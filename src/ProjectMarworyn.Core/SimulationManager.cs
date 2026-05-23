using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Services;

namespace ProjectMarworyn.Core
{
    internal class SimulationManager
    {
        private readonly IFileManager _fileManager;
        private readonly IGenerationManager _generationManager;
        private readonly IConsoleService _consoleService;
        private readonly ISeedGenerator _seedGenerator;
        private readonly IHeartbeat _heartbeat;
        private readonly IPersonGenerator _personGenerator;
        private readonly IAgeProcessor _ageProcessor;
        private readonly IDeathEngine _deathEngine;
        private readonly IPairingEngine _pairingEngine;

        public SimulationManager(IFileManager fileManager,
            IGenerationManager generationManager,
            IConsoleService consoleService,
            ISeedGenerator seedGenerator,
            IHeartbeat heartbeat,
            IPersonGenerator personGenerator,
            IAgeProcessor ageProcessor,
            IDeathEngine deathEngine,
            IPairingEngine pairingEngine)
        {
            _fileManager = fileManager;
            _generationManager = generationManager;
            _consoleService = consoleService;
            _seedGenerator = seedGenerator;
            _heartbeat = heartbeat;
            _personGenerator = personGenerator;
            _ageProcessor = ageProcessor;
            _deathEngine = deathEngine;
            _pairingEngine = pairingEngine;
        }

        public void Start()
        {
            var worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator
                .GetThreeWords());

            var initialPeople = _fileManager.ReadInitialPersonFile();
            var people = _personGenerator.Initialise(initialPeople,
                worldSeed);
            var currentGeneration = _generationManager.Initialise(people);
            
            var pairs = new List<Pair>();

            _heartbeat.Start();

            var exit = false;
            while(!exit)
            {
                _consoleService.Delay();
                _heartbeat.Tick();

                //Extinction
                if (_generationManager.CheckForExtinction(people))
                {
                    _consoleService.WriteLine("The population has gone extinct. Less than 2 people remain");
                    _heartbeat.Stop();
                    exit = true;
                    break;
                }

                //Generation
                var currentTime = _heartbeat.GetCurrentTime();
                if (currentTime.Day == 01 &&
                    currentTime.Month == 01)
                {
                    _consoleService.WriteLine($"Happy new year!",
                        ConsoleColor.DarkMagenta);
                    _consoleService.WriteLine($"Number of people: {people.Count}",
                        ConsoleColor.DarkMagenta);
                    _consoleService.WriteLine($"Number of children: {people.Count(x => x.Age < 18)}",
                        ConsoleColor.DarkMagenta);

                    if (currentTime.Year % 20 == 0)
                    {
                        currentGeneration.Iteration += 1;
                        _consoleService.WriteLine($"New Generation: {currentGeneration.Iteration}",
                            ConsoleColor.DarkMagenta);
                    }
                }

                //Age
                people = _ageProcessor.Age(people);
                

                //Death
                currentGeneration = _deathEngine.ProcessDeaths(people,
                    currentGeneration,
                    worldSeed);

                //Pair
                (pairs, people) = _pairingEngine.GeneratePairs(currentGeneration.People,
                    pairs,
                    worldSeed);

                //Generate Children
                (var children, people) = _personGenerator.GenerateChildren(pairs,
                    worldSeed,
                    people.MaxBy(x => x.Id ).Id,
                    people);

                people = people.Concat(children)
                    .ToList();
            }
        }
    }
}