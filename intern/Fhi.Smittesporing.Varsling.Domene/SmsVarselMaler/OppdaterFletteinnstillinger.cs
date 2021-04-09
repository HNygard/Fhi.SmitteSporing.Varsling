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
    public class OppdaterFletteinnstillinger
    {
        public class Command : IRequest
        {
            public SmsFletteinnstillingerAm NyeInnstillinger { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly IMapper _mapper;

            public Handler(IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, IMapper mapper)
            {
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var innstillinger = (await _applikasjonsinnstillingRepository
                        .HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                    .ValueOr(() => new SmsFletteinnstillinger());

                _mapper.Map(request.NyeInnstillinger, innstillinger);

                await _applikasjonsinnstillingRepository.SettInnstilling(
                    Applikasjonsinnstillinger.SmsFletteinnstillinger, innstillinger);

                return Unit.Value;
            }
        }
    }
}