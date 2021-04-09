using AutoMapper;
using Fhi.Smittesporing.Helsenorge.Api.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Helsenorge.Api.Handlers
{
    public class HentHelsenorgeHendelser
    {
        public class Query : IRequest<InnsynHendelserHn>
        {
            public HelsenorgeToken Token { get; set; }
        }

        public class Handler : IRequestHandler<Query, InnsynHendelserHn>
        {
            private readonly IInternFacade _facade;
            private readonly IMapper _mapper;

            public Handler(IInternFacade facade, IMapper mapper)
            {
                _facade = facade;
                _mapper = mapper;
            }

            public async Task<InnsynHendelserHn> Handle(Query request, CancellationToken cancellationToken)
            {
                var innsyn = await _facade.HentHelsenorgeHendelser(request.Token);

                return _mapper.Map<InnsynHendelserHn>(innsyn);
            }
        }
    }
}
