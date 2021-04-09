using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class HentKontaktrapportHandler : IRequestHandler<HentKontaktrapportQuery, Option<SimulaKontaktrapport>>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public HentKontaktrapportHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public Task<Option<SimulaKontaktrapport>> Handle(HentKontaktrapportQuery request, CancellationToken cancellationToken)
        {
            return _simulaInternKlient.HentKontaktrapport(request.Id);
        }
    }
}