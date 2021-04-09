using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ISmsVarselRepository
    {
        Task OpprettSmsVarsler(IEnumerable<SmsVarsel> smsVarsler);
        Task<Option<SmsVarsel>> FinnForReferanse(Guid referanse);
        Task<int> Lagre();
    }
}