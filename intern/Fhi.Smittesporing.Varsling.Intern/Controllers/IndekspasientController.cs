using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Rapport;
using Fhi.Smittesporing.Varsling.Intern.Autorisering;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fhi.Smittesporing.Varsling.Intern.Controllers
{
    [Authorize(Policy = Tilgangsregler.Basic)]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class IndekspasientController : ControllerBase
    {

        private readonly IMediator _mediator;

        public IndekspasientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedListAm<IndekspasientMedAntallAm>>> HentListe([FromQuery] IndekspasientAm.Filter filter)
        {
            return await _mediator.Send(new HentListe.Query
            {
                Filter = filter,
                Brukernavn = User.Identity.Name
            });
        }

        [HttpGet("Rapport")]
        public async Task<ActionResult<IndekspasientRapportAm>> HentRapport([FromQuery] IndekspasientRapportAm.Filter filter)
        {
            return await _mediator.Send(new HentRapport.Query
            {
                Filter = filter
            });
        }

        [HttpGet("{indekspasientId}")]
        public async Task<ActionResult<IndekspasientMedAntallAm>> HentForId(int indekspasientId)
        {
            var result = await _mediator.Send(new HentForId.Query
            {
                IndekspasientId = indekspasientId
            });

            return result.Match<ActionResult<IndekspasientMedAntallAm>>(
                none: () => NotFound(),
                some: r => r
            );
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpGet("{indekspasientId}/Personopplysninger")]
        public async Task<ActionResult<IndekspasientPersonopplysningerAm>> HentPersonopplysningerForId(int indekspasientId)
        {
            var result = await _mediator.Send(new HentPersonopplysninger.Query
            {
                IndekspasientId = indekspasientId,
                Brukernavn = User.Identity.Name
            });

            return result.Match<ActionResult<IndekspasientPersonopplysningerAm>>(
                none: () => NotFound(),
                some: r => r
            );
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPost("{indekspasientId}/RegistrerTelefon")]
        public async Task<ActionResult> RegistrerTelefon(int indekspasientId, string telefonnummer, bool ikkeManueltFunnetKontaktInfo = false)
        {
            await _mediator.Send(new RegistrerTelefonnummer.Command
            {
                IndekspasientId = indekspasientId,
                Telefonnummer = telefonnummer,
                IkkeManueltFunnetKontaktInfo = ikkeManueltFunnetKontaktInfo
            });

            return NoContent();
        }

        [HttpGet("{indekspasientId}/Varslingssimulering")]
        public async Task<ActionResult<VarslingssimuleringAm>> HentVarslingssimulering(int indekspasientId)
        {
            return await _mediator.Send(new HentVarslingssimulering.Query
            {
                IndekspasientId = indekspasientId
            });
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPost("{indekspasientId}/SettVarslingFerdig")]
        public async Task<ActionResult<int>> SettVarslingFerdig(int indekspasientId)
        {
            await _mediator.Send(new SettVarslingFerdig.Command
            {
                IndekspasientId = indekspasientId
            });

            return NoContent();
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPost("{indekspasientId}/GodkjennVarsling")]
        public async Task<ActionResult<int>> GodkjennForVarsling(int indekspasientId)
        {
            await _mediator.Send(new GodkjennForVarsling.Command
            {
                IndekspasientIder = new [] {indekspasientId}
            });

            return NoContent();
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPost("GodkjennVarslingBatch")]
        public async Task<ActionResult<int>> GodkjennForVarslingBatch(int[] indekspasientIder)
        {
            await _mediator.Send(new GodkjennForVarsling.Command
            {
                IndekspasientIder = indekspasientIder
            });

            return NoContent();
        }
    }
}
