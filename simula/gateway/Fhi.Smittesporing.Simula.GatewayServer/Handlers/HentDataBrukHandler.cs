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
    public class HentDataBrukHandler : IRequestHandler<HentDataBrukCommand, PagedListAm<SimulaDataBruk>>
    {
        private readonly ISimulaEksternApiKlient _simulaEksternApiKlient;

        public HentDataBrukHandler(ISimulaEksternApiKlient simulaEksternApiKlient)
        {
            _simulaEksternApiKlient = simulaEksternApiKlient;
        }

        public async Task<PagedListAm<SimulaDataBruk>> Handle(HentDataBrukCommand request, CancellationToken cancellationToken)
        {
            var response = await _simulaEksternApiKlient.HentTilgangslogg(new SimulaTransparencyRequest
            {
                PhoneNumber = request.Henvendelse.TilknyttetTelefonnummer,
                PageNumber = request.Henvendelse.Sideindeks + 1,// Simula bruker 1 index paging
                PerPage = request.Henvendelse.Sideantall,
                LegalMeans = request.Henvendelse.RettsligFormal,
                PersonId = request.Henvendelse.PersonIdentifikator,
                PersonName = request.Henvendelse.PersonNavn,
                PersonOrganization = request.Henvendelse.PersonOrganisasjon
            });

            return new PagedListAm<SimulaDataBruk>
            {
                Sideindeks = response.PageNumber - 1,// Simula bruker 1 index paging
                Sideantall = response.PerPage,
                TotaltAntall = response.Total,
                AntallSider = response.Total / response.PerPage + (response.Total % response.PerPage > 0 ? 1 : 0),
                Resultater = response.Events.Select(e => new SimulaDataBruk
                {
                    Tidspunkt = e.Timestamp,
                    PersonOrganisasjon = e.PersonOrganization,
                    PersonIdentifikator = e.PersonId,
                    PersonNavn = e.PersonName,
                    TekniskOrganisasjon = e.TechnicalOrganization,
                    TilknyttetTelefonnummer = e.PhoneNumber,
                    RettsligFormal = e.LegalMeans
                })
            };
        }
    }
}