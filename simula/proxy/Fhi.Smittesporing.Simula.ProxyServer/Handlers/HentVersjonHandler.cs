using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using Fhi.Smittesporing.Simula.InternKlient;
using MediatR;

namespace Fhi.Smittesporing.Simula.ProxyServer.Handlers
{
    public class HentVersjonHandler : IRequestHandler<HentVersjonQuery, ServerVersjonAm>
    {
        private readonly ISimulaInternKlient _simulaInternKlient;

        public HentVersjonHandler(ISimulaInternKlient simulaInternKlient)
        {
            _simulaInternKlient = simulaInternKlient;
        }

        public async Task<ServerVersjonAm> Handle(HentVersjonQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _simulaInternKlient.HentVersjon();
            }
            catch
            {
                return new ServerVersjonAm
                {
                    AssemblyVersjon = "Utilgjengelig"
                };
            }
        }
    }
}