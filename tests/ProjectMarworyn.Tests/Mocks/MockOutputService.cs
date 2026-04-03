using ProjectMarworyn.Services;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockOutputService : IConsoleService
    {
        public List<string> Messages { get; } = new List<string>();

        public void WriteLine(string message)
        {
            Messages.Add(message);
        }

        public void Delay()
        {
        }
    }
}