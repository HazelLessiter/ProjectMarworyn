using Microsoft.Extensions.Options;
using ProjectMarworyn.Core.Configuration;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Services
{
    internal class ConsoleService : IConsoleService
    {
        private readonly SimulationClock _simulationClock;
        private readonly AppSettings _appSettings;

        public ConsoleService(IOptions<AppSettings> appSettings,
            SimulationClock simulationClock)
        {
            _simulationClock = simulationClock;
            _appSettings = appSettings.Value;
        }

        public void WriteLine(string message,
            ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine($"Day {_simulationClock.TickCount}: {message}");
        }

        public void Delay()
        {
            Thread.Sleep(_appSettings.Delay);
        }
    }
}