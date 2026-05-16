using ProjectMarworyn.Generators;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockDiceGenerator : IDiceGenerator
    {
        private readonly Random _random;

        public MockDiceGenerator(int seed = 0)
        {
            _random = new Random(seed);
        }

        public Random Create(int worldSeed)
        {
            return _random;
        }

        public double NextDouble(Random random)
        {
            return random.NextDouble();
        }
    }
}