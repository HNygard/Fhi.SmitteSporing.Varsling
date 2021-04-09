using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler
{
    public class SjekkSmsTilgang
    {
        public class Query : IRequest<SmsTilgangAm>
        {

        }

        public class Handler : IRequestHandler<Query, SmsTilgangAm>
        {
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly IMapper _mapper;

            public Handler(ISmsTjenesteFacade smsTjenesteFacade, IMapper mapper)
            {
                _smsTjenesteFacade = smsTjenesteFacade;
                _mapper = mapper;
            }

            public async Task<SmsTilgangAm> Handle(Query request, CancellationToken cancellationToken)
            {
                return _mapper.Map<SmsTilgangAm>(await _smsTjenesteFacade.SjekkTilgang());
            }
        }
    }
}