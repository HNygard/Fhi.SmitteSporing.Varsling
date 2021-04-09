
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using MediatR;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;

namespace Fhi.Smittesporing.Varsling.Domene.Smittekontakter

{
    public class HentListe
    {
        public class Query : IRequest<PagedListAm<SmittekontaktListemodellAm>>
        {
            public SmittekontaktAm.Filter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedListAm<SmittekontaktListemodellAm>>
        {
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly IMapper _mapper;

            public Handler(ISmittekontaktRespository smittekontaktRespository, IMapper mapper)
            {
                _smittekontaktRespository = smittekontaktRespository;
                _mapper = mapper;
            }

            public async Task<PagedListAm<SmittekontaktListemodellAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var filter = _mapper.Map<Smittekontakt.Filter>(request.Filter);
                var smittekontakter = await _smittekontaktRespository.HentListeUtenDetaljer(filter);
                return smittekontakter.Map(_mapper.Map<SmittekontaktListemodellAm>).TilAm();
            }
        }
    }
}