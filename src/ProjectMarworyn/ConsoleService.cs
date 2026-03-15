using Microsoft.Extensions.Configuration;

namespace ProjectMarworyn
{
    internal class ConsoleService : IConsoleService
    {
        private readonly int _delay;

        public ConsoleService(IConfiguration configuration)
        {
            _delay = configuration.GetValue<int>("Delay");
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Delay()
        {
            Thread.Sleep(_delay);
        }
    }
}