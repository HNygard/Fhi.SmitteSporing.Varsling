using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using Optional;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class TelefonRepository : ITelefonRespository
    {
        private readonly SmitteVarslingContext _dbContext;

        public TelefonRepository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Option<Telefon>> FinnForTelefonnummer(string telefonnummer)
        {
            return (await _dbContext.Telefon.FirstOrDefaultAsync(x => x.Telefonnummer == telefonnummer)).SomeNotNull();
        }

        public Task<List<Telefon>> HentAlleTelefoner()
        {
            return _dbContext.Telefon.ToListAsync();
        }

        public async Task SlettTelefonMedTilknyttetInnhold(Telefon telefon)
        {
            var pasienterTilSletting = await _dbContext.Indekspasienter.Where(i => i.TelefonId == telefon.TelefonId).ToListAsync();
            var smittekontakterTilSletting = await _dbContext.Smittekontakt.Where(k => k.TelefonId == telefon.TelefonId).ToListAsync();

            _dbContext.Indekspasienter.RemoveRange(pasienterTilSletting);
            _dbContext.Smittekontakt.RemoveRange(smittekontakterTilSletting);
            _dbContext.Telefon.Remove(telefon);

            await _dbContext.SaveChangesAsync();
        }
    }
}
