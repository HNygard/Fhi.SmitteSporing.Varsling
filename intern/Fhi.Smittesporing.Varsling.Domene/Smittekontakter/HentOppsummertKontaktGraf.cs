using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentOppsummertKontaktGraf
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
                var smittekontakt = await _smittekontaktRespository.HentForIdMedDiagramOgTelefon(request.SmittekontaktId);

                return smittekontakt.Map(s => new Fil
                {
                    Filnavn = $"kontaktplot_{request.SmittekontaktId}.png",
                    MimeType = "image/png",
                    Bytes = _cryptoManagerFacade.DekrypterDataTilknyttet(s.SoyleDiagram.Data, s.Telefon.Telefonnummer, "Telefonnummer", "Kvalitetssikring av kontaktrapport", request.Brukernavn)
                });
            }
        }
    }
}