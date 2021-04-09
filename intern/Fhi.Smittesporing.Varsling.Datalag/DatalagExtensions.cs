

using Fhi.Smittesporing.Varsling.Datalag.Repositories;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Smittesporing.Varsling.Datalag
{
    public static class DatalagExtensions
    {
        public static IServiceCollection LeggTilDatalag(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SmitteVarslingContext>(options => options.UseSqlServer(connectionString));

            services.AddTransient<IIndekspasientRepository, IndekspasientRepository>();
            services.AddTransient<ITelefonRespository, TelefonRepository>();
            services.AddTransient<ISmittekontaktRespository, SmittekontaktRespository>();
            services.AddTransient<IApplikasjonsinnstillingRepository, ApplikasjonsinnstillingRepository>();
            services.AddTransient<ISmsVarselRepository, SmsVarselRepository>();
            services.AddTransient<IInnsynloggRespository, InnsynloggRespository>();
            services.AddTransient<IKommuneRepository, KommuneRepository>();

            return services;
        }
    }
}
