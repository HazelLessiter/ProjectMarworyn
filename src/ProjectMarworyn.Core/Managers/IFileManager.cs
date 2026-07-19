using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core
{
    public interface IFileManager
    {
        public List<InitialPerson> ReadInitialPersonFile();
        public List<SeedWord> ReadSeedWordFile();
    }
}