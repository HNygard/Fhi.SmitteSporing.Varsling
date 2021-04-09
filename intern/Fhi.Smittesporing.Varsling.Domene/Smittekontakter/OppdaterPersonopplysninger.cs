using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    /// <summary>
    /// Kun for manipulasjon av data ifm test/verifikasjon
    /// </summary>
    public class OppdaterPersonopplysninger
    {
        public class Command : IRequest
        {
            public int SmittekontaktId { get; set; }
            public SmittekontaktPersonopplysningerAm Personopplysninger { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly ITelefonRespository _telefonRespository;
            private readonly ITelefonNormalFacade _telefonNormalFacade;
            private readonly ICryptoManagerFacade _cryptoManager;
            private readonly FunksjonsbrytereKonfig _funksjonsbrytereKonfig;

            public Handler(ISmittekontaktRespository smittekontaktRespository, ITelefonNormalFacade telefonNormalFacade, ITelefonRespository telefonRespository, IOptions<FunksjonsbrytereKonfig> funksjonsbrytereKonfig, ICryptoManagerFacade cryptoManager)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _telefonNormalFacade = telefonNormalFacade;
                _telefonRespository = telefonRespository;
                _cryptoManager = cryptoManager;
                _funksjonsbrytereKonfig = funksjonsbrytereKonfig.Value;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!_funksjonsbrytereKonfig.EndringAvSmittekontakter)
                {
                    throw new Exception("Endring av smittekontakter er ikke tillatt i dette miljøet");
                }

                var smittekontakt = (await _smittekontaktRespository
                    .HentForIdMedTelefon(request.SmittekontaktId))
                    .ValueOr(() => throw new Exception("Finnes ingen smittekontakt med ID = " + request.SmittekontaktId));


                var normTlf = _telefonNormalFacade.NormaliserStrict(request.Personopplysninger.Telefonnummer)
                    .ValueOr(e => throw new Exception(e));
                var kryptertTlfNummer = _cryptoManager.KrypterUtenBrukerinnsyn(normTlf);

                var telefonMatch = await _telefonRespository.FinnForTelefonnummer(kryptertTlfNummer);

                smittekontakt.Telefon = telefonMatch.ValueOr(new Telefon
                {
                    Telefonnummer = kryptertTlfNummer
                });

                await _smittekontaktRespository.Lagre();

                return Unit.Value;
            }
        }
    }
}
