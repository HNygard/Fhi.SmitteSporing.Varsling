using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.ServerInfo
{
    public class HentFunksjonsbrytere
    {
        public class Query : IRequest<FunksjonsbrytereAm>
        {

        }

        public class Handler : IRequestHandler<Query, FunksjonsbrytereAm>
        {
            private readonly FunksjonsbrytereKonfig _fnKonfig;

            public Handler(IOptions<FunksjonsbrytereKonfig> fnKonfig)
            {
                _fnKonfig = fnKonfig.Value;
            }

            public Task<FunksjonsbrytereAm> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new FunksjonsbrytereAm
                {
                    TillatAngiKontaktinfoManuelt = _fnKonfig.TillatAngiKontaktinfoManuelt
                });
            }
        }
    }
}