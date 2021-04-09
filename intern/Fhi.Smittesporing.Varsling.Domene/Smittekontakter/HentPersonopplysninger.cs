using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentPersonopplysninger
    {
        public class Query : IRequest<Option<SmittekontaktPersonopplysningerAm>>
        {
            public int SmittekontaktId { get; set; }
            public string Brukernavn { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<SmittekontaktPersonopplysningerAm>>
        {
            private readonly ICryptoManagerFacade _cryptoManagerFacade;
            private readonly ISmittekontaktRespository _smittekontaktRespository;

            public Handler(ICryptoManagerFacade cryptoManagerFacade, ISmittekontaktRespository smittekontaktRespository)
            {
                _cryptoManagerFacade = cryptoManagerFacade;
                _smittekontaktRespository = smittekontaktRespository;
            }

            public async Task<Option<SmittekontaktPersonopplysningerAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kontakt = await _smittekontaktRespository.HentForIdMedTelefon(request.SmittekontaktId);
                return kontakt.Map(k => new SmittekontaktPersonopplysningerAm
                {
                    Telefonnummer = _cryptoManagerFacade
                        .DekrypterForBruker(k.Telefon.Telefonnummer, "Telefonnummer", "Kvalitetssikring av varsel", request.Brukernavn)
                });
            }
        }
    }
}