using Microsoft.Extensions.DependencyInjection;
using ProjectMarworyn.Generators;
using ProjectMarworyn.Models;
using ProjectMarworyn.Services;

namespace ProjectMarworyn.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IAgeProcessor, AgeProcessor>();
            services.AddTransient<IGenerationManager, GenerationManager>();
            services.AddTransient<IConsoleService, ConsoleService>();
            services.AddTransient<IDiceGenerator, DiceGenerator>();
            services.AddTransient<ISeedGenerator, SeedGenerator>();
            services.AddTransient<IPersonGenerator, PersonGenerator>();
            services.AddTransient<IDeathEngine, DeathEngine>();
            services.AddTransient<IPairingEngine, PairingEngine>();

            services.AddSingleton<IHeartbeat, Heartbeat>();
            services.AddSingleton<SimulationManager>();
            services.AddSingleton<SimulationClock>();

            return services;
        }
    }
}