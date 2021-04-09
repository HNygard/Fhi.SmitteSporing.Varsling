using AutoMapper;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Optional.Unsafe;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
    public class HentInnsynSmittekontakter
    {
        public class Query : IRequest<PagedListAm<InnsynSmittekontaktAm>>
        {
            public InnsynFilterAm Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedListAm<InnsynSmittekontaktAm>>
        {
            private readonly IInnsynloggRespository _repository;
            private readonly IMapper _mapper;
            private readonly ITelefonNormalFacade _telefonManager;
            private readonly ICryptoManagerFacade _cryptoManager;

            public Handler(IInnsynloggRespository repository, IMapper mapper, ITelefonNormalFacade telefonManager, ICryptoManagerFacade cryptoManager)
            {
                _repository = repository;
                _mapper = mapper;
                _telefonManager = telefonManager;
                _cryptoManager = cryptoManager;
            }

            public async Task<PagedListAm<InnsynSmittekontaktAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var filter = _mapper.Map<InnsynFilter>(request.Filter);
                filter.Telefonnummer = filter.Telefonnummer
                    .Map(x => _telefonManager.NormaliserStrict(x).ValueOrFailure())
                    .Map(x => _cryptoManager.KrypterUtenBrukerinnsyn(x));
                var smittekontakter = await _repository.HentInnsynSmittekontakt(filter);
                return smittekontakter.Map(_mapper.Map<InnsynSmittekontaktAm>).TilAm();
            }
        }
    }
}
