using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Fhi.Smittesporing.Simula.GatewayServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfig) =>
                {
                    loggerConfig
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithProperty("Serviceinformasjon", new Serviceinformasjon
                        {
                            Tjenestenavn = hostingContext.Configuration["Tjenestenavn"] ??
                                           "Fhi.Smittesporing.SimulaGateway",
                            Versjon = Assembly.GetEntryAssembly()?.GetName().Version.ToString()
                        }, destructureObjects: true);
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }

    public class Serviceinformasjon
    {
        public string Versjon { get; set; }
        public string Tjenestenavn { get; set; }
    }
}
