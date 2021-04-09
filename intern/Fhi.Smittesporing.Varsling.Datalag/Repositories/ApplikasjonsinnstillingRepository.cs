using System.Text.Json;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class ApplikasjonsinnstillingRepository : IApplikasjonsinnstillingRepository
    {
        private readonly SmitteVarslingContext _dbContext;

        public ApplikasjonsinnstillingRepository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Option<T>> HentInnstilling<T>(string nokkel)
        {
            var appInnstilling = await _dbContext.Applikasjonsinnstillinger
                .FirstOrDefaultAsync(x => x.Nokkel == nokkel);

            return appInnstilling.SomeNotNull().Map(x => JsonSerializer.Deserialize<T>(x.Verdi));
        }

        public async Task SettInnstilling<T>(string nokkel, T verdi)
        {
            var appInnstilling = await _dbContext.Applikasjonsinnstillinger
                .FirstOrDefaultAsync(x => x.Nokkel == nokkel)
                ??
                _dbContext.Applikasjonsinnstillinger.Add(new Applikasjonsinnstilling
                {
                    Nokkel = nokkel
                }).Entity;

            appInnstilling.Verdi = JsonSerializer.Serialize(verdi);

            await _dbContext.SaveChangesAsync();
        }
    }
}