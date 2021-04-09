using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Helsenorge.Api.Handlers;
using Fhi.Smittesporing.Helsenorge.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Helsenorge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InnsynController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HelsenorgeKonfigurasjon _helsenorgeKonfigurasjon;
        private readonly ILogger<InnsynController> _logger;
        public InnsynController(IOptions<HelsenorgeKonfigurasjon> konfiguration, ILogger<InnsynController> logger, IMediator mediator)
        {
            _helsenorgeKonfigurasjon = konfiguration.Value;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("logg")]
        public async Task<ActionResult<IEnumerable<InnsynLoggHn>>> GetLogg()
        {
            var token = HelsenorgeToken.OpprettFraRequest(Request, _helsenorgeKonfigurasjon.ValidateToken, _helsenorgeKonfigurasjon.TokenSigningSertifikatThumbprint, _logger);

            if(token == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new HentInnsynlogg.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("loggformatert")]
        [Produces(System.Net.Mime.MediaTypeNames.Text.Xml)]
        public async Task<ActionResult<Innsyn>> GetLoggFormatert()
        {
            var token = HelsenorgeToken.OpprettFraRequest(Request, _helsenorgeKonfigurasjon.ValidateToken, _helsenorgeKonfigurasjon.TokenSigningSertifikatThumbprint, _logger);

            if (token == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new HentInnsynlogg.Query
            {
                Token = token
            });

            return Ok(result.AsInnsyn());
        }

        [HttpGet("")]
        public async Task<ActionResult<InnsynHn>> Get()
        {
            var token = HelsenorgeToken.OpprettFraRequest(Request, _helsenorgeKonfigurasjon.ValidateToken, _helsenorgeKonfigurasjon.TokenSigningSertifikatThumbprint, _logger);

            if(token == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new HentHelsenorgeInnsyn.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("hendelser")]
        public async Task<ActionResult<InnsynHendelserHn>> GetHendelser()
        {
            var token = HelsenorgeToken.OpprettFraRequest(Request, _helsenorgeKonfigurasjon.ValidateToken, _helsenorgeKonfigurasjon.TokenSigningSertifikatThumbprint, _logger);

            if (token == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new HentHelsenorgeHendelser.Query
            {
                Token = token
            });

            return Ok(result);
        }

        [HttpGet("formatert")]
        [Produces(System.Net.Mime.MediaTypeNames.Text.Xml)]
        public async Task<ActionResult<Innsyn>> GetFormatert()
        {
            var token = HelsenorgeToken.OpprettFraRequest(Request, _helsenorgeKonfigurasjon.ValidateToken, _helsenorgeKonfigurasjon.TokenSigningSertifikatThumbprint, _logger);

            if (token == null)
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new HentHelsenorgeInnsyn.Query
            {
                Token = token
            });

            return Ok(result.AsInnsyn());
        }
    }
}
