using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Simula.InternApi.Requests
{
    public class HentDataBrukCommand : IRequest<PagedListAm<SimulaDataBruk>>
    {
        public SimulaDataBruk.HentCommand Henvendelse { get; set; }
    }
}