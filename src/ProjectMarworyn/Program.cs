using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ProjectMarworyn.Extensions;

namespace ProjectMarworyn
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            // Configure appsettings.json
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Appsettings.json", optional: false, reloadOnChange: true);
            
            builder.Services.AddProjectServices();

            var host = builder.Build();

            var initialiser = host.Services.GetRequiredService<Initiliser>();
            initialiser.Start();
        }
    }
}