using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class KommuneRepository : IKommuneRepository
    {
        private readonly SmitteVarslingContext _dbContext;

        public KommuneRepository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Option<Kommune>> HentForId(int id)
        {
            return (await _dbContext.Kommuner.FindAsync(id)).SomeNotNull();
        }

        public Task<List<Kommune>> HentListe(Option<string> soketekst)
        {
            var kommuner = _dbContext.Kommuner.AsQueryable();

            soketekst.MatchSome(s => kommuner = kommuner.Where(k => k.Navn.Contains(s)));

            return kommuner.ToListAsync();
        }

        public async Task<Option<Kommune>> HentByKommuneNr(string kommuneNr)
        {
            return (await _dbContext.Kommuner.Where(p => p.KommuneNr.Equals(kommuneNr)).SingleOrDefaultAsync()).SomeNotNull();
        }

        public Task<int> Lagre()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}