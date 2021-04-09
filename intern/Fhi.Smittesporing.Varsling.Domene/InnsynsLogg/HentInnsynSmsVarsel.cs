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
    public class HentInnsynSmsVarsel
	{
        public class Query : IRequest<PagedListAm<InnsynSmsVarselAm>>
        {
            public InnsynFilterAm Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedListAm<InnsynSmsVarselAm>>
        {
            private readonly IInnsynloggRespository _repository;
            private readonly IMapper _mapper;
            private readonly ITelefonNormalFacade _telefonManager;
            private readonly ICryptoManagerFacade _cryptoManagerFacade;

            public Handler(IInnsynloggRespository repository, IMapper mapper, ITelefonNormalFacade telefonManager, ICryptoManagerFacade cryptoManagerFacade)
            {
                _repository = repository;
                _mapper = mapper;
                _telefonManager = telefonManager;
                _cryptoManagerFacade = cryptoManagerFacade;
            }

            public async Task<PagedListAm<InnsynSmsVarselAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var filter = _mapper.Map<InnsynFilter>(request.Filter);
                filter.Telefonnummer = filter.Telefonnummer
                    .Map(x => _telefonManager.NormaliserStrict(x).ValueOrFailure())
                    .Map(x => _cryptoManagerFacade.KrypterUtenBrukerinnsyn(x));
                var smsvarsler = await _repository.HentInnsynSmsVarsel(filter);
                return smsvarsler.Map(_mapper.Map<InnsynSmsVarselAm>).TilAm();
            }
        }
    }
}
