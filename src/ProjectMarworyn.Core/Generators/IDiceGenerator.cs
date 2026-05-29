namespace ProjectMarworyn.Core.Generators
{
    public interface IDiceGenerator
    {
        public Random Create(int worldSeed, DateTime currentTime);
        public Random Create(int worldSeed);
        int Next(Random random, int startInclusive, int endExclusive);
        double NextDouble(Random random);
    }
}