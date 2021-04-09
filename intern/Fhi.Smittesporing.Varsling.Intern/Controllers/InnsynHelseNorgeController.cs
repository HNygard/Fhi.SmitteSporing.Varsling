using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Fhi.Smittesporing.Varsling.Domene.InnsynsLogg;
using System.Linq;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using Microsoft.AspNetCore.Authorization;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using System;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
	[Authorize(Policy = Tilgangsregler.Innsyn)]
	[ApiController]
	[Route("api/[controller]")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class InnsynHelseNorgeController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ITelefonNormalFacade _telefonNormalFacade;

		public InnsynHelseNorgeController(IMediator mediator, ITelefonNormalFacade telefonNormalFacade)
		{
			_mediator = mediator;
			_telefonNormalFacade = telefonNormalFacade;
		}

		[HttpGet("")]
		public async Task<InnsynHelsenorgeAm> Hent([FromQuery]InnsynHelsenorgeRequestAm request)
		{
			await _mediator.Send(request.AsLoggCommand());

			var filter = new InnsynFilterAm
			{
				Telefonnummer = request.Telefonnummer,
				Fodselsnummer = request.Fodselsnummer,
				Sideindeks = 0,
				Sideantall = int.MaxValue
			};

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

			return InnsynExtensions.MapToHelsenorgeAm(
				_telefonNormalFacade.Normaliser(request.Telefonnummer),
				request.Fodselsnummer,
				indekspasienterRequest.Resultater,
				smittekontakterRequest.Resultater,
				smsvarselRequest.Resultater
				);
		}

		[HttpGet("logg")]
		public async Task<IEnumerable<InnsynLoggHelsenorgeAm>> HentLogg([FromQuery]InnsynHelsenorgeRequestAm request)
		{
			var filter = new InnsynFilterAm
			{
				Telefonnummer = request.Telefonnummer,
				Fodselsnummer = request.Fodselsnummer,
				Sideindeks = 0,
				Sideantall = int.MaxValue
			};

			var logg = await _mediator.Send(new HentInnsynLogg.Query
			{
				Filter = filter
			});

			//TODO: Legg inn logginnslag
			return logg.Resultater
				.ToHelsenorgeFormat()
				.EkskluderInnbygger(filter)
				.Pseudonymiser()
				.Reduce();
		}

		[HttpGet("hendelser")]
		public async Task<InnsynHendelserHelsenorgeAm> HentHendelser([FromQuery]InnsynHelsenorgeRequestAm request)
		{
			await _mediator.Send(request.AsLoggCommand());

			var filter = new InnsynFilterAm
			{
				Telefonnummer = request.Telefonnummer,
				Fodselsnummer = request.Fodselsnummer,
				Sideindeks = 0,
				Sideantall = int.MaxValue
			};

			var smittekontakter = await _mediator.Send(new HentInnsynSmittekontakter.Query
			{
				Filter = filter
			});
			var indekspasienter = await _mediator.Send(new HentInnsynIndekspasienter.Query
			{
				Filter = filter
			});
			var smsvarsel = await _mediator.Send(new HentInnsynSmsVarsel.Query
			{
				Filter = filter
			});

			var hendelser = new List<InnsynHendelseHelseNorgeAm>();

			hendelser.AddRange(smittekontakter.Resultater.Select(s => new InnsynHendelseHelseNorgeAm
			{
				Dato = s.Created,
				Hendelse = "Smittekontakt",
				Beskrivelse = GetBeskrivelse(s)
			}));

			hendelser.AddRange(indekspasienter.Resultater.Select(s => new InnsynHendelseHelseNorgeAm
			{
				Dato = s.Provedato ?? DateTime.Now, //TODO: Finn ut hvorfor dette skjer
				Hendelse = "Positiv prøve",
				Beskrivelse = GetBeskrivelse(s)
			}));

			hendelser.AddRange(smsvarsel.Resultater.Select(sms => new InnsynHendelseHelseNorgeAm
			{
				Dato = sms.Created,
				Hendelse = "Smsvarsel opprettet",
				Beskrivelse = GetBeskrivelse(sms)
			}));

			return new InnsynHendelserHelsenorgeAm
			{
				Telefonnummer = _telefonNormalFacade.Normaliser(request.Telefonnummer),
				Fodselsnummer = request.Fodselsnummer,
				Hendelser = hendelser.OrderBy(h => h.Dato)
			};
		}

		private string GetBeskrivelse(InnsynSmsVarselAm sms)
		{
			if(sms == null)
			{
				return null;
			}

			if(string.IsNullOrWhiteSpace(sms.Verifiseringskode))
			{
				return $"Status: '{sms.Status}' (sist oppdatert '{sms.SisteEksterneHendelsestidspunkt ?? sms.Created}').";
			}

			return $"Status: '{sms.Status}' (sist oppdatert '{sms.SisteEksterneHendelsestidspunkt ?? sms.Created}') med verifiseringskode '{sms.Verifiseringskode}'.";
		}

		private string GetBeskrivelse(InnsynSmittekontaktAm kontakt)
		{
			if (kontakt == null)
			{
				return null;
			}

			if(string.IsNullOrWhiteSpace(kontakt.Verifiseringskode))
			{
				return $"Potensiell smittekontakt klassifisert med risikokategori '{kontakt.Risikokategori}'.";
			}

			return $"Potensiell smittekontakt klassifisert med risikokategori '{kontakt.Risikokategori}' med verifiseringskode '{kontakt.Verifiseringskode}'.";
		}

		private string GetBeskrivelse(InnsynIndekspasientAm pasient)
		{
			if(pasient == null)
			{
				return string.Empty;
			}

			if(string.IsNullOrWhiteSpace(pasient.Kommune))
			{
				return $"Positiv smitteprøve avlagt.";
			}

			return $"Positiv smitteprøve avlagt i '{pasient.Kommune}'.";
		}
	}
}