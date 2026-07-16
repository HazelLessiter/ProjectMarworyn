using Microsoft.Extensions.DependencyInjection;
using ProjectMarworyn.Core.Generators;
using ProjectMarworyn.Core.Managers;
using ProjectMarworyn.Core.Models;

namespace ProjectMarworyn.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IAgeProcessor, AgeProcessor>();
            services.AddTransient<IGenerationManager, GenerationManager>();
            services.AddTransient<IDiceGenerator, DiceGenerator>();
            services.AddTransient<ISeedGenerator, SeedGenerator>();
            services.AddTransient<IPersonGenerator, PersonGenerator>();
            services.AddTransient<IDeathEngine, DeathEngine>();
            services.AddTransient<IPairingEngine, PairingEngine>();

            //Singleton because SimulationManager owns the one running simulation's mutable state (_people, _pairs, _currentGeneration),
            //the same way SimulationClock/GameState below represent app-wide state. If it were Transient and ever got resolved a
            //second time (a debug panel, a save/load screen), that second resolution would silently start a second, empty simulation
            //instead of sharing the one already running.
            services.AddSingleton<ISimulationManager, SimulationManager>();
            services.AddSingleton<IHeartbeat, Heartbeat>();
            services.AddSingleton<SimulationClock>();
            services.AddSingleton<GameState>();
        }
    }
}