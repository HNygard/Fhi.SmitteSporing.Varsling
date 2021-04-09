using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentGpsHistogram
    {
        public class Query : IRequest<Option<Fil>>
        {
            public int SmittekontaktId { get; set; }
            public string Brukernavn { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<Fil>>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(ISmittekontaktRespository smittekontaktRespository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Option<Fil>> Handle(Query request, CancellationToken cancellationToken)
            {
                var smittekontakt = await _smittekontaktRespository.HentForIdMedGpsHistogramOgTelefon(request.SmittekontaktId);

                return smittekontakt.FlatMap(s => s.GpsHistogram.SomeNotNull().Map(gpsHist => new Fil
                {
                    Filnavn = $"kontaktplot_{request.SmittekontaktId}.png",
                    MimeType = "image/png",
                    Bytes = _cryptoManagerFacade.DekrypterDataTilknyttet(gpsHist.Data, s.Telefon.Telefonnummer, "Telefonnummer", "Kvalitetssikring av kontaktrapport", request.Brukernavn)
                }));
            }
        }
    }
}