using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.ServerInfo
{
    public class HentSimulaApiInfo
    {
        public class Query : IRequest<SimulaApiInfoAm>
        {

        }

        public class Handler : IRequestHandler<Query, SimulaApiInfoAm>
        {
            private readonly ISimulaFacade _simulaFacade;

            public Handler(ISimulaFacade simulaFacade)
            {
                _simulaFacade = simulaFacade;
            }

            public Task<SimulaApiInfoAm> Handle(Query request, CancellationToken cancellationToken)
            {
                return _simulaFacade.GetSimulaApiInfo();
            }
        }
    }
}