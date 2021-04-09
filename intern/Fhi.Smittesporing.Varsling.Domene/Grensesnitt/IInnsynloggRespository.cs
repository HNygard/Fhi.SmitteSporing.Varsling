using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg;
using Fhi.Smittesporing.Varsling.Felles.Domene;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IInnsynloggRespository
    {
        void Opprett(Innsynlogg innsynLogg);
        Task<int> Lagre();
        Task<int> OpprettOgLagre(Innsynlogg innsynLogg);
        Task<PagedList<Innsynlogg>> HentInnsynlogg(InnsynFilter filter);
        Task<PagedList<Smittekontakt>> HentInnsynSmittekontakt(InnsynFilter filter);
        Task<PagedList<Indekspasient>> HentInnsynIndekspasienter(InnsynFilter filter);
        Task<PagedList<SmsVarsel>> HentInnsynSmsVarsel(InnsynFilter filter);
    }
}
