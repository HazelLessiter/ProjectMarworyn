namespace ProjectMarworyn
{
    internal class Initiliser
    {
        public IFileManager _fileManager;
        public INameProcessor _nameProcessor;

        public Initiliser(IFileManager fileManager,
            INameProcessor nameProcessor)
        {
            _fileManager = fileManager;
            _nameProcessor = nameProcessor;
        }

        public void Start()
        {
            var names = _fileManager.ReadNameFile();
            for (int i = 0; i <= 100; i++)
            {
                _nameProcessor.ListNumberOfNamesByGender(names);
                _nameProcessor.GenerateChildren(names);
            }
        }
    }
}