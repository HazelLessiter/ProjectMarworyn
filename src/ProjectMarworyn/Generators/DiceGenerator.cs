namespace ProjectMarworyn.Generators
{
    internal class DiceGenerator : IDiceGenerator
    {
        public Random Create(int worldSeed)
        {
            return new Random(worldSeed);
        }

        public double NextDouble(Random random)
        {
            return random.NextDouble();
        }
    }
}