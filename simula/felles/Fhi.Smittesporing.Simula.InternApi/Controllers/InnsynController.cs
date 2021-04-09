using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Autorisering;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Simula.InternApi.Controllers
{
    [Authorize(Policy = Tilgangsregler.Simula)]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings]
    public class InnsynController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InnsynController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("GpsData")]
        public async Task<ActionResult<PagedListAm<SimulaGpsData>>> HentGpsData(SimulaGpsData.HentCommand henvendelse)
        {
            return await _mediator.Send(new HentGpsDataCommand
            {
                Henvendelse = henvendelse
            });
        }

        [HttpPost("LoggOverBruk")]
        public async Task<ActionResult<PagedListAm<SimulaDataBruk>>> HentBrukslogg(SimulaDataBruk.HentCommand query)
        {
            return await _mediator.Send(new HentDataBrukCommand
            {
                Henvendelse = query
            });
        }
    }
}