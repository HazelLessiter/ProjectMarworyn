using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn.Services
{
    internal class ConsoleService : IConsoleService
    {
        private readonly AppSettings _appSettings;

        public ConsoleService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Delay()
        {
            Thread.Sleep(_appSettings.Delay);
        }
    }
}