using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IMsisFacade
    {
        Task<IEnumerable<MsisSmittetilfelle>> GetSmittetilfeller(DateTime fromDate);
    }
}
