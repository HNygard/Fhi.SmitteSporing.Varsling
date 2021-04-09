using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Test.Web
{
    public class ControllerTestBase
    {
        protected ControllerContext LagControllerContextMedBruker(string brukernavn = "Ola")
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new GenericPrincipal(new GenericIdentity(brukernavn), new string[0])
                }
            };
        }
    }
}
