using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IKommuneRepository
    {
        Task<Option<Kommune>> HentForId(int id);
        Task<List<Kommune>> HentListe(Option<string> soketekst);
        Task<Option<Kommune>> HentByKommuneNr(string kommuneNr);
        Task<int> Lagre();
    }
}