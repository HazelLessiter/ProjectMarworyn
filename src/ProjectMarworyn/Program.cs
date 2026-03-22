using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ProjectMarworyn.Extensions;
using ProjectMarworyn.Configuration;

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
            
            // Configure options
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Configuration"));
            
            builder.Services.AddServices();

            var host = builder.Build();

            var initialiser = host.Services.GetRequiredService<Initialiser>();
            initialiser.Start();
        }
    }
}