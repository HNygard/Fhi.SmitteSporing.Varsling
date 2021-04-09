using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using System.Linq;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
	[Authorize(Policy = Tilgangsregler.Innsyn)]
	[ApiController]
	[Route("api/[controller]")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class InnsynController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IEksporterInnsynService _eksportService;

		public InnsynController(IMediator mediator, IEksporterInnsynService eksportService)
		{
			_mediator = mediator;
			_eksportService = eksportService;
		}

		[HttpPost("logg")]
		public async Task<ActionResult> LoggSøk(InnsynFilterAm filter)
		{
			await _mediator.Send(new LoggSøk.Command
			{
				Fodselsnummer = filter.Fodselsnummer,
				Navn = User.Identity.Name,
				Formal = "Behandling av innsynsforespørsel."
			});

			return Ok();
		}

		[HttpGet("logg")]
		public async Task<ActionResult<PagedListAm<InnsynLoggAm>>> HentListe([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynLogg.Query
			{
				Filter = filter
			});
		}

		[HttpGet("smittekontakter")]
		public async Task<ActionResult<PagedListAm<InnsynSmittekontaktAm>>> HentSmittekontakter([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynSmittekontakter.Query
			{
				Filter = filter
			});
		}

		[HttpGet("indekspasienter")]
		public async Task<ActionResult<PagedListAm<InnsynIndekspasientAm>>> HentIndekspasienter([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynIndekspasienter.Query
			{
				Filter = filter
			});
		}

		[HttpGet("smsvarsler")]
		public async Task<ActionResult<PagedListAm<InnsynSmsVarselAm>>> HentIndeksVarsler([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynSmsVarsel.Query
			{
				Filter = filter
			});
		}

		[HttpGet("simuladatabruk")]
		public async Task<ActionResult<PagedListAm<InnsynSimulaDatabrukAm>>> HentSimulaDatabruk([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynSimulaDatabruk.Query
			{
				Filter = filter
			});

		}

		[HttpGet("simulagpsdata")]
		public async Task<ActionResult<PagedListAm<InnsynSimulaGpsDataAm>>> HentSimulaGpsData([FromQuery] InnsynFilterAm filter)
		{
			return await _mediator.Send(new HentInnsynSimulaGpsData.Query { Filter = filter });
		}

		[HttpGet("eksport")]
		public async Task<ActionResult> Export([FromQuery] string telefonnummer, [FromQuery] string fodselsnummer)
		{
			var filter = new InnsynFilterAm
			{
				Telefonnummer = telefonnummer,
				Fodselsnummer = fodselsnummer,
				Sideindeks = 0,
				Sideantall = int.MaxValue
			};

			var innsynsloggRequest = await _mediator.Send(new HentInnsynLogg.Query
			{
				Filter = filter
			});
			var smittekontakterRequest = await _mediator.Send(new HentInnsynSmittekontakter.Query
			{
				Filter = filter
			});
			var indekspasienterRequest = await _mediator.Send(new HentInnsynIndekspasienter.Query
			{
				Filter = filter
			});
			var smsvarselRequest = await _mediator.Send(new HentInnsynSmsVarsel.Query
			{
				Filter = filter
			});
			var simulafilter = new InnsynFilterAm
			{
				Telefonnummer = telefonnummer,
				Fodselsnummer = fodselsnummer,
				Sideindeks = 0,
				Sideantall = 1000
			};
			var simuladatabrukRequest = await _mediator.Send(new HentInnsynSimulaDatabruk.Query
			{
				Filter = simulafilter

			});

			var simulagpsdataRequest = await _mediator.Send(new HentInnsynSimulaGpsData.Query
			{
				Filter = simulafilter
			});
			List<InnsynSimulaGpsDataAm> simulaGpsDataListe = new List<InnsynSimulaGpsDataAm>();
			simulaGpsDataListe.AddRange(simulagpsdataRequest.Resultater);
			if (simulagpsdataRequest.TotaltAntall > simulaGpsDataListe.Count) {
				int sideantall = simulagpsdataRequest.Sideantall;
				int gpsSiderAntall = simulagpsdataRequest.TotaltAntall / sideantall + (simulagpsdataRequest.TotaltAntall % simulagpsdataRequest.Sideantall > 0 ? 1 : 0);
				for (int i = 1; i < gpsSiderAntall; i++) {
					var gpsFilter = new InnsynFilterAm
					{
						Telefonnummer = telefonnummer,
						Fodselsnummer = fodselsnummer,
						Sideindeks = i,
						Sideantall = sideantall
					};

					var eksportgpsdata = await _mediator.Send(new HentInnsynSimulaGpsData.Query
					{
						Filter = gpsFilter
					});

					simulaGpsDataListe.AddRange(eksportgpsdata.Resultater);
				}
			}
			List<InnsynSimulaDatabrukAm> simulaDatabrukListe = new List<InnsynSimulaDatabrukAm>();
			simulaDatabrukListe.AddRange(simuladatabrukRequest.Resultater);
			if(simuladatabrukRequest.TotaltAntall > simulaDatabrukListe.Count)
			{
				int sideantall = simuladatabrukRequest.Sideantall;
				int databrukSiderAntall = simuladatabrukRequest.TotaltAntall / sideantall + (simuladatabrukRequest.TotaltAntall % simuladatabrukRequest.Sideantall > 0 ? 1 : 0);
				for (int i = 1; i < databrukSiderAntall; i++) {
					var databrukFilter = new InnsynFilterAm { 
						Telefonnummer = telefonnummer,
						Fodselsnummer = fodselsnummer,
						Sideindeks = i,
						Sideantall = sideantall
					};
					var eksportdatabruk = await _mediator.Send(new HentInnsynSimulaDatabruk.Query { 
						Filter = databrukFilter
					});
					simulaDatabrukListe.AddRange(eksportdatabruk.Resultater);
				}
			}

			var data = InnsynExtensions.MapToHelsenorgeAm(
				telefonnummer,
				fodselsnummer,
				indekspasienterRequest.Resultater,
				smittekontakterRequest.Resultater,
				smsvarselRequest.Resultater
				);

			var bytes = await _eksportService.CreateFile(
				innsynsloggRequest.Resultater
					.ToHelsenorgeFormat()
					.EkskluderInnbygger(filter)
					.Pseudonymiser()
					.Reduce()
					.ToArray(),
				data,
				//simuladatabrukRequest.Resultater.ToArray(),
				simulaDatabrukListe.ToArray(),
				simulaGpsDataListe.ToArray()
				//simulagpsdataRequest.Resultater.ToArray()
				);

			return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Export-{telefonnummer}.xlsx");
		}
	}
}
