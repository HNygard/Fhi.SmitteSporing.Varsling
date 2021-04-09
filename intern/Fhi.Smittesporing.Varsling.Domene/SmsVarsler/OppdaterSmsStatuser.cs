using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;
using Microsoft.Extensions.Logging;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Domene.SmsVarsler
{
    public class OppdaterSmsStatuser
    {
        public class Command : IRequest<int>
        {
            public int Batchstorrelse { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private const string SisteSmsOppdateringLopenummer = "SisteSmsOppdateringLopenummer";

            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ISmsVarselRepository _smsVarselRepository;
            private readonly ILogger<Handler> _logger;

            public Handler(ISmsTjenesteFacade smsTjenesteFacade, IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, ISmsVarselRepository smsVarselRepository, ILogger<Handler> logger)
            {
                _smsTjenesteFacade = smsTjenesteFacade;
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _smsVarselRepository = smsVarselRepository;
                _logger = logger;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var sisteOppdateringLopenummer = (await _applikasjonsinnstillingRepository
                    .HentInnstilling<int>(SisteSmsOppdateringLopenummer))
                    .ValueOr(-1);

                var smsOppdateringer = await _smsTjenesteFacade
                    .HentStatusoppdateringerEtterLopenummer(sisteOppdateringLopenummer, request.Batchstorrelse);

                int antallBehandlet = 0;
                foreach (var oppdatering in smsOppdateringer)
                {
                    var maybeMatch = await oppdatering.SmsUtsendingReferanse
                        .FlatMapAsync(guid => _smsVarselRepository.FinnForReferanse(guid));

                    await maybeMatch.MatchAsync(
                        none: () =>
                        {
                            _logger.LogWarning($"Fant ingen tilhørende SMS-varsel for oppdatering. (Løpenummer: {oppdatering.Loepenummer}, ref: {oppdatering.SmsUtsendingReferanse})");
                            return Task.CompletedTask;
                        },
                        some: async varsel =>
                        {
                            if (varsel.Status != oppdatering.GjeldeneStatus)
                            {
                                varsel.Status = oppdatering.GjeldeneStatus;
                                varsel.SisteEksterneHendelsestidspunkt = oppdatering.Tidspunkt;
                                await _smsVarselRepository.Lagre();
                            }
                        });

                    sisteOppdateringLopenummer = oppdatering.Loepenummer;
                    antallBehandlet++;
                }

                if (antallBehandlet > 0)
                {
                    await _applikasjonsinnstillingRepository
                        .SettInnstilling(SisteSmsOppdateringLopenummer, sisteOppdateringLopenummer);
                }

                return antallBehandlet;
            }
        }
    }
}