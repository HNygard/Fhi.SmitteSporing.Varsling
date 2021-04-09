using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;
using Optional;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler
{
    public class HentStandardmal
    {
        public class Query : IRequest<Option<SmsVarselMalAm>>
        {

        }

        public class Handler : IRequestHandler<Query, Option<SmsVarselMalAm>>
        {
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly IMapper _mapper;

            public Handler(IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, ISmsTjenesteFacade smsTjenesteFacade, IMapper mapper)
            {
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _smsTjenesteFacade = smsTjenesteFacade;
                _mapper = mapper;
            }

            public async Task<Option<SmsVarselMalAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var standardSmsMalId =
                    await _applikasjonsinnstillingRepository.HentInnstilling<int>(Applikasjonsinnstillinger
                        .SmsVarselMalId);

                var smsMal =
                    await standardSmsMalId.FlatMapAsync(malId => _smsTjenesteFacade.HentSmsVarselMal(malId));

                return smsMal.Map(_mapper.Map<SmsVarselMalAm>);
            }
        }
    }
}