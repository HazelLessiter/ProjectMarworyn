using ProjectMarworyn.Generators;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockDiceGenerator : IDiceGenerator
    {
        private readonly Random _random;

        public MockDiceGenerator(Random? random = null)
        {
            _random = random ??
                new Random(0);
        }

        public Random Create(int worldSeed)
        {
            return _random;
        }
    }
}