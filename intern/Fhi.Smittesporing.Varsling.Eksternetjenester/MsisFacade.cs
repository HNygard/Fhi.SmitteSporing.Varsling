using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public class MsisFacade : IMsisFacade
    {
        private readonly HttpClient _httpclient;

        public MsisFacade(HttpClient httpclient)
        {
            _httpclient = httpclient;
        }

        public async Task<IEnumerable<MsisSmittetilfelle>> GetSmittetilfeller(DateTime fromDate)
        {
            return await KallExternTjeneste(fromDate);
        }

        private async Task<IEnumerable<MsisSmittetilfelle>> KallExternTjeneste(DateTime fromDate)
        {
            var from = fromDate.ToUniversalTime().ToString("O");

            var response = await _httpclient.GetAsync("api/smittesporing/korona?fraTidspunkt=" + from);

            response.EnsureSuccessStatusCode();

            var smittetilfeller = await response.Content.ReadAsAsync<IEnumerable<MsisSmittetilfelle>>();
            return smittetilfeller;
        }
    }
}
