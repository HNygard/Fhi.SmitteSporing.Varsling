using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class OpprettKontaktrapportHandler : IRequestHandler<OpprettKontaktrapportCommand, Option<Guid>>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public OpprettKontaktrapportHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public Task<Option<Guid>> Handle(OpprettKontaktrapportCommand request, CancellationToken cancellationToken)
        {
            return _simulaInternKlient.OpprettKontaktrapport(request.Data);
        }
    }
}
