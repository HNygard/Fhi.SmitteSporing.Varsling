using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter
{
    public class HentDetaljertModell
    {
        public class Query : IRequest<Option<SmittekontaktAm>>
        {
            public int SmittekontaktId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Option<SmittekontaktAm>>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly IMapper _mapper;

            public Handler(ISmittekontaktRespository smittekontaktRespository, IMapper mapper)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _mapper = mapper;
            }

            public async Task<Option<SmittekontaktAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var smittekontakt = await _smittekontaktRespository.HentForIdMedDetaljer(request.SmittekontaktId);
                return smittekontakt.Map(_mapper.Map<SmittekontaktAm>);
            }
        }
    }
}