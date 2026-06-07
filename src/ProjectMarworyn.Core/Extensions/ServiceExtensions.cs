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

            services.AddSingleton<ISimulationManager, SimulationManager>();
            services.AddSingleton<IHeartbeat, Heartbeat>();
            services.AddSingleton<SimulationClock>();
            services.AddSingleton<GameState>();
        }
    }
}