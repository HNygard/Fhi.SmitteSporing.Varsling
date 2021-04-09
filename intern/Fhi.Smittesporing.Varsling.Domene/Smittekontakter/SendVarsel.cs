using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class SendVarsel
    {
        public class Command : IRequest
        {
            public int SmittekontaktId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly ISmsVarselRepository _smsVarselRepository;
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(ISmsTjenesteFacade smsTjenesteFacade, ISmittekontaktRespository smittekontaktRespository, ISmsVarselRepository smsVarselRepository, IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _smsTjenesteFacade = smsTjenesteFacade;
                _smittekontaktRespository = smittekontaktRespository;
                _smsVarselRepository = smsVarselRepository;
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var smittekontakt = (await _smittekontaktRespository
                    .HentSmittekontaktTilVarslingForId(request.SmittekontaktId))
                    .ValueOr(() => throw new ArgumentException("Finnes ingen smittekontakt for angitt ID"));

                await SendVarsel(smittekontakt);

                return Unit.Value;
            }

            private async Task SendVarsel(Smittekontakt smittekontakt)
            {
                var smsVarsel = new SmsVarsel
                {
                    Smittekontakt = smittekontakt
                };

                await _smsVarselRepository.OpprettSmsVarsler(new []{ smsVarsel });

                var smsMalId =
                    (await _applikasjonsinnstillingRepository.HentInnstilling<int>(Applikasjonsinnstillinger.SmsVarselMalId))
                    .ValueOr(() => throw new Exception("Ingen SMS-mal for varsel er satt opp"));

                var fletteinnstillinger =
                    (await _applikasjonsinnstillingRepository.HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                    .ValueOr(() => new SmsFletteinnstillinger());


                var smsUtsending = smsVarsel.LagUtsending(_cryptoManagerFacade, fletteinnstillinger);
                var smsJobbId = await _smsTjenesteFacade.OpprettSmsJobb(smsMalId, new []{ smsUtsending });

                smsVarsel.Status = SmsStatus.Klargjort;
                smittekontakt.VarsletTidspunkt = DateTime.Now;
                await _smsVarselRepository.Lagre();
                await _smsTjenesteFacade.StartSmsJobb(smsJobbId);
            }
        }
    }
}