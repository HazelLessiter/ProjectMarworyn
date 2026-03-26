using Microsoft.Extensions.DependencyInjection;
using ProjectMarworyn.Generators;
using ProjectMarworyn.Services;

namespace ProjectMarworyn.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<INameProcessor, NameProcessor>();
            services.AddTransient<IAgeProcessor, AgeProcessor>();
            services.AddTransient<IGenerationManager, GenerationManager>();
            services.AddTransient<IConsoleService, ConsoleService>();
            services.AddTransient<IDiceGenerator, DiceGenerator>();
            services.AddTransient<ISeedGenerator, SeedGenerator>();
            services.AddTransient<IPersonGenerator, PersonGenerator>();
            services.AddTransient<IDeathEngine, DeathEngine>();

            services.AddSingleton<IHeartbeat, Heartbeat>();
            services.AddSingleton<SimulationManager>();
            services.AddSingleton<SimulationClock>();

            return services;
        }
    }
}