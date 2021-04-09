using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional;
using Optional.Collections;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class MockSimulaEksternApiKlient : ISimulaEksternApiKlient
    {
        private readonly SimulaMockData _testdata = new SimulaMockData();

        private static readonly IDictionary<Guid, SimulaContactReport> Resultater =
            new Dictionary<Guid, SimulaContactReport>();

        public Task<Option<SimulaStartContactResult>> StartKontaktberegning(SimulaStartContactRequest request)
        {
            var result = _testdata
                .LookupMockdata(request.PhoneNumber)
                .Else(_testdata.CreateRandomResponse(request.PhoneNumber));

            return Task.FromResult(result.Map(r =>
            {
                var resultId = Guid.NewGuid();
                Resultater.Add(resultId, r);
                return new SimulaStartContactResult
                {
                    RequestId = resultId,
                    ResultUrl = "http://results.tracker.ru/"
                };
            }));
        }

        public Task<Option<Option<SimulaContactReport, SimulaNotFinishedResult>>> HentKontaktresultat(Guid requestId)
        {
            return Task.FromResult(Resultater.GetValueOrNone(requestId)
                .Map(r => r.Some<SimulaContactReport, SimulaNotFinishedResult>()));
        }

        public Task<SimulaDeletionsResponse> HentSlettinger(SimulaDeletionsRequest request)
        {
            return Task.FromResult(new SimulaDeletionsResponse
            {
                DeletedPhoneNumbers = request.PhoneNumbers.Where(x => x.EndsWith("7"))
            });
        }

        public Task<SimulaEventListResponse<SimulaGpsDataEgressEvent>> HentGpsData(SimulaGpsDataEgressRequest request)
        {
            var result = new SimulaEventListResponseBuilder<SimulaGpsDataEgressEvent>(
                    request, 
                    (indeks, total) => new SimulaGpsDataEgressEventBuilder().Build())
                .Build();
            return Task.FromResult(result);
        }

        public Task<SimulaEventListResponse<SimulaAccessLogEvent>> HentTilgangslogg(SimulaTransparencyRequest request)
        {
            var result = new SimulaEventListResponseBuilder<SimulaAccessLogEvent>(
                    request,
                    (indeks, total) => new SimulaAccessLogEventBuilder().Build())
                .Build();
            return Task.FromResult(result);
        }
    }
}
