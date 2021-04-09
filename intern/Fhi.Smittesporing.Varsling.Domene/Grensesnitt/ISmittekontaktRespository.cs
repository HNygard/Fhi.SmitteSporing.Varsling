using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Domene;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ISmittekontaktRespository
    {
        Task<PagedList<SmittekontaktOgSisteKontaktdato>> HentListeUtenDetaljer(Smittekontakt.Filter filter);
        Task<Option<SmittekontaktMedDetaljer>> HentForIdMedDetaljer(int smittekontaktId);
        Task<Option<Smittekontakt>> HentForIdMedVarsler(int smittekontaktId);
        Task<int> Opprett(Smittekontakt smittekontakt);
        Task<int> Opprett(List<Smittekontakt> smittekontaktListe);
        Task<List<Smittekontakt>> HentSmittekontaktTilVarslingForIndekspasient(int indekspasientId);
        Task<Option<Smittekontakt>> HentSmittekontaktTilVarslingForId(int smittekontaktId);
        Task<Option<Smittekontakt>> HentForIdMedTelefon(int smittekontaktId);
        Task<Option<Smittekontakt>> HentForIdMedDiagramOgTelefon(int smittekontaktId);
        Task<Option<Smittekontakt>> HentForIdMedGpsHistogramOgTelefon(int smittekontaktId);
        Task<Option<SmittekontaktDetaljer>> HentDetaljerForDagMedHtmlKartOgTelefon(int smittekontaktId, int smittekontaktDagDetaljerId);
        Task Lagre();
    }
}
