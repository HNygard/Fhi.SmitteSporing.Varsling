using System.Reflection;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.ServerInfo;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ServerInfoController
    {
        private readonly IMediator _mediator;

        public ServerInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("Versjon")]
        public ActionResult<ServerVersjonAm> HentVersjon()
        {
            return new ServerVersjonAm
            {
                AssemblyVersjon = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
        }

        [AllowAnonymous]
        [HttpGet("SimulaApi")]
        public async Task<ActionResult<SimulaApiInfoAm>> HentSimulaApiInfo()
        {
            return await _mediator.Send(new HentSimulaApiInfo.Query());
        }

        [Authorize]
        [HttpGet("Funksjoner")]
        public async Task<ActionResult<FunksjonsbrytereAm>> HentFunksjonsbrytere()
        {
            return await _mediator.Send(new HentFunksjonsbrytere.Query());
        }
    }
}