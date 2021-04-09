using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;

namespace Fhi.Smittesporing.Simula.GatewayServer.Handlers
{
    public class HentVersjonHandler : IRequestHandler<HentVersjonQuery, ServerVersjonAm>
    {
        public Task<ServerVersjonAm> Handle(HentVersjonQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ServerVersjonAm
            {
                AssemblyVersjon = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });
        }
    }
}