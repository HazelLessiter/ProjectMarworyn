using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Managers
{
    internal class SimulationManager : ISimulationManager
    {
        private readonly IFileManager _fileManager;
        private readonly IGenerationManager _generationManager;
        private readonly ISeedGenerator _seedGenerator;
        private readonly IHeartbeat _heartbeat;
        private readonly IPersonGenerator _personGenerator;
        private readonly IAgeProcessor _ageProcessor;
        private readonly IDeathEngine _deathEngine;
        private readonly IPairingEngine _pairingEngine;
        private int _worldSeed;
        private List<Person> _people;
        private int _generationIteration;
        private GameState _gameState;
        private List<Pair> _pairs;

        public SimulationManager(IFileManager fileManager,
            IGenerationManager generationManager,
            ISeedGenerator seedGenerator,
            IHeartbeat heartbeat,
            IPersonGenerator personGenerator,
            IAgeProcessor ageProcessor,
            IDeathEngine deathEngine,
            IPairingEngine pairingEngine,
            GameState gameState)
        {
            _fileManager = fileManager;
            _generationManager = generationManager;
            _seedGenerator = seedGenerator;
            _heartbeat = heartbeat;
            _personGenerator = personGenerator;
            _ageProcessor = ageProcessor;
            _deathEngine = deathEngine;
            _pairingEngine = pairingEngine;
            _gameState = gameState;
        }

        public void Start()
        {
            _worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator.GetThreeWords());
            var initialPeople = _fileManager.ReadInitialPersonFile();

            _people = _personGenerator.Initialise(initialPeople,
                _worldSeed);

            _generationIteration = 0;
            _heartbeat.Start();
            _pairs = new List<Pair>();
        }

        public void ProgressDay()
        {
            _heartbeat.Tick();

            _gameState.Text.Clear();
            var date = _heartbeat.GetCurrentTime();
            _gameState.Text.Add($"Date: {date.Day} {date.Month} {date.Year}");

            //Extinction
            if (_generationManager.CheckForExtinction(_people))
            {
                _gameState.Text.Add("The population has gone extinct. Less than 2 people remain");
                _heartbeat.Stop();
                return;
            }

            //Generation
            var currentTime = _heartbeat.GetCurrentTime();
            if (currentTime.Day == 01 &&
                currentTime.Month == 01)
            {
                _gameState.Text.Add("Happy new year!");
                _gameState.Text.Add($"Number of people: {_people.Count}");
                _gameState.Text.Add($"Number of children: {_people.Count(x => x.Age < 18)}");

                if (currentTime.Year % 20 == 0)
                {
                    _generationIteration += 1;
                    _gameState.Text.Add($"New Generation: {_generationIteration}");
                }
            }

            //Age
            _people = _ageProcessor.Age(_people,
                date);

            //Death
            _people = _deathEngine.ProcessDeaths(_people,
                _worldSeed,
                date);

            //Pair
            var pairingResult = _pairingEngine.GeneratePairs(_people,
                _pairs,
                _worldSeed,
                date);
            _pairs = pairingResult.Pairs;

            //Everyone alive can die on the same day, and the extinction check only runs at the
            //start of the next day - so the rest of this day must handle an empty population
            if (_people.Count == 0)
            {
                return;
            }

            //Generate Children
            var children = _personGenerator.GenerateChildren(_pairs,
                _worldSeed,
                _people.MaxBy(x => x.Id ).Id,
                date);

            _people.AddRange(children);
        }
    }
}