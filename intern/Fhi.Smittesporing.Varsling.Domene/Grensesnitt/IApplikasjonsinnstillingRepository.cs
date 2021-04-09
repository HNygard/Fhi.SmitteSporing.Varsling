using System.Threading.Tasks;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IApplikasjonsinnstillingRepository
    {
        Task<Option<T>> HentInnstilling<T>(string nokkel);
        Task SettInnstilling<T>(string nokkel, T verdi);
    }
}