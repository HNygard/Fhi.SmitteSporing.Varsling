using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Kommuner
{
    public class HentListe
    {
        public class Query : IRequest<List<KommuneAm>>
        {
            public string Sok { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<KommuneAm>>
        {
            private readonly IKommuneRepository _kommuneRepository;
            private readonly IMapper _mapper;

            public Handler(IKommuneRepository kommuneRepository, IMapper mapper)
            {
                _kommuneRepository = kommuneRepository;
                _mapper = mapper;
            }

            public async Task<List<KommuneAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kommuner = await _kommuneRepository.HentListe(request.Sok.SomeNotNull());
                return kommuner
                    .OrderBy(k => k.Navn)
                    .Select(x => _mapper.Map<KommuneAm>(x))
                    .ToList();
            }
        }
    }
}