using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class SimulationManager
    {
        public IFileManager _fileManager;
        public IGenerationManager _generationManager;
        public IConsoleService _consoleService;
        public ISeedGenerator _seedGenerator;
        public IHeartbeat _heartbeat;
        public IPersonGenerator _personGenerator;
        public IAgeProcessor _ageProcessor;
        public IDeathEngine _deathEngine;
        public IPairingEngine _pairingEngine;

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

            var names = _fileManager.ReadNameFile();
            var people = _personGenerator.Initialise(names,
                worldSeed);
            var currentGeneration = _generationManager.Initialise(names);
            
            var pairs = new List<Pair>();

            _heartbeat.Start();

            var exit = false;
            while(!exit)
            {
                _consoleService.Delay();
                _heartbeat.Tick();

                //Extinction
                _consoleService.WriteLine($"Current population: {people.Count}");
                if (_generationManager.CheckForExtinction(people))
                {
                    _consoleService.WriteLine("The population has gone extinct. Less than 2 people remain");
                    _heartbeat.Stop();
                    exit = true;
                    break;
                }

                //Generation
                _consoleService.WriteLine($"Current Generation: {currentGeneration.Iteration}");
                if (_heartbeat.GetCurrentTime().Year % 20 == 0)
                {
                    currentGeneration.Iteration += 1;
                    _consoleService.WriteLine($"New Generation: {currentGeneration.Iteration}");
                }

                //Age
                people = _ageProcessor.Age(people);
                

                //Death
                (people, currentGeneration) = _deathEngine.ProcessDeaths(people,
                    currentGeneration,
                    worldSeed);//TODO: Tuples are not ideal, fix

                //Pair
                (pairs, people) = _pairingEngine.GeneratePairs(people,
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