using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectMarworyn.Extensions;

namespace ProjectMarworyn
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddProjectServices();

            var host = builder.Build();

            var initialiser = host.Services.GetRequiredService<Initiliser>();
            initialiser.Start();
        }
    }
}