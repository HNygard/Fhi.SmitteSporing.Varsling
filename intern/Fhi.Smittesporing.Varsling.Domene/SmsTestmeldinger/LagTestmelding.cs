using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Sms;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using MediatR;
using Optional;
using Optional.Async.Extensions;
using Optional.Unsafe;

namespace Fhi.Smittesporing.Varsling.Domene.SmsTestmeldinger
{
    public class LagTestmelding
    {
        public class Command : IRequest
        {
            public SmsTestmeldingAm Testmelding { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IApplikasjonsinnstillingRepository _applikasjonsinnstillingRepository;
            private readonly ISmsTjenesteFacade _smsTjenesteFacade;
            private readonly IKommuneRepository _kommuneRepository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(IApplikasjonsinnstillingRepository applikasjonsinnstillingRepository, ISmsTjenesteFacade smsTjenesteFacade, IKommuneRepository kommuneRepository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _applikasjonsinnstillingRepository = applikasjonsinnstillingRepository;
                _smsTjenesteFacade = smsTjenesteFacade;
                _kommuneRepository = kommuneRepository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var standardSmsMalId = (await _applikasjonsinnstillingRepository
                    .HentInnstilling<int>(Applikasjonsinnstillinger.SmsVarselMalId))
                    .ValueOr(() => throw new Exception("Standard SMS-mal mangler"));

                var fletteinnstillinger =
                    (await _applikasjonsinnstillingRepository.HentInnstilling<SmsFletteinnstillinger>(Applikasjonsinnstillinger.SmsFletteinnstillinger))
                    .ValueOr(() => new SmsFletteinnstillinger());

                // Oppretter en testkontakt med data nødvendig for SMS-fletting
                var smittekontakt = new Smittekontakt
                {
                    Telefon = new Telefon
                    {
                        Telefonnummer = _cryptoManagerFacade.KrypterUtenBrukerinnsyn(request.Testmelding.Telefonnummer)
                    },
                    Verifiseringskode = "7e57k0d3",
                    Risikokategori = request.Testmelding.Risikokategori ?? "high",
                    Detaljer = new List<SmittekontaktDetaljer>
                    {
                        new SmittekontaktDetaljer
                        {
                            Dato = request.Testmelding.DatoSisteKontakt ?? DateTime.Now.AddDays(-3)
                        }
                    },
                    Indekspasient = new Indekspasient
                    {
                        Kommune = (await request.Testmelding.KommuneId.ToOption()
                            .FlatMapAsync(kommuneId => _kommuneRepository.HentForId(kommuneId)))
                            .ValueOrDefault()
                    }
                };

                var smsVarsel = new SmsVarsel
                {
                    Smittekontakt = smittekontakt,
                    Referanse = request.Testmelding.Referanse
                };

                var testUtsending = smsVarsel.LagUtsending(_cryptoManagerFacade, fletteinnstillinger);

                await _smsTjenesteFacade.SendTestmeldingForMal(standardSmsMalId, testUtsending);

                return Unit.Value;
            }
        }
    }
}