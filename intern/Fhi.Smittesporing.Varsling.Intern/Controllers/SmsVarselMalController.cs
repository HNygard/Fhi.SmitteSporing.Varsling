using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.SmsVarselMaler;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
    [Authorize(Policy = Tilgangsregler.Smittesporer)]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class SmsVarselMalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SmsVarselMalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<SmsVarselMalAm>> HentStandardmal()
        {
            var result = await _mediator.Send(new HentStandardmal.Query());

            return result.Match<ActionResult<SmsVarselMalAm>>(
                none: () => NotFound(),
                some: x => x
            );
        }

        [HttpGet("Flettefelter")]
        public async Task<ActionResult<List<SmsFlettefeltAm>>> HentTilgjengeligeFlettefelter()
        {
            return await _mediator.Send(new HentFlettefelter.Query());
        }

        [HttpPut]
        public async Task<ActionResult> OppdaterStandardmal(SmsVarselMalAm mal)
        {
            await _mediator.Send(new OppdaterStandardmal.Command
            {
                SmsVarselMal = mal
            });

            return NoContent();
        }

        [HttpGet("Fletteinnstillinger")]
        public async Task<ActionResult<SmsFletteinnstillingerAm>> HentFletteinnstillinger()
        {
            return await _mediator.Send(new HentFletteinnstillinger.Query());
        }

        [HttpPut("Fletteinnstillinger")]
        public async Task<ActionResult> OppdaterFletteinnstillinger(SmsFletteinnstillingerAm nyeInnstillinger)
        {
            await _mediator.Send(new OppdaterFletteinnstillinger.Command
            {
                NyeInnstillinger = nyeInnstillinger
            });

            return NoContent();
        }

        [HttpGet("Tilgangsjekk")]
        public async Task<ActionResult<SmsTilgangAm>> SjekkSmsTilgang()
        {
            return await _mediator.Send(new SjekkSmsTilgang.Query());
        }
    }
}