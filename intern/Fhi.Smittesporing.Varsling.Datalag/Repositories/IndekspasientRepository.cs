using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Microsoft.EntityFrameworkCore;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Felles.Domene;

namespace Fhi.Smittesporing.Varsling.Datalag.Repositories
{
    public class IndekspasientRepository : IIndekspasientRepository
    {
        private readonly SmitteVarslingContext _dbContext;

        public IndekspasientRepository(SmitteVarslingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<IndekspasientOgAntall>> HentMedAntall(Indekspasient.Filter filter)
        {
            return await LagQueryForFilter(filter)
                .Include(x => x.Telefon)
                .Select(x => new IndekspasientOgAntall
                {
                    Indekspasient = x,
                    AntallSmittekontakter = _dbContext.Smittekontakt.Count(k => k.IndekspasientId == x.IndekspasientId)
                })
                .ToPagedListAsync(filter);
        }

        public async Task<List<Indekspasient>> HentIndekspasienterTilKontaktsjekk(int antall = 100)
        {
            return await _dbContext.Indekspasienter
                .Where(e => 
                    e.Telefon != null &&
                    e.Status == IndekspasientStatus.Registrert)
                .Include(e => e.Telefon)
                .OrderBy(x => x.IndekspasientId)
                .Take(antall)
                .ToListAsync();
        }

        public Task<List<Indekspasient>> HentForIder(IEnumerable<int> indekspasientIder)
        {
            return _dbContext.Indekspasienter
                .Where(x => indekspasientIder.Contains(x.IndekspasientId))
                .ToListAsync();
        }

        public async Task<IndekspasientRapport> HentRapport(Indekspasient.Filter filter)
        {
            var query = LagQueryForFilter(filter);

            var indekspasientData = await query
                .Select(x => new IndekspasientRapport.IndekspasientData
                {
                    IndekspasientId = x.IndekspasientId,
                    Status = x.Status,
                    OpprettetTidspunkt = x.Opprettettidspunkt,
                    AntallSmittekontakter = x.Smittekontakter.Count
                })
                .OrderBy(x => x.OpprettetTidspunkt)
                .ToListAsync();

            return new IndekspasientRapport(filter, indekspasientData);
        }

        public async Task<int> Opprett(Indekspasient indekspasient)
        {
   
            _dbContext.Indekspasienter.Add(indekspasient);
            await _dbContext.SaveChangesAsync();
            return indekspasient.IndekspasientId;
        }

        public async Task<int> Lagre()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> OppdaterStatus(int indekspasientId,IndekspasientStatus nyStatus)
        {
            var indekspasient= await _dbContext.Indekspasienter.Where(e => e.IndekspasientId == indekspasientId).FirstAsync();
            indekspasient.Status = nyStatus;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> OppdaterVarslingsstatus(int indekspasientId, Varslingsstatus varslingstatus)
        {
            var indekspasient = await _dbContext.Indekspasienter.FindAsync(indekspasientId);
            indekspasient.Varslingsstatus = varslingstatus;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Option<Indekspasient>> HentEldsteGodkjentMenIkkeVarslet()
        {
            return (await _dbContext.Indekspasienter
                    .Where(x => x.Varslingsstatus == Varslingsstatus.Godkjent)
                    .OrderBy(x => x.Created)
                    .FirstOrDefaultAsync()
                ).SomeNotNull();
        }

        private IQueryable<Indekspasient> LagQueryForFilter(Indekspasient.Filter filter)
        {
            var queryable = _dbContext.Indekspasienter.AsQueryable();

            if (!filter.VisSkjulteStatuser)
            {
                queryable = queryable.Where(x => Indekspasient.StatuserKanVises.Contains(x.Status));
            }

            filter.KommuneNr.MatchSome(kommuneNr =>
                queryable = queryable.Where(x => x.Kommune.KommuneNr.Equals(kommuneNr))
            );
            filter.KreverGodkjenning.MatchSome(kreverGodkjenning =>
                queryable = kreverGodkjenning
                    ? queryable.Where(d => d.Varslingsstatus == Varslingsstatus.TilGodkjenning)
                    : queryable.Where(d => d.Varslingsstatus != Varslingsstatus.TilGodkjenning)
            );
            filter.ErFerdig.MatchSome(erFerdig =>
                queryable = erFerdig
                    ? queryable.Where(d => d.Varslingsstatus == Varslingsstatus.Ferdig)
                    : queryable.Where(d => d.Varslingsstatus != Varslingsstatus.Ferdig)
            );
            filter.ManglerKontaktinfo.MatchSome(manglerKontaktinfo =>
                queryable = manglerKontaktinfo
                    ? queryable.Where(d => d.Status == IndekspasientStatus.KontaktInfoMangler)
                    : queryable.Where(d => d.Status != IndekspasientStatus.KontaktInfoMangler)
            );
            filter.MedSmittekontakt.MatchSome(medSmittekontakt => 
                queryable = medSmittekontakt
                ? queryable.Where(e => e.Status == IndekspasientStatus.SmitteKontakt)
                : queryable.Where(e => e.Status != IndekspasientStatus.SmitteKontakt));
            filter.ErRegistert.MatchSome(erRegistert =>
                queryable = erRegistert
                    ? queryable.Where(d => d.Status == IndekspasientStatus.Registrert)
                    : queryable.Where(d => d.Status != IndekspasientStatus.Registrert));
            filter.IndekspasientId.MatchSome(indekspasientId => 
                queryable = queryable.Where(x => x.IndekspasientId == indekspasientId));

            filter.Telefonnummer.MatchSome(telefonnummer =>
                queryable = queryable.Where(x => x.Telefon.Telefonnummer == telefonnummer &&
                                                 Indekspasient.StatuserKanVises.Contains(x.Status)));

            filter.FraOgMed.MatchSome(fraOgMed => queryable = queryable.Where(x => x.Opprettettidspunkt >= fraOgMed));
            filter.TilOgMed.MatchSome(tilOgMed => queryable = queryable.Where(x => x.Opprettettidspunkt <= tilOgMed));

            return queryable;
        }

        public async Task<int> FjernUtgattData(DateTime utgattTidspunkt)
        {
            var indekspasienter = await _dbContext.Indekspasienter
                .Where(x => x.Opprettettidspunkt < utgattTidspunkt)
                .ToListAsync();

            //slett indekspasienter
            // Cascade Indekspasienter -> Smittekontakter -> SmittekontaktDetaljer, SmsVarsler
            _dbContext.Indekspasienter.RemoveRange(indekspasienter);

            var antall = await _dbContext.SaveChangesAsync();

            //slett smittekontakter og detaljer
            var utgatteSmittekontaktDetaljer = await _dbContext.SmittekontaktDetaljer
                .Where(d => d.Dato < utgattTidspunkt)
                .ToListAsync();
            _dbContext.SmittekontaktDetaljer.RemoveRange(utgatteSmittekontaktDetaljer);

            antall += await _dbContext.SaveChangesAsync();

            var utgatteSmittekontakter = await _dbContext.Smittekontakt
                .Where(k => !k.Detaljer.Any())
                .ToListAsync();
            _dbContext.Smittekontakt.RemoveRange(utgatteSmittekontakter);

            antall += await _dbContext.SaveChangesAsync();

            //slett ubrukte telefoner
            var ubrukteTelefoner = await _dbContext.Telefon
                .Where(t => !t.IndekspasienterForTelefon.Any() && !t.SmittekontaktForTelefon.Any())
                .ToListAsync();
            _dbContext.Telefon.RemoveRange(ubrukteTelefoner);

            antall += await _dbContext.SaveChangesAsync();

            //slett logger knyttet til telefon / fnr som ikke lenger finnes
            var utdatertInnsynslogg = await _dbContext.Innsynlogg
                .Where(il => !_dbContext.Indekspasienter.Any(ip => ip.Fodselsnummer == il.Hva) &&
                             !_dbContext.Telefon.Any(t => t.Telefonnummer == il.Hva))
                .ToListAsync();
            _dbContext.Innsynlogg.RemoveRange(utdatertInnsynslogg);

            antall += await _dbContext.SaveChangesAsync();

            return antall;
        }

        public async Task<Option<Indekspasient>> HentForIdInkluderTelefon(int indekspasientId)
        {
            return (await _dbContext.Indekspasienter
                    .Where(x => x.IndekspasientId == indekspasientId)
                    .Include(x => x.Telefon)
                    .FirstOrDefaultAsync())
                .SomeNotNull();
        }
    }
}
