using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Optional;

namespace Fhi.Smittesporing.Simula.InternKlient
{
    public interface ISimulaInternKlient
    {
        Task<Option<Guid>> OpprettKontaktrapport(SimulaKontaktrapport.OpprettCommand command);
        Task<Option<SimulaKontaktrapport>> HentKontaktrapport(Guid id);
        Task<ServerVersjonAm> HentVersjon();
        Task<ServerVersjonAm> HentProxyVersjon();
        Task<List<string>> HentSlettinger(IEnumerable<string> telefonnummer);
        Task<PagedListAm<SimulaDataBruk>> HentLoggOverBruk(SimulaDataBruk.HentCommand command);
        Task<PagedListAm<SimulaGpsData>> HentGpsData(SimulaGpsData.HentCommand command);
    }
}