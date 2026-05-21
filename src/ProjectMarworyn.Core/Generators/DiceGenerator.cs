namespace ProjectMarworyn.Core.Generators
{
    internal class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed)
        {
            return new Random(worldSeed);
        }

        public int Next(Random random,
            int startInclusive,
            int endExclusive)
        {
            return random.Next(startInclusive, endExclusive);
        }

        public double NextDouble(Random random)
        {
            return random.NextDouble();
        }
    }
}