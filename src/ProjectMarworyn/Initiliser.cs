using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal class Initiliser
    {
        public IFileManager _fileManager;
        public INameProcessor _nameProcessor;
        public IGenerationManager _generationManager;

        public Initiliser(IFileManager fileManager,
            INameProcessor nameProcessor,
            IGenerationManager generationManager)
        {
            _fileManager = fileManager;
            _nameProcessor = nameProcessor;
            _generationManager = generationManager;
        }

        public void Start()
        {
            var names = _fileManager.ReadNameFile();
            var currentGeneration = _generationManager.Initialise(names);

            while(currentGeneration.Names.Count() > 1)
            {
                _nameProcessor.ListNumberOfNamesByGender(currentGeneration.Names);
                currentGeneration = _nameProcessor.GenerateChildren(currentGeneration);
                Console.WriteLine($"New Generation: {currentGeneration.Iteration}");
            }
            if (currentGeneration.Names.Count() < 2)
            {
                Console.WriteLine("The population has gone extinct. Less than 2 people remain");
                Thread.Sleep(500);
            }
        }
    }
}