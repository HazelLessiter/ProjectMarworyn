namespace ProjectMarworyn.Core.Generators
{
    internal interface IDiceGenerator
    {
        public Random Create(int worldSeed);
        int Next(Random random, int startInclusive, int endExclusive);
        double NextDouble(Random random);
    }
}