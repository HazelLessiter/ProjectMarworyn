using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Generators
{
    internal interface ISeedGenerator
    {
        public List<SeedWord> GetThreeWords();
        public int CreateWorldSeed(List<SeedWord> seedWords);
    }
}