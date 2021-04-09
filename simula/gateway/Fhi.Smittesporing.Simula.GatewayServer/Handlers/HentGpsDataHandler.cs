using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Simula.GatewayServer.Handlers
{
    public class HentGpsDataHandler : IRequestHandler<HentGpsDataCommand, PagedListAm<SimulaGpsData>>
    {
        private readonly ISimulaEksternApiKlient _simulaEksternApiKlient;

        public HentGpsDataHandler(ISimulaEksternApiKlient simulaEksternApiKlient)
        {
            _simulaEksternApiKlient = simulaEksternApiKlient;
        }

        public async Task<PagedListAm<SimulaGpsData>> Handle(HentGpsDataCommand request, CancellationToken cancellationToken)
        {
            var response = await _simulaEksternApiKlient.HentGpsData(new SimulaGpsDataEgressRequest
            {
                PhoneNumber = request.Henvendelse.TilknyttetTelefonnummer,
                PageNumber = request.Henvendelse.Sideindeks + 1,// Simula bruker 1 index paging
                PerPage = request.Henvendelse.Sideantall,
                LegalMeans = request.Henvendelse.RettsligFormal,
                PersonId = request.Henvendelse.PersonIdentifikator,
                PersonName = request.Henvendelse.PersonNavn,
                PersonOrganization = request.Henvendelse.PersonOrganisasjon,
                TimeFrom = request.Henvendelse.FraTidspunkt,
                TimeTo = request.Henvendelse.TilTidspunkt
            });

            return new PagedListAm<SimulaGpsData>
            {
                Sideindeks = response.PageNumber - 1,// Simula bruker 1 index paging
                Sideantall = response.PerPage,
                TotaltAntall = response.Total,
                AntallSider = response.Total / response.PerPage + (response.Total % response.PerPage > 0 ? 1 : 0),
                Resultater = response.Events.Select(e => new SimulaGpsData
                {
                    FraTidspunkt = e.TimeFrom,
                    TilTidspunkt = e.TimeTo,
                    Lengdegrad = e.Longitude,
                    Breddegrad = e.Latitude,
                    Noyaktighet = e.Accuracy,
                    Hastighet = e.Speed,
                    Hoyde = e.Altitude,
                    HoydeNoyaktighet = e.AltitudeAccuracy
                })
            };
        }
    }
}