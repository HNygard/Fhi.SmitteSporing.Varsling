using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Optional;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class SmittekontaktRespository : ISmittekontaktRespository
    {
        private readonly SmitteVarslingContext _dbContext;

        public SmittekontaktRespository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<SmittekontaktOgSisteKontaktdato>> HentListeUtenDetaljer(Smittekontakt.Filter filter)
        {
            var queryable = _dbContext.Smittekontakt.AsQueryable();

            filter.IndekspasientId.MatchSome(indekspasientId => 
                queryable = queryable.Where(k => k.IndekspasientId == indekspasientId));

            return await queryable
                .Select(x => new SmittekontaktOgSisteKontaktdato
                {
                    Smittekontakt = x,
                    SisteKontaktdato = x.Detaljer.OrderByDescending(d => d.Dato).Select(d => d.Dato).FirstOrDefault()
                })
                .ToPagedListAsync(filter);
        }

        public async Task<Option<SmittekontaktMedDetaljer>> HentForIdMedDetaljer(int smittekontaktId)
        {
            return (await _dbContext.Smittekontakt
                    .Include(x => x.Detaljer)
                    .Where(x => x.SmittekontaktId == smittekontaktId)
                    .Select(s => new SmittekontaktMedDetaljer
                    {
                        Smittekontakt = s,
                        HarGpsHistogram = _dbContext.SmittekontaktGpsHistogrammer.Any(x => x.SmittekontaktId == s.SmittekontaktId),
                        HarKontaktDiagram = _dbContext.SmittekontaktDiagrammer.Any(x => x.SmittekontaktId == s.SmittekontaktId),
                        Detaljer = s.Detaljer.Select(d => new SmittekontaktDetaljerMedKartinfo
                        {
                            SmittekontaktDetaljer = d,
                            HarKart = _dbContext.SmittekontaktDetaljerHtmlKart.Any(x => x.SmittekontaktDetaljerId == d.SmittekontaktDetaljerId)
                        })
                    })
                    .FirstOrDefaultAsync()
                ).SomeNotNull();
        }

        public async Task<Option<Smittekontakt>> HentForIdMedVarsler(int smittekontaktId)
        {
            return (await _dbContext.Smittekontakt
                    .Include(x => x.SmsVarsler)
                    .FirstOrDefaultAsync(x => x.SmittekontaktId == smittekontaktId)
                ).SomeNotNull();
        }

        public async Task<int> Opprett(Smittekontakt smittekontakt)
        {
            _dbContext.Smittekontakt.Add(smittekontakt);
            await _dbContext.SaveChangesAsync();
            return smittekontakt.SmittekontaktId;
        }

        public async Task<int> Opprett(List<Smittekontakt> smittekontaktListe)
        {
            await _dbContext.Smittekontakt.AddRangeAsync(smittekontaktListe);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<List<Smittekontakt>> HentSmittekontaktTilVarslingForIndekspasient(int indekspasientId)
        {
            return LagQueryableForVarsling()
                .Where(x => x.IndekspasientId == indekspasientId)
                .ToListAsync();
        }

        public async Task<Option<Smittekontakt>> HentSmittekontaktTilVarslingForId(int smittekontaktId)
        {
            return (await LagQueryableForVarsling()
                .Where(x => x.SmittekontaktId == smittekontaktId)
                .FirstOrDefaultAsync()).SomeNotNull();
        }

        public async Task<Option<Smittekontakt>> HentForIdMedTelefon(int smittekontaktId)
        {
            return (await _dbContext.Smittekontakt
                .Include(x => x.Telefon)
                .FirstOrDefaultAsync(x => x.SmittekontaktId == smittekontaktId))
                .SomeNotNull();
        }

        public async Task<Option<Smittekontakt>> HentForIdMedDiagramOgTelefon(int smittekontaktId)
        {
            return (await _dbContext.Smittekontakt
                    .Include(x => x.Telefon)
                    .Include(x => x.SoyleDiagram)
                    .FirstOrDefaultAsync(x => x.SmittekontaktId == smittekontaktId))
                .SomeNotNull();
        }

        public async Task<Option<Smittekontakt>> HentForIdMedGpsHistogramOgTelefon(int smittekontaktId)
        {
            return (await _dbContext.Smittekontakt
                    .Include(x => x.Telefon)
                    .Include(x => x.GpsHistogram)
                    .FirstOrDefaultAsync(x => x.SmittekontaktId == smittekontaktId))
                .SomeNotNull();
        }

        public async Task<Option<SmittekontaktDetaljer>> HentDetaljerForDagMedHtmlKartOgTelefon(int smittekontaktId, int smittekontaktDagDetaljerId)
        {
            return (await _dbContext.SmittekontaktDetaljer
                    .Include(x => x.OppsummertPlotDetaljerHtml)
                    .Include(x => x.Smittekontakt)
                    .ThenInclude(x => x.Telefon)
                    .FirstOrDefaultAsync(x => x.SmittekontaktId == smittekontaktId && x.SmittekontaktDetaljerId == smittekontaktDagDetaljerId))
                .SomeNotNull();
        }

        public Task Lagre()
        {
            return _dbContext.SaveChangesAsync();
        }

        private IQueryable<Smittekontakt> LagQueryableForVarsling()
        {
            return _dbContext.Smittekontakt
                .Include(x => x.SmsVarsler)
                .Include(x => x.Detaljer)
                .Include(x => x.Telefon)
                    .ThenInclude(x => x.IndekspasienterForTelefon)
                .Include(x => x.Telefon)
                    .ThenInclude(x => x.SmittekontaktForTelefon)
                        .ThenInclude(x => x.SmsVarsler)
                .Include(x => x.Indekspasient)
                    .ThenInclude(x => x.Kommune);
        }
    }
}
