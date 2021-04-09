using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Domene;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class InnsynloggRespository : IInnsynloggRespository
    {
        private readonly SmitteVarslingContext _dbContext;

        public InnsynloggRespository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<Innsynlogg>> HentInnsynlogg(InnsynFilter filter)
        {
            var queryForTelefonnummer = _dbContext.Innsynlogg.AsQueryable();

            filter.Telefonnummer.MatchSome(tlf =>
                queryForTelefonnummer = queryForTelefonnummer.Where(x => x.Hva.Equals(tlf))
            );
            filter.Telefonnummer.MatchNone(() =>
                queryForTelefonnummer = queryForTelefonnummer.Where(x => false)
            );

            var queryForFodselsnummer = _dbContext.Innsynlogg.AsQueryable();

            filter.Fodselsnummer.MatchSome(fnr =>
                queryForFodselsnummer = queryForFodselsnummer.Where(x => x.Hva.Equals(fnr))
            );
            filter.Fodselsnummer.MatchNone(() =>
                queryForFodselsnummer = queryForFodselsnummer.Where(x => false)
            );

            return await queryForTelefonnummer.Union(queryForFodselsnummer).ToPagedListAsync(filter);
        }

        public Task<PagedList<Smittekontakt>> HentInnsynSmittekontakt(InnsynFilter filter)
        {
            Task<PagedList<Smittekontakt>> result = null;

            filter.Telefonnummer.MatchNone(() =>
            {
                result = _dbContext.Smittekontakt.Where(s => false).ToPagedListAsync(filter);
            });
            filter.Telefonnummer.MatchSome(tlf =>
            {
                result = _dbContext.Telefon
                .Where(t => t.Telefonnummer.Equals(tlf))
                .Include(i => i.SmittekontaktForTelefon)
                .ThenInclude(ti => ti.Detaljer)
                .SelectMany(t => t.SmittekontaktForTelefon)
                .ToPagedListAsync(filter);
            });

            return result;
        }

        public async Task<int> Lagre()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Opprett(Innsynlogg innsynLogg)
        {
            _dbContext.Innsynlogg.Add(innsynLogg);
        }

        public async Task<int> OpprettOgLagre(Innsynlogg innsynLogg)
        {
            Opprett(innsynLogg);
            return await Lagre();
        }

        public async Task<PagedList<Indekspasient>> HentInnsynIndekspasienter(InnsynFilter filter)
        {
            var baseQuery = _dbContext.Indekspasienter
                .Include(i => i.Telefon)
                .Include(i => i.Kommune);

            //IQueryable<Indekspasient> queryForTelefonnummer = null;

            //filter.Telefonnummer.MatchSome(tlf =>
            //    queryForTelefonnummer = baseQuery
            //                    .Where(pasient => pasient.Telefon != null)
            //                    .Where(pasient => pasient.Telefon.Telefonnummer.Equals(tlf))
            //);
            //filter.Telefonnummer.MatchNone(() =>
            //    queryForTelefonnummer = baseQuery.Where(pasient => false)
            //);

            IQueryable<Indekspasient> queryForFodselsnummer = null;
            filter.Fodselsnummer.MatchSome(fnr =>
                queryForFodselsnummer = baseQuery.Where(pasient => pasient.Fodselsnummer.Equals(fnr))
            );
            filter.Fodselsnummer.MatchNone(() =>
                queryForFodselsnummer = baseQuery.Where(pasient => false)
            );

            //return await queryForTelefonnummer.Union(queryForFodselsnummer).ToPagedListAsync(filter);
            return await queryForFodselsnummer.ToPagedListAsync(filter);
        }

        public Task<PagedList<SmsVarsel>> HentInnsynSmsVarsel(InnsynFilter filter)
        {
            Task<PagedList<SmsVarsel>> res = null;
            filter.Telefonnummer.MatchNone(() =>
            {
                res = _dbContext.SmsVarsler.Where(t => false).ToPagedListAsync(filter);
            });
            filter.Telefonnummer.MatchSome(
                    sms =>
                    {
                        res = _dbContext.Telefon
                            .Include(i => i.SmittekontaktForTelefon)
                            .ThenInclude(v => v.SmsVarsler)
                            .Where(t => t.Telefonnummer.Equals(sms))
                            .SelectMany(t => t.SmittekontaktForTelefon.SelectMany(i => i.SmsVarsler))
                            .ToPagedListAsync(filter);
                    }
                );
            return res;
        }
    }
}
