    using Fhi.Smittesporing.Helsenorge.Api.Handlers;
using Fhi.Smittesporing.Helsenorge.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Helsenorge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestInnsynController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestInnsynController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("logg")]
        public async Task<ActionResult<IEnumerable<InnsynLoggHn>>> GetLogg([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
        {
            var token = new HelsenorgeToken(fodselsnummer, fodselsnummer, telefonnummer);

            var result = await _mediator.Send(new HentInnsynlogg.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("loggformatert")]
        [Produces(System.Net.Mime.MediaTypeNames.Text.Xml)]
        public async Task<ActionResult<Innsyn>> GetLoggFormatert([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
        {
            var token = new HelsenorgeToken(fodselsnummer, fodselsnummer, telefonnummer);

            var result = await _mediator.Send(new HentInnsynlogg.Query
            {
                Token = token
            });

            return Ok(result.AsInnsyn());
        }

        [HttpGet("")]
        public async Task<ActionResult<InnsynHn>> Get([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
        {
            var token = new HelsenorgeToken(fodselsnummer, fodselsnummer, telefonnummer);

            var result = await _mediator.Send(new HentHelsenorgeInnsyn.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("hendelser")]
        public async Task<ActionResult<InnsynHendelserHn>> GetHendelser([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
        {
            var token = new HelsenorgeToken(fodselsnummer, fodselsnummer, telefonnummer);

            var result = await _mediator.Send(new HentHelsenorgeHendelser.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("formatert")]
        [Produces(System.Net.Mime.MediaTypeNames.Text.Xml)]
        public async Task<ActionResult<Innsyn>> GetFormatert([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
        {
            var token = new HelsenorgeToken(fodselsnummer, fodselsnummer, telefonnummer);

            var result = await _mediator.Send(new HentHelsenorgeInnsyn.Query
            {
                Token = token
            });

            return Ok(result.AsInnsyn());
        }
    }
}
