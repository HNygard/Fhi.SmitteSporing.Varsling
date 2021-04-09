using AutoMapper;
using Fhi.Smittesporing.Helsenorge.Api.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Helsenorge.Api.Handlers
{
    public class HentInnsynlogg
    {
        public class Query : IRequest<IEnumerable<InnsynLoggHn>>
        {
            public HelsenorgeToken Token { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<InnsynLoggHn>>
        {
            private readonly IInternFacade _facade;
            private readonly IMapper _mapper;

            public Handler(IInternFacade facade, IMapper mapper)
            {
                _facade = facade;
                _mapper = mapper;
            }

            public async Task<IEnumerable<InnsynLoggHn>> Handle(Query request, CancellationToken cancellationToken)
            {
                var logg = await _facade.HentInnsynlogg(request.Token);
                return _mapper.Map<IEnumerable<InnsynLoggHn>>(logg);
            }
        }
    }
}
