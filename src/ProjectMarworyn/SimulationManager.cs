using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn
{
    internal class SimulationManager
    {
        public IFileManager _fileManager;
        public INameProcessor _nameProcessor;
        public IGenerationManager _generationManager;
        public IConsoleService _consoleService;
        public ISeedGenerator _seedGenerator;
        public IHeartbeat _heartbeat;
        public IPersonGenerator _personGenerator;
        public IAgeProcessor _ageProcessor;
        public IDeathEngine _deathEngine;

        public SimulationManager(IFileManager fileManager,
            INameProcessor nameProcessor,
            IGenerationManager generationManager,
            IConsoleService consoleService,
            ISeedGenerator seedGenerator,
            IHeartbeat heartbeat,
            IPersonGenerator personGenerator,
            IAgeProcessor ageProcessor,
            IDeathEngine deathEngine)
        {
            _fileManager = fileManager;
            _nameProcessor = nameProcessor;
            _generationManager = generationManager;
            _consoleService = consoleService;
            _seedGenerator = seedGenerator;
            _heartbeat = heartbeat;
            _personGenerator = personGenerator;
            _ageProcessor = ageProcessor;
            _deathEngine = deathEngine;
        }

        public void Start()
        {
            var names = _fileManager.ReadNameFile();
            var people = _personGenerator.Initialise(names);
            var currentGeneration = _generationManager.Initialise(names);
            var worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator
                .GetThreeWords());
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
                if (_heartbeat.GetCurrentTime().Year % 20 == 0)
                {
                    currentGeneration.Iteration += 1;
                    _consoleService.WriteLine($"New Generation: {currentGeneration.Iteration}");
                }

                //Age
                people = _ageProcessor.Age(people);
                

                //Death
                (people, currentGeneration) = _deathEngine.ProcessDeaths(people,
                    currentGeneration);//TODO: Tuples are not ideal, fix

                //Pair
                pairs = _personGenerator.GeneratePairs(people,
                    pairs,
                    worldSeed);

                //Generate Children
                people = _personGenerator.GenerateChildren(people);
            }
        }
        }
    }
}