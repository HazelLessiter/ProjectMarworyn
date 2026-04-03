using ProjectMarworyn.Models;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockFileManager : IFileManager
    {
        public List<Name> NamesToReturn { get; set; } = new List<Name>();
        public List<SeedWord> SeedWordsToReturn { get; set; } = new List<SeedWord>();

        public List<Name> ReadNameFile()
        {
            return NamesToReturn;
        }

        public List<SeedWord> ReadSeedWordFile()
        {
            return SeedWordsToReturn;
        }
    }
}
