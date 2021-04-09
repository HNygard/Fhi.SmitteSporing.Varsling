using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter

{
    public class HentFraMsis
    {
        public class Query : IRequest<IEnumerable<MsisSmittetilfelle>>
        {
            public DateTime FraDato { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<MsisSmittetilfelle>>
        {
            private readonly IMsisFacade _msisFacade;
        
            public Handler(IMsisFacade msisFacade)
            {
                _msisFacade = msisFacade;
            }

            public Task<IEnumerable<MsisSmittetilfelle>> Handle(Query request, CancellationToken cancellationToken)
            {
                return _msisFacade.GetSmittetilfeller(request.FraDato);
            }
        }
    }
}