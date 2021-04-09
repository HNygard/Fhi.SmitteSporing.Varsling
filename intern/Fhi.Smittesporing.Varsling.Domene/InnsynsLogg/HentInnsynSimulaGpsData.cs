using AutoMapper;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Varsling.Domene.InnsynsLogg
{
	public class HentInnsynSimulaGpsData
	{
		public class Query : IRequest<PagedListAm<InnsynSimulaGpsDataAm>>
		{
			public InnsynFilterAm Filter { get; set; }
		}

		public class Handler : IRequestHandler<Query, PagedListAm<InnsynSimulaGpsDataAm>>
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

			public async Task<PagedListAm<InnsynSimulaGpsDataAm>> Handle(Query request, CancellationToken cancellationToken)
			{
				var gpsFra = new DateTime(2020, 4, 1);
				var gpsTil = DateTime.Now;
				var kommando = new Simula.Applikasjonsmodell.SimulaGpsData.HentCommand
				{
					FraTidspunkt = gpsFra,
					TilTidspunkt = gpsTil,
					Sideantall = request.Filter.Sideantall ?? 100,
					Sideindeks = request.Filter.Sideindeks ?? 0,
					TilknyttetTelefonnummer = _telefonManager.Normaliser(request.Filter.Telefonnummer),
					PersonIdentifikator = request.Filter.Fodselsnummer,
					PersonNavn = "Personvernsoffiser",
					PersonOrganisasjon = "FHI",
					TekniskOrganisasjon = "FHI",
					RettsligFormal = "Manuelt innsyn hos FHI"
				};

				var gpsdata = await _facade.HentGpsData(kommando);

				return new PagedListAm<InnsynSimulaGpsDataAm>()
				{
					Sideindeks = gpsdata.Sideindeks,
					Sideantall = gpsdata.Sideantall,
					TotaltAntall = gpsdata.TotaltAntall,
					AntallSider = gpsdata.TotaltAntall / gpsdata.Sideantall + (gpsdata.TotaltAntall % gpsdata.Sideantall > 0 ? 1 : 0),
					Resultater = gpsdata.Resultater.Select(d => _mapper.Map<InnsynSimulaGpsDataAm>(d))
				};
			}
		}
	}
}
