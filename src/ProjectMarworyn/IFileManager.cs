using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IFileManager
    {
        public List<InitialPerson> ReadInitialPersonFile();
        public List<SeedWord> ReadSeedWordFile();
    }
}