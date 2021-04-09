using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;

namespace Fhi.Smittesporing.Simula.GatewayServer.Handlers
{
    public class HentSlettingerHandler : IRequestHandler<HentSlettingerQuery, List<string>>
    {
        private readonly ISimulaEksternApiKlient _eksternApiKlient;

        public HentSlettingerHandler(ISimulaEksternApiKlient eksternApiKlient)
        {
            _eksternApiKlient = eksternApiKlient;
        }

        public async Task<List<string>> Handle(HentSlettingerQuery request, CancellationToken cancellationToken)
        {
            var simulaResponse = await _eksternApiKlient.HentSlettinger(new SimulaDeletionsRequest
            {
                PhoneNumbers = request.Telefonnummer
            });
            return simulaResponse.DeletedPhoneNumbers.ToList();
        }
    }
}