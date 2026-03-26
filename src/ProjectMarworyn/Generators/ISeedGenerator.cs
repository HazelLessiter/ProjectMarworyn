using ProjectMarworyn.Models;

namespace ProjectMarworyn.Generators
{
    internal interface ISeedGenerator
    {
        public List<SeedWord> GetThreeWords();
        public int CreateWorldSeed(List<SeedWord> seedWords);
    }
}