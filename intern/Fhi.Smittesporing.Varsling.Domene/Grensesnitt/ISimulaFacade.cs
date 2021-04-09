using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ISimulaFacade
    {
        Task<Option<SimulaKontaktrapport>> GetSmittekontakter(string telefonnummer, DateTime fraTidspunkt, DateTime tilTidspunkt);
        Task<SimulaApiInfoAm> GetSimulaApiInfo();
        Task<bool> SjekkFinnes(string telefonnummer);
        Task<IEnumerable<string>> SjekkSlettinger(IEnumerable<string> telefonnummerListe);
        Task<PagedListAm<SimulaGpsData>> HentGpsData(SimulaGpsData.HentCommand command);
        Task<PagedListAm<SimulaDataBruk>> HentLoggOverBruk(SimulaDataBruk.HentCommand command);
    }
}
