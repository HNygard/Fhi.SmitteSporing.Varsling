using System;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Microsoft.Extensions.Options;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class DelvisMockSimulaEksternApiKlient : ISimulaEksternApiKlient
    {
        private readonly SimulaEksternApiKlient _ekteKlient;
        private readonly MockSimulaEksternApiKlient _mockKlient;
        private readonly Konfig _konfig;

        public DelvisMockSimulaEksternApiKlient(SimulaEksternApiKlient ekteKlient, MockSimulaEksternApiKlient mockKlient, IOptions<Konfig> konfig)
        {
            _ekteKlient = ekteKlient;
            _mockKlient = mockKlient;
            _konfig = konfig.Value;
        }

        public class Konfig
        {
            public bool MockKontaktoppslag { get; set; }
            public bool MockSlettinger { get; set; }
            public bool MockInnsyn { get; set; }
        }

        public Task<Option<SimulaStartContactResult>> StartKontaktberegning(SimulaStartContactRequest request)
        {
            return _konfig.MockKontaktoppslag
                ? _mockKlient.StartKontaktberegning(request)
                : _ekteKlient.StartKontaktberegning(request);
        }

        public Task<Option<Option<SimulaContactReport, SimulaNotFinishedResult>>> HentKontaktresultat(Guid requestId)
        {
            return _konfig.MockKontaktoppslag
                ? _mockKlient.HentKontaktresultat(requestId)
                : _ekteKlient.HentKontaktresultat(requestId);
        }

        public Task<SimulaDeletionsResponse> HentSlettinger(SimulaDeletionsRequest request)
        {
            return _konfig.MockSlettinger
                ? _mockKlient.HentSlettinger(request)
                : _ekteKlient.HentSlettinger(request);
        }

        public Task<SimulaEventListResponse<SimulaGpsDataEgressEvent>> HentGpsData(SimulaGpsDataEgressRequest request)
        {
            return _konfig.MockInnsyn
                ? _mockKlient.HentGpsData(request)
                : _ekteKlient.HentGpsData(request);
        }

        public Task<SimulaEventListResponse<SimulaAccessLogEvent>> HentTilgangslogg(SimulaTransparencyRequest request)
        {
            return _konfig.MockInnsyn
                ? _mockKlient.HentTilgangslogg(request)
                : _ekteKlient.HentTilgangslogg(request);
        }
    }
}