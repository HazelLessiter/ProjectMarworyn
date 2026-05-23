using ProjectMarworyn.Core;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockFileManager : IFileManager
    {
        public List<InitialPerson> InitialPeopleToReturn { get; set; } = new List<InitialPerson>();
        public List<SeedWord> SeedWordsToReturn { get; set; } = new List<SeedWord>();

        public List<InitialPerson> ReadInitialPersonFile()
        {
            return InitialPeopleToReturn;
        }

        public List<SeedWord> ReadSeedWordFile()
        {
            return SeedWordsToReturn;
        }
    }
}