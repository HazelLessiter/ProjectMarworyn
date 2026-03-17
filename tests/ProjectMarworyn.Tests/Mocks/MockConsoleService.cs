using ProjectMarworyn.Services;

namespace ProjectMarworyn.Tests.Mocks
{
    internal class MockConsoleService : IConsoleService
    {
        public List<string> Lines { get; } = new List<string>();

        public void WriteLine(string message)
        {
            Lines.Add(message);
        }

        public void Delay()
        {
        }
    }
}
