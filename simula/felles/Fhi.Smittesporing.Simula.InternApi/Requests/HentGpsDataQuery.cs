using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Simula.InternApi.Requests
{
    public class HentGpsDataCommand : IRequest<PagedListAm<SimulaGpsData>>
    {
        public SimulaGpsData.HentCommand Henvendelse { get; set; }
    }
}