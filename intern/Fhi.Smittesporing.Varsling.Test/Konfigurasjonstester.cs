using AutoMapper;
using Fhi.Smittesporing.Varsling.Intern;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fhi.Smittesporing.Varsling.Test
{
    public class Konfigurasjonstester
    {
        [Fact]
        public void RegistrertMapper_HarGyldigKonfigurasjon()
        {
            var host = Program.CreateHostBuilder(new string[0]).Build();

            using (var scope = host.Services.CreateScope())
            {
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                mapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
        }
    }
}
