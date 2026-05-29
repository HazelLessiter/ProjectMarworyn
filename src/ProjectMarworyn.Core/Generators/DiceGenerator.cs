namespace ProjectMarworyn.Core.Generators
{
    public class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed,
            DateTime currentTime)
        {
            var seed = worldSeed + currentTime.Day + currentTime.Month + currentTime.Year;

            return new Random(seed);
        }

        public Random Create(int worldSeed)
        {
            return new Random(worldSeed);
        }

        public int Next(Random random,
            int startInclusive,
            int endExclusive)
        {
            return random.Next(startInclusive,
                endExclusive);
        }

        public double NextDouble(Random random)
        {
            return random.NextDouble();
        }
    }
}