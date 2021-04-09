using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Kommuner
{
    public class HentKommune
    {
        public class Query : IRequest<Option<KommuneAm>>
        {
            public string KommuneNr { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<KommuneAm>>
        {
            private readonly IKommuneRepository _kommuneRepository;
            private readonly IMapper _mapper;

            public Handler(IKommuneRepository kommuneRepository, IMapper mapper)
            {
                _kommuneRepository = kommuneRepository;
                _mapper = mapper;
            }

            public async Task<Option<KommuneAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kommune = await _kommuneRepository.HentByKommuneNr(request.KommuneNr);
                return kommune.Map(x => _mapper.Map<KommuneAm>(x));
            }
        }
    }
}