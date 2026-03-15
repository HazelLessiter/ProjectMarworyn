using Microsoft.Extensions.Options;
using ProjectMarworyn.Configuration;

namespace ProjectMarworyn
{
    internal class ConsoleService : IConsoleService
    {
        private readonly Configuration.AppSettings _appSettings;

        public ConsoleService(IOptions<Configuration.AppSettings> appSettings)
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