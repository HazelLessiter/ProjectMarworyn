using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;
using ProjectMarworyn.Core.Services;

namespace ProjectMarworyn.Core.Managers
{
    internal class SimulationManager : ISimulationManager
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
        private int _worldSeed;
        private List<Person> _people;
        private Generation _currentGeneration;
        private GameState _gameState;

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
            _gameState = new GameState();

            _worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator.GetThreeWords());
            var initialPeople = _fileManager.ReadInitialPersonFile();
            _people = _personGenerator.Initialise(initialPeople,
                _worldSeed);
            _currentGeneration = _generationManager.Initialise(_people);
            _heartbeat.Start();
        }

        public void ProgressDay()
        {
            var pairs = new List<Pair>();
            _gameState.Extinction = false;
            _gameState.NewYear = false;
            _gameState.NewGeneration = false;

            //Extinction
            if (_generationManager.CheckForExtinction(_people))
            {
                _gameState.Extinction = true;
                //_consoleService.WriteLine("The population has gone extinct. Less than 2 people remain");
                _heartbeat.Stop();
                return;
            }

            //Generation
            var currentTime = _heartbeat.GetCurrentTime();
            if (currentTime.Day == 01 &&
                currentTime.Month == 01)
            {
                //_consoleService.WriteLine($"Happy new year!",
                //    ConsoleColor.DarkMagenta);
                _gameState.NewYear = true;
                _gameState.NumberOfPeople = _people.Count;
                _gameState.NumberOfChildren = _people.Count(x => x.Age < 18);
                //_consoleService.WriteLine($"Number of people: {_people.Count}",
                //    ConsoleColor.DarkMagenta);
                //_consoleService.WriteLine($"Number of children: {_people.Count(x => x.Age < 18)}",
                //    ConsoleColor.DarkMagenta);

                if (currentTime.Year % 20 == 0)
                {
                    _currentGeneration.Iteration += 1;
                    _gameState.NewGeneration = true;
                    //_consoleService.WriteLine($"New Generation: {_currentGeneration.Iteration}",
                    //    ConsoleColor.DarkMagenta);
                }
            }

            //Age
            _people = _ageProcessor.Age(_people);

            //Death
            _currentGeneration = _deathEngine.ProcessDeaths(_people,
                _currentGeneration,
                _worldSeed);

            //Pair
            (pairs, _people) = _pairingEngine.GeneratePairs(_currentGeneration.People,
                pairs,
                _worldSeed);

            //Generate Children
            (var children, _people) = _personGenerator.GenerateChildren(pairs,
                _worldSeed,
                _people.MaxBy(x => x.Id ).Id,
                _people);

            _people = _people.Concat(children)
                .ToList();
        }
    }
}