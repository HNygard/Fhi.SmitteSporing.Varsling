using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.InternApi.Autorisering;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Simula.InternApi.Controllers
{
    [Authorize(Policy = Tilgangsregler.Simula)]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings]
    public class SlettingerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SlettingerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> HentSlettinger(List<string> telefonnummer)
        {
            return await _mediator.Send(new HentSlettingerQuery
            {
                Telefonnummer = telefonnummer
            });
        }
    }
}