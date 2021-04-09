using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IIndekspasientRepository
    {
        Task<int> Opprett(Indekspasient indekspasient);
        Task<PagedList<IndekspasientOgAntall>> HentMedAntall(Indekspasient.Filter filter);
        Task<List<Indekspasient>> HentIndekspasienterTilKontaktsjekk(int antall = 100);
        Task<List<Indekspasient>> HentForIder(IEnumerable<int> indekspasientIder);
        Task<IndekspasientRapport> HentRapport(Indekspasient.Filter filter);

        Task<int> Lagre();
        Task<int> OppdaterStatus(int indekspasientId, IndekspasientStatus nyStatus);
        Task<int> OppdaterVarslingsstatus(int indekspasientId, Varslingsstatus varslingstatus);
        Task<Option<Indekspasient>> HentEldsteGodkjentMenIkkeVarslet();
        /// <summary>
        /// Fjerner alle utgåtte data for gitt UtgattTidspunkt
        /// TODO: Flytt til mer passende plass?
        /// </summary>
        Task<int> FjernUtgattData(DateTime utgattTidspunkt);
        Task<Option<Indekspasient>> HentForIdInkluderTelefon(int indekspasientId);
    }
}
