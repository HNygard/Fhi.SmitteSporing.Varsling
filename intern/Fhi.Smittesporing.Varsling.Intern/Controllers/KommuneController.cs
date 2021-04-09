using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Kommuner;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
    [Authorize(Policy = Tilgangsregler.Basic)]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class KommuneController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KommuneController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<KommuneAm>>> HentListe(string sok)
        {
            return await _mediator.Send(new HentListe.Query
            {
                Sok = sok
            });
        }

        [HttpGet("{kommuneId}")]
        public async Task<ActionResult<KommuneAm>> HentDetaljer(int kommuneId)
        {
            var result = await _mediator.Send(new HentDetaljer.Query
            {
                Id = kommuneId
            });

            return result.Match<ActionResult<KommuneAm>>(
                none: () => NotFound(),
                some: x => x
            );
        }

        [HttpPut("{kommuneId}/SmsFletteinfo")]
        public async Task<ActionResult> OppdaterSmsFletteinfo(int kommuneId, KommuneAm.OppdaterSmsFletteinfoCommand command)
        {
            await _mediator.Send(new OppdaterSmsFletteinfo.Command
            {
                KommuneId = kommuneId,
                SmsFletteinfo = command.SmsFletteinfo
            });

            return NoContent();
        }
    }
}