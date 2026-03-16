namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockDiceGenerator : IDiceGenerator
    {
        public Random Create()
        {
            return new Random();
        }
    }
}