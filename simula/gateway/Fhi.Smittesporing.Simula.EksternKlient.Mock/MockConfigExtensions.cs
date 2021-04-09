using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public static class MockConfigExtensions
    {
        public static IServiceCollection AddDelivMockSimulaApiKlient(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddSimulaApiKlient<SimulaEksternApiKlient, SimulaEksternApiKlient>(config)
                .AddTransient<MockSimulaEksternApiKlient>()
                .Configure<DelvisMockSimulaEksternApiKlient.Konfig>(config.GetSection("DelvisMock"))
                .AddTransient<ISimulaEksternApiKlient, DelvisMockSimulaEksternApiKlient>();
        }
    }
}