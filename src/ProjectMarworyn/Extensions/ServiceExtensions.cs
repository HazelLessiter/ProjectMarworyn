using Microsoft.Extensions.DependencyInjection;
using ProjectMarworyn.Configuration;
using ProjectMarworyn.Services;

namespace ProjectMarworyn.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<INameProcessor, NameProcessor>();
            services.AddTransient<IGenerationManager, GenerationManager>();
            services.AddTransient<IConsoleService, ConsoleService>();
            services.AddSingleton<Initialiser>();

            return services;
        }
    }
}