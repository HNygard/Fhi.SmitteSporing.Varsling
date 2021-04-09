using System;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient
{
    public interface ISimulaEksternApiKlient
    {
        /// <summary>
        /// Start kontaktberegning for angitt request
        /// </summary>
        /// <param name="request">Request med info om tidsrom og TLF</param>
        /// <returns>None hvis TLF ikke finnes, Some hvis TLF finnes</returns>
        Task<Option<SimulaStartContactResult>> StartKontaktberegning(SimulaStartContactRequest request);

        /// <summary>
        /// Hent kontaktberegningsresultat for en request-ID fra tidligere kall til StartKontaktberegning
        /// </summary>
        /// <param name="requestId">Request-ID fra tidligere StartKontaktberegning</param>
        /// <returns>None hvis Request-ID er ugyldig, Some av SimulaContactReport/SimulaNotFinishedResult hvis gyldig request-ID avhengig av status</returns>
        Task<Option<Option<SimulaContactReport, SimulaNotFinishedResult>>> HentKontaktresultat(Guid requestId);

        /// <summary>
        /// Tar en inputliste av telefonnummer som så sjekkes mot eksisterende TLF hos simula.
        /// </summary>
        /// <param name="request">Request med telefonnummerliste som skal sjekkes</param>
        /// <returns>Liste av TLF som simula ikke fant igjen på sin side (slettet)</returns>
        Task<SimulaDeletionsResponse> HentSlettinger(SimulaDeletionsRequest request);

        /// <summary>
        /// Henter GPS data hos simula for et gitt telefonnummer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SimulaEventListResponse<SimulaGpsDataEgressEvent>> HentGpsData(SimulaGpsDataEgressRequest request);

        /// <summary>
        /// Henter liste for innsyn i "logg over bruk"
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SimulaEventListResponse<SimulaAccessLogEvent>> HentTilgangslogg(SimulaTransparencyRequest request);
    }
}