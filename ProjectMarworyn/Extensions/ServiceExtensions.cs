using Microsoft.Extensions.DependencyInjection;

namespace ProjectMarworyn.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<INameProcessor, NameProcessor>();
            services.AddSingleton<Initiliser>();

            return services;
        }
    }
}