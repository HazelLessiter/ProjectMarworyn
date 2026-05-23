using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Generators
{
    public interface ISeedGenerator
    {
        public List<SeedWord> GetThreeWords();
        public int CreateWorldSeed(List<SeedWord> seedWords);
    }
}