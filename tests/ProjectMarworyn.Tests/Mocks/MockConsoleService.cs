using ProjectMarworyn.Core.Services;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockConsoleService : IConsoleService
    {
        public List<string> Lines { get; } = new List<string>();

        public void WriteLine(string message,
            ConsoleColor colour)
        {
            Lines.Add(message);
        }

        public void Delay()
        {
        }
    }
}
