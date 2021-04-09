using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;
using Optional.Collections;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class HentForId
    {
        public class Query : IRequest<Option<IndekspasientMedAntallAm>>
        {
            public int IndekspasientId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<IndekspasientMedAntallAm>>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly IMapper _mapper;

            public Handler(IIndekspasientRepository indekspasientRepository, IMapper mapper)
            {
                _indekspasientRepository = indekspasientRepository;
                _mapper = mapper;
            }

            public async Task<Option<IndekspasientMedAntallAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pagedList = await _indekspasientRepository.HentMedAntall(new Indekspasient.Filter
                {
                    IndekspasientId = request.IndekspasientId.Some(),
                    Sideantall = 1.Some()
                });
                return pagedList.Resultater.FirstOrNone().Map(_mapper.Map<IndekspasientMedAntallAm>);
            }
        }
    }
}