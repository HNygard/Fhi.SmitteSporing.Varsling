using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class HentDataBrukHandler : IRequestHandler<HentDataBrukCommand, PagedListAm<SimulaDataBruk>>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public HentDataBrukHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public Task<PagedListAm<SimulaDataBruk>> Handle(HentDataBrukCommand request, CancellationToken cancellationToken)
        {
            return _simulaInternKlient.HentLoggOverBruk(request.Henvendelse);
        }
    }
}