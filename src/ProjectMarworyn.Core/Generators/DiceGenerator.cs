namespace ProjectMarworyn.Core.Generators
{
    public class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed,
            DateTime currentTime)
        {
            //Positional encoding avoids collisions between different dates (e.g. Day=1,Month=12 and Day=12,Month=1 previously summed to the same value)
            var datePart = (currentTime.Year * 100) + (currentTime.Month * 10) + currentTime.Day;
            var seed = worldSeed + datePart;

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