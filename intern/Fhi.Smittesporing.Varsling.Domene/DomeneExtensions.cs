using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Smittesporing.Varsling.Domene
{
    public static class DomeneExtensions
    {
        public static IServiceCollection LeggTilVarslingsregler(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddSingleton<IVarslingsregel, ErIkkeSmittetSelvRegel>()
                .AddSingleton<IVarslingsregel, KontaktInnenforToUkerRegel>()
                .Configure<RisikoKategoriRegel>(config.GetSection("RisikoKategoriRegel"))
                .AddSingleton<IVarslingsregel, RisikoKategoriRegel>()
                .AddSingleton<IVarslingsregel, SammeKontaktIkkeVarsletRegel>()
                .AddSingleton<IVarslingsregel, SammeTelefonIkkeVarsletSammeDagRegel>();
        }
    }
}