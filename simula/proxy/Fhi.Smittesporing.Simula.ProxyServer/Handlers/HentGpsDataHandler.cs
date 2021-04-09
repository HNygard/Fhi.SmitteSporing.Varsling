using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class HentGpsDataHandler : IRequestHandler<HentGpsDataCommand, PagedListAm<SimulaGpsData>>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public HentGpsDataHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public Task<PagedListAm<SimulaGpsData>> Handle(HentGpsDataCommand request, CancellationToken cancellationToken)
        {
            return _simulaInternKlient.HentGpsData(request.Henvendelse);
        }
    }
}