using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Simula.InternApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerInfoController
    {
        private readonly IMediator _mediator;

        public ServerInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("Versjon")]
        public async Task<ActionResult<ServerVersjonAm>> HentVersjon()
        {
            return await _mediator.Send(new HentVersjonQuery());
        }
    }
}