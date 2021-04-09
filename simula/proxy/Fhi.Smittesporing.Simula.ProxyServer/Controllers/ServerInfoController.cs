using System.Reflection;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerInfoController
    {
        [AllowAnonymous]
        [HttpGet("ProxyVersjon")]
        public ActionResult<ServerVersjonAm> HentProxyVersjon()
        {
            return new ServerVersjonAm
            {
                AssemblyVersjon = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
        }
    }
}