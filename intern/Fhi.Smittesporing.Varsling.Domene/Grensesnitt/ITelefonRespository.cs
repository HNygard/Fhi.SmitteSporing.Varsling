using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ITelefonRespository
    {
        Task<Option<Telefon>> FinnForTelefonnummer(string telefonnummer);
        Task<List<Telefon>> HentAlleTelefoner();
        Task SlettTelefonMedTilknyttetInnhold(Telefon telefon);
    }
}
