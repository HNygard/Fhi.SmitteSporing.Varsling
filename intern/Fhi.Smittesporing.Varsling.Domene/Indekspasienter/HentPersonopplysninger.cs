using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;
using Optional.Unsafe;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class HentPersonopplysninger
    {
        public class Query : IRequest<Option<IndekspasientPersonopplysningerAm>>
        {
            public string Brukernavn { get; set; }
            public int IndekspasientId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<IndekspasientPersonopplysningerAm>>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(IIndekspasientRepository indekspasientRepository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _indekspasientRepository = indekspasientRepository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Option<IndekspasientPersonopplysningerAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var indekspasient = await _indekspasientRepository.HentForIdInkluderTelefon(request.IndekspasientId);
                return indekspasient.Map(s =>
                {
                    if (!s.KanVisesTilBruker)
                    {
                        throw new Exception("Kan bare dekryptere indekspasienter som ble funnet i Smittestopp");
                    }
                    return new IndekspasientPersonopplysningerAm
                    {
                        Telefonnummer = s.Telefon
                            .SomeNotNull()
                            .Map(tlf => _cryptoManagerFacade.DekrypterForBruker(tlf.Telefonnummer, "Telefonnummer",
                                "Kvalitetssikring av varsler", request.Brukernavn))
                            .ValueOrDefault(),
                        Fodselsnummer = s.Fodselsnummer
                            .SomeNotNull()
                            .Map(fnr => _cryptoManagerFacade.DekrypterForBruker(fnr, "Fodselsnummer",
                                "Kvalitetssikring av varsler", request.Brukernavn))
                            .ValueOrDefault()
                    };
                });
            }
        }
    }
}