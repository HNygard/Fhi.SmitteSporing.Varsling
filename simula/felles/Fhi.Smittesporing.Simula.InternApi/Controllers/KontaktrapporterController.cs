using System;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
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
    public class KontaktrapporterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KontaktrapporterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<SimulaKontaktrapport.OpprettResult>> OpprettKontaktrapport(SimulaKontaktrapport.OpprettCommand command)
        {
            var kanskjeOpprettetId = await _mediator.Send(new OpprettKontaktrapportCommand
            {
                Data = command
            });

            return kanskjeOpprettetId.Match<ActionResult>(
                none: NotFound,
                some: id => Created(Request.Path.Add("/" + id), new { Id = id })
            );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimulaKontaktrapport>> HentKontaktrapport(Guid id)
        {
            var kanskjeRapport = await _mediator.Send(new HentKontaktrapportQuery
            {
                Id = id
            });

            return kanskjeRapport
                .Map(x => (ActionResult<SimulaKontaktrapport>) x)
                .ValueOr(() => NotFound());
        }
    }
}
