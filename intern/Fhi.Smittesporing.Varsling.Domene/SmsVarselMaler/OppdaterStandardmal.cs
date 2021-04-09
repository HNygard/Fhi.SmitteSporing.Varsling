using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler
{
    public class OppdaterStandardmal
    {
        public class Command : IRequest
        {
            public SmsVarselMalAm SmsVarselMal { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;

            public Handler(IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, ISmsTjenesteFacade smsTjenesteFacade)
            {
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _smsTjenesteFacade = smsTjenesteFacade;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var standardSmsMalId =
                    await _applikasjonsinnstillingRepository.HentInnstilling<int>(Applikasjonsinnstillinger
                        .SmsVarselMalId);

                var eksisterendeMal =
                    await standardSmsMalId.FlatMapAsync(malId => _smsTjenesteFacade.HentSmsVarselMal(malId));

                var malTilLagring = eksisterendeMal.Match(
                    none: () => new SmsVarselMal(request.SmsVarselMal.Avsender, request.SmsVarselMal.Meldingsinnhold),
                    some: m =>
                    {
                        m.Avsender = request.SmsVarselMal.Avsender;
                        m.Meldingsinnhold = request.SmsVarselMal.Meldingsinnhold;
                        return m;
                    }
                );

                var oppdatertMalId = await _smsTjenesteFacade.LagreSmsVarselMal(malTilLagring);

                await _applikasjonsinnstillingRepository.SettInnstilling(
                    Applikasjonsinnstillinger.SmsVarselMalId, oppdatertMalId);

                return Unit.Value;
            }
        }
    }
}