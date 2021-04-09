using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Async.Extensions;
using Optional.Collections;
using Optional.Unsafe;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class ForsokOpprett
    {
        public class Command : IRequest<bool>
        {
            public string Fodselsnummer { get; set; }
            public DateTime Opprettettidspunkt { get; set; }
            public DateTime? Provedato { get; set; }
            public string Bostedkommunenummer { get; set; }
            public string Bostedkommune { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly ILogger<Handler> _logger;
            private readonly FunksjonsbrytereKonfig _funksjonsbrytere;
            private readonly IPregFacade _pregFacade;
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;
            private readonly ITelefonNormalFacade _telefonNormalFacade;
            private readonly IKommuneRepository _kommuneRepository;
            private readonly IKontaktInfoFacade _kontaktInfoFacade;
            private readonly ITelefonRespository _telefonRespository;
            private readonly ISimulaFacade _simulaFacade;

            public Handler(IIndekspasientRepository indekspasientRepository, ICryptoManagerFacade cryptoManagerFacade, IKommuneRepository kommuneRepository, IPregFacade pregFacade, ILogger<Handler> logger, IKontaktInfoFacade kontaktInfoFacade, ITelefonRespository telefonRespository, ITelefonNormalFacade telefonNormalFacade, IOptions<FunksjonsbrytereKonfig> funksjonsbrytere, ISimulaFacade simulaFacade)
            {
                _indekspasientRepository = indekspasientRepository;
                _cryptoManagerFacade = cryptoManagerFacade;
                _kommuneRepository = kommuneRepository;
                _pregFacade = pregFacade;
                _logger = logger;
                _kontaktInfoFacade = kontaktInfoFacade;
                _telefonRespository = telefonRespository;
                _telefonNormalFacade = telefonNormalFacade;
                _simulaFacade = simulaFacade;
                _funksjonsbrytere = funksjonsbrytere.Value;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.Fodselsnummer))
                {
                    _logger.LogDebug("Ny indekspasient avvist pga manglende fødselsnummer");
                    return false;
                }

                if (!await SjekkOverAldersgrenseOgIkkeHemmeligAdresse(request.Fodselsnummer))
                {
                    _logger.LogDebug("Ny indekspasient avvist pga hemmelig adresse og/eller aldersgrense");
                    return false;
                }

                var telefonFraKrr = await FinnTelefonKrr(request.Fodselsnummer);

                var opprettesMedStatus = await telefonFraKrr.MatchAsync(
                    none: () =>
                    {
                        if (!_funksjonsbrytere.TillatAngiKontaktinfoManuelt)
                        {
                            _logger.LogDebug("Ny indekspasient avvist pga manglende kontaktinfo");
                            return Option.None<IndekspasientStatus>();
                        }
                        return IndekspasientStatus.KontaktInfoMangler.Some();
                    },
                    some: async tlf =>
                    {
                        if (_funksjonsbrytere.SjekkSimulaForOppretting && !await _simulaFacade.SjekkFinnes(tlf))
                        {
                            _logger.LogDebug("Ny indekspasient avvist pga ingen treff i simula");
                            return Option.None<IndekspasientStatus>();
                        }

                        return IndekspasientStatus.Registrert.Some();
                    }
                );

                return await opprettesMedStatus.MatchAsync(
                    none: () => false,
                    some: async status =>
                    {
                        var kommune = (await request.Bostedkommunenummer
                            .SomeWhen(x => !string.IsNullOrEmpty(x))
                            .MapAsync(async knr => (await _kommuneRepository
                                    .HentByKommuneNr(request.Bostedkommunenummer))
                                .ValueOr(new Kommune
                                {
                                    Navn = request.Bostedkommune,
                                    KommuneNr = request.Bostedkommunenummer
                                })
                            )).ValueOrDefault();

                        var telefon = await telefonFraKrr
                            .Map(normTlf => _cryptoManagerFacade.KrypterUtenBrukerinnsyn(normTlf))
                            .MapAsync(async kryptertTlf => (await _telefonRespository.FinnForTelefonnummer(kryptertTlf))
                                .ValueOr(() => new Telefon
                                {
                                    Telefonnummer = kryptertTlf
                                }));

                        await _indekspasientRepository.Opprett(new Indekspasient
                        {
                            Opprettettidspunkt = request.Opprettettidspunkt,
                            Provedato = request.Provedato,
                            Fodselsnummer = _cryptoManagerFacade.KrypterUtenBrukerinnsyn(request.Fodselsnummer),
                            Telefon = telefon.ValueOrDefault(),
                            Status = status,
                            Kommune = kommune
                        });

                        return true;
                    }
                );
            }

            private async Task<Option<string>> FinnTelefonKrr(string fodselsnummer)
            {
                var kontaktInfoResultat = await _kontaktInfoFacade.HentPersoner(new[] { fodselsnummer });

                if (kontaktInfoResultat == null || kontaktInfoResultat.Status != StatusKoder.Ok)
                {
                    throw new Exception("Fikk ugyldig svar fra kontaktinfotjenesten");
                }

                return kontaktInfoResultat.Kontaktinformasjon
                    .FirstOrNone(kontaktInfo => kontaktInfo.Fnr == fodselsnummer)
                    .Filter(kontaktInfo => kontaktInfo.Status == PersonStatus.Aktiv && !string.IsNullOrEmpty(kontaktInfo.Mobil))
                    .Map(kontaktinfo => kontaktinfo.Mobil)
                    .FlatMap(mobil => _telefonNormalFacade.NormaliserStrict(mobil));
            }

            private async Task<bool> SjekkOverAldersgrenseOgIkkeHemmeligAdresse(string fodselsnummer)
            {
                if (string.IsNullOrEmpty(fodselsnummer))
                {
                    // Må ha fødselsnummer
                    return false;
                }

                // Sjekk PREG - Må være minst 16 år og ikke ha hemmelig adresse
                var fodselsdatoSeinest = DateTime.Now.AddYears(-16);
                var ikkeHemmeligAdresseOgMinst16 = (await _pregFacade.FinnPerson(fodselsnummer))
                    .Map(x => !x.HarHemmeligAdresse && x.Fodselsdato.Map(fdato => fdato.Date <= fodselsdatoSeinest).ValueOr(false))
                    .ValueOr(false);
                return ikkeHemmeligAdresseOgMinst16;
            }
        }
    }
}