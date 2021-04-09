using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ISmsTjenesteFacade
    {
        Task<int> OpprettSmsJobb(int smsMalId, IEnumerable<SmsUtsending> smsUtsendinger);
        Task StartSmsJobb(int jobbId);
        Task<IEnumerable<SmsStatusoppdatering>> HentStatusoppdateringerEtterLopenummer(int lopenummer, int antall);
        Task<Option<SmsVarselMal>> HentSmsVarselMal(int malId);
        Task<int> LagreSmsVarselMal(SmsVarselMal mal);
        Task SendTestmeldingForMal(int malId, SmsUtsending smsUtsending);
        Task<SmsTilgang> SjekkTilgang();
        Task<IEnumerable<SmsStatusoppdatering>> HentStatusoppdateringerForSms(Guid referanse);
    }
}