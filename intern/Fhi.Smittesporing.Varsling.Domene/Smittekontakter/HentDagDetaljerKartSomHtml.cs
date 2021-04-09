using System;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentDagDetaljerKartSomHtml
    {
        public class Query : IRequest<Option<string>>
        {
            public int SmittekontaktId { get; set; }
            public int SmittekontaktDagDetaljerId { get; set; }
            public string Brukernavn { get; set; }
        }

        public class Handler: IRequestHandler<Query, Option<string>>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(ISmittekontaktRespository smittekontaktRespository, ICryptoManagerFacade cryptoManagerFacade)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<Option<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                var smittekontaktDetaljer = await _smittekontaktRespository.HentDetaljerForDagMedHtmlKartOgTelefon(request.SmittekontaktId, request.SmittekontaktDagDetaljerId);

                return smittekontaktDetaljer.Map(d =>
                {
                    if (d.OppsummertPlotDetaljerHtml == null)
                    {
                        return "<html><head></head><body>Kontakthendelse har ikke tilgjengelig kart.</body></html>";
                    }

                    var kartInnhold = _cryptoManagerFacade.DekrypterDataTilknyttet(
                        d.OppsummertPlotDetaljerHtml.Innhold, d.Smittekontakt.Telefon.Telefonnummer, "Telefonnummer",
                        "Kvalitetssikring av kontaktrapport", request.Brukernavn);

                    if (kartInnhold.StartsWith("<div"))
                    {
                        // Gammelt dynamisk kart (ikke fungerende sikker sone)
                        return $"<html><head></head><body>{kartInnhold}</body></html>";
                    }
                    else if (kartInnhold.IndexOf("no gps", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        // Innhold er beskrivende tekst om manglende GPS-data
                        return $"<html><head></head><body>{kartInnhold}</body></html>";
                    }
                    else
                    {
                        // Nytt statisk kart som bilde (for sikker sone uten tilgang til eksterne ressurser)
                        return $"<html><head></head><body><img style=\"width: 100%; height: auto;\" src=\"data:image/png;base64,{kartInnhold}\" /></body></html>";
                    }
                });
            }
        }
    }
}