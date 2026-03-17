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

        public Initialiser(IFileManager fileManager,
            INameProcessor nameProcessor,
            IGenerationManager generationManager,
            IConsoleService consoleService,
            ISeedGenerator seedGenerator)
        {
            _fileManager = fileManager;
            _nameProcessor = nameProcessor;
            _generationManager = generationManager;
            _consoleService = consoleService;
            _seedGenerator = seedGenerator;
        }

        public void Start()
        {
            var names = _fileManager.ReadNameFile();
            var currentGeneration = _generationManager.Initialise(names);
            var worldSeed = _seedGenerator.CreateWorldSeed(_seedGenerator
                .GetThreeWords());

            while(currentGeneration.Names.Count > 1)
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