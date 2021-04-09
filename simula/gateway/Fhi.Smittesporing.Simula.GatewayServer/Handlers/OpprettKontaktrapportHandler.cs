using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;
using Optional;
using Optional.Unsafe;

namespace Fhi.Smittesporing.Simula.GatewayServer.Handlers
{
    public class OpprettKontaktrapportHandler : IRequestHandler<OpprettKontaktrapportCommand, Option<Guid>>
    {
        private readonly ISimulaEksternApiKlient _simulaKlient;

        public OpprettKontaktrapportHandler(ISimulaEksternApiKlient simulaKlient)
        {
            _simulaKlient = simulaKlient;
        }

        public async Task<Option<Guid>> Handle(OpprettKontaktrapportCommand request, CancellationToken cancellationToken)
        {
            var simulaApiStartResponse = await _simulaKlient.StartKontaktberegning(new SimulaStartContactRequest
            {
                PhoneNumber = request.Data.Telefonnummer,
                TimeFrom = request.Data.FraTidspunkt,
                TimeTo = request.Data.TilTidspunkt
            });

            return simulaApiStartResponse.Map(x => x.RequestId);
        }
    }
}