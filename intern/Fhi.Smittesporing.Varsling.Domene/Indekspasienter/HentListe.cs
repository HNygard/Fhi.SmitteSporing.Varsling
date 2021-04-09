using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using MediatR;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter

{
    public class HentListe
    {
        public class Query : IRequest<PagedListAm<IndekspasientMedAntallAm>>
        {
            public IndekspasientAm.Filter Filter { get; set; }
            public string Brukernavn { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedListAm<IndekspasientMedAntallAm>>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly IMapper _mapper;
            private readonly ITelefonNormalFacade _telefonNormalFacade;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(IIndekspasientRepository indekspasientRepository, IMapper mapper, ITelefonNormalFacade telefonNormalFacade, ICryptoManagerFacade cryptoManagerFacade)
            {
                _indekspasientRepository = indekspasientRepository;
                _mapper = mapper;
                _telefonNormalFacade = telefonNormalFacade;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<PagedListAm<IndekspasientMedAntallAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var filter = _mapper.Map<Indekspasient.Filter>(request.Filter);

                filter.Telefonnummer = filter.Telefonnummer.Map(tlf => _telefonNormalFacade
                    .NormaliserStrict(tlf)
                    .Match(
                        none: (e) => "<ugyldig-tlf>",
                        some: normTlf => _cryptoManagerFacade
                            .KrypterForBruker(normTlf, "Telefonnummer", "Søk i indekspasienter", request.Brukernavn)
                    ));

                var indekspasienter = await _indekspasientRepository.HentMedAntall(filter);
                return indekspasienter.TilAm(_mapper.Map<IndekspasientMedAntallAm>);
            }
        }
    }
}