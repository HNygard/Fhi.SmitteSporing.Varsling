using AutoMapper;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
    public class HentInnsynSimulaDatabruk
    {
        public class Query : IRequest<PagedListAm<InnsynSimulaDatabrukAm>>
        {
            public InnsynFilterAm Filter { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedListAm<InnsynSimulaDatabrukAm>>
        {
            private readonly ISimulaFacade _facade;
            private readonly ITelefonNormalFacade _telefonManager;
            private readonly IMapper _mapper;

            public Handler(ISimulaFacade repository, ITelefonNormalFacade telefonManager, IMapper mapper)
            {
                _facade = repository;
                _telefonManager = telefonManager;
                _mapper = mapper;
            }

            public async Task<PagedListAm<InnsynSimulaDatabrukAm>> Handle(Query request, CancellationToken cancellationToken)
            {
                var kommando = new Simula.Applikasjonsmodell.SimulaDataBruk.HentCommand
                {
                    Sideindeks = request.Filter.Sideindeks ?? 0,
                    Sideantall = request.Filter.Sideantall ?? 100,
                    PersonIdentifikator = request.Filter.Fodselsnummer,
                    TilknyttetTelefonnummer = _telefonManager.Normaliser(request.Filter.Telefonnummer),
                    PersonNavn = "Personvernsoffiser",
                    PersonOrganisasjon = "FHI",
                    TekniskOrganisasjon = "FHI",
                    RettsligFormal = "Manuelt innsyn hos FHI"
                };

                var databruk = await _facade.HentLoggOverBruk(kommando);

                return new PagedListAm<InnsynSimulaDatabrukAm>()
                {
                    Sideindeks = databruk.Sideindeks,
                    Sideantall = databruk.Sideantall,
                    TotaltAntall = databruk.TotaltAntall,
                    AntallSider = databruk.AntallSider,
                    Resultater = databruk.Resultater.Select(d => _mapper.Map<InnsynSimulaDatabrukAm>(d))
                };
            }
        }
    }
}
