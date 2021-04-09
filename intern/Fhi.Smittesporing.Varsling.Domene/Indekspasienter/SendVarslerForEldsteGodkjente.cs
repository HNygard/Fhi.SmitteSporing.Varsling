using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Varslingsregler;
using MediatR;
using Microsoft.Extensions.Logging;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class SendVarslerForEldsteGodkjente
    {
        public class Command : IRequest<bool>
        {

        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly ISmsVarselRepository _smsVarselRepository;
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;
            private readonly ILogger<SendVarslerForEldsteGodkjente> _logger;
            private readonly List<IVarslingsregel> _varslingsregler;

            public Handler(ISmsTjenesteFacade smsTjenesteFacade, ISmittekontaktRespository smittekontaktRespository, ISmsVarselRepository smsVarselRepository, IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, IIndekspasientRepository indekspasientRepository, ILogger<SendVarslerForEldsteGodkjente> logger, IEnumerable<IVarslingsregel> varslingsregler, ICryptoManagerFacade cryptoManagerFacade)
            {
                _smsTjenesteFacade = smsTjenesteFacade;
                _smittekontaktRespository = smittekontaktRespository;
                _smsVarselRepository = smsVarselRepository;
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _indekspasientRepository = indekspasientRepository;
                _logger = logger;
                _cryptoManagerFacade = cryptoManagerFacade;
                _varslingsregler = varslingsregler.ToList();
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var indekspasientTilVarsling = await _indekspasientRepository
                    .HentEldsteGodkjentMenIkkeVarslet();

                return await indekspasientTilVarsling.MatchAsync(
                    none: () =>
                    {
                        _logger.LogDebug("Ingen godkjente indekspasienter å sende varsler for..");
                        return Task.FromResult(false);
                    },
                    some: async x =>
                    {
                        await SendVarslerForIndekspasient(x.IndekspasientId);
                        return true;
                    });
            }

            private async Task SendVarslerForIndekspasient(int indekspasientId)
            {
                var smittekontakterForIndekspasient = await _smittekontaktRespository
                    .HentSmittekontaktTilVarslingForIndekspasient(indekspasientId);

                var smittekontakterSomSkalVarsles = smittekontakterForIndekspasient
                    .Where(x => _varslingsregler.All(r => r.KanVarsles(x))).ToList();

                if (smittekontakterSomSkalVarsles.Any())
                {
                    _logger.LogInformation($"Sender varsler til {smittekontakterSomSkalVarsles.Count} av {smittekontakterForIndekspasient.Count} smittekontakter for indekspasient {indekspasientId}");
                    // 1. Klargjør SMS-jobb
                    var smsVarsler = smittekontakterSomSkalVarsles.Select(x => new SmsVarsel
                    {
                        Smittekontakt = x
                    }).ToList();

                    await _smsVarselRepository.OpprettSmsVarsler(smsVarsler);

                    var smsMalId =
                        (await _applikasjonsinnstillingRepository.HentInnstilling<int>(Applikasjonsinnstillinger.SmsVarselMalId))
                        .ValueOr(() => throw new Exception("Ingen SMS-mal for varsel er satt opp"));

                    var fletteinnstillinger =
                        (await _applikasjonsinnstillingRepository.HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                        .ValueOr(() => new SmsFletteinnstillinger());

                    _logger.LogDebug($"Oppretter SMS-jobb for indekspasient {indekspasientId} basert på SMS-mal {smsMalId}.");
                    var smsUtsendinger = smsVarsler.Select(x => x.LagUtsending(_cryptoManagerFacade, fletteinnstillinger)).ToList();
                    var smsJobbId = await _smsTjenesteFacade.OpprettSmsJobb(smsMalId, smsUtsendinger);

                    smsVarsler.ForEach(x => x.Status = SmsStatus.Klargjort);
                    smittekontakterSomSkalVarsles.ForEach(x => x.VarsletTidspunkt = DateTime.Now);
                    await _smsVarselRepository.Lagre();

                    // 2. Fjern indekspasient fra varslingskø (Unngå duplikate meldinger hvis neste steg gir uventet resultat)
                    _logger.LogDebug($"Fjerner indekspasient {indekspasientId} fra varslingskø.");
                    await _indekspasientRepository.OppdaterVarslingsstatus(indekspasientId, Varslingsstatus.Klargjort);

                    // 3. Start SMS-jobb
                    _logger.LogDebug($"Starter SMS-jobb for indekspasient {indekspasientId}.");
                    await _smsTjenesteFacade.StartSmsJobb(smsJobbId);
                }

                // Til slutt: marker indekspasient som ferdig behandlet for varsling
                _logger.LogInformation("Indekspasient ferdig behandlet for varsling: " + indekspasientId);
                await _indekspasientRepository.OppdaterVarslingsstatus(indekspasientId, Varslingsstatus.Ferdig);
            }
        }
    }
}