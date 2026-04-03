using ProjectMarworyn.Models;

namespace ProjectMarworyn
{
    internal interface IFileManager
    {
        public List<Name> ReadNameFile();
        public List<SeedWord> ReadSeedWordFile();
    }
}