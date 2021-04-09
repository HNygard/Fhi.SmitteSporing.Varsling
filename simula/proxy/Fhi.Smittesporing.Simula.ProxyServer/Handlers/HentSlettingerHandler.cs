using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using MediatR;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class HentSlettingerHandler : IRequestHandler<HentSlettingerQuery, List<string>>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public HentSlettingerHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public async Task<List<string>> Handle(HentSlettingerQuery request, CancellationToken cancellationToken)
        {
            return await _simulaInternKlient.HentSlettinger(request.Telefonnummer);
        }
    }
}