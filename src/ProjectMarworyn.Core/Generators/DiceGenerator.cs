namespace ProjectMarworyn.Core.Generators
{
    public class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed,
            DateTime currentTime)
        {
            //yyyyMMdd-style positional encoding: Month*100+Day never exceeds 1231, so it can never spill into Year's digits, guaranteeing no collisions
            var datePart = (currentTime.Year * 10000) + (currentTime.Month * 100) + currentTime.Day;
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