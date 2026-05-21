using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    internal interface IFileManager
    {
        public List<InitialPerson> ReadInitialPersonFile();
        public List<SeedWord> ReadSeedWordFile();
    }
}