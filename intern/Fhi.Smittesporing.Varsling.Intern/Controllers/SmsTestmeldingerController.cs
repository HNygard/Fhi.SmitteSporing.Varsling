using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.SmsTestmeldinger;
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
    public class SmsTestmeldingerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SmsTestmeldingerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> LagTestmelding(SmsTestmeldingAm testmelding)
        {
            await _mediator.Send(new LagTestmelding.Command
            {
                Testmelding = testmelding
            });

            return NoContent();
        }

        [HttpGet("{referanse}/Hendelser")]
        public async Task<ActionResult<List<SmsStatusoppdateringAm>>> HentHendelserForTestmelding(Guid referanse)
        {
            return await _mediator.Send(new HentHendelser.Query
            {
                Referanse = referanse
            });
        }
    }
}