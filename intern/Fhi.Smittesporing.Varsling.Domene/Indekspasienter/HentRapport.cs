using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class HentRapport
    {
        public class Query : IRequest<IndekspasientRapportAm>
        {
            public IndekspasientRapportAm.Filter Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, IndekspasientRapportAm>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly IMapper _mapper;

            public Handler(IIndekspasientRepository indekspasientRepository, IMapper mapper)
            {
                _indekspasientRepository = indekspasientRepository;
                _mapper = mapper;
            }

            public async Task<IndekspasientRapportAm> Handle(Query request, CancellationToken cancellationToken)
            {
                var rapport = await _indekspasientRepository.HentRapport(
                    new Indekspasient.Filter
                    {
                        FraOgMed = request.Filter.FraOgMed.ToOption().Or(() => DateTime.Today.AddDays(-6)),
                        TilOgMed = request.Filter.FraOgMed.ToOption().Or(() => DateTime.Now),
                        KommuneNr = request.Filter.KommuneNr.SomeNotNull()
                    });

                return _mapper.Map<IndekspasientRapportAm>(rapport);
            }
        }
    }
}