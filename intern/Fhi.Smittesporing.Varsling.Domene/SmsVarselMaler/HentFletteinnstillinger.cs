using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler
{
    public class HentFletteinnstillinger
    {
        public class Query : IRequest<SmsFletteinnstillingerAm>
        {

        }

        public class Handler : IRequestHandler<Query, SmsFletteinnstillingerAm>
        {
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly IMapper _mapper;

            public Handler(IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, IMapper mapper)
            {
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _mapper = mapper;
            }

            public async Task<SmsFletteinnstillingerAm> Handle(Query request, CancellationToken cancellationToken)
            {
                var innstillinger = (await _applikasjonsinnstillingRepository
                        .HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                    .ValueOr(() => new SmsFletteinnstillinger());

                return _mapper.Map<SmsFletteinnstillingerAm>(innstillinger);
            }
        }
    }
}