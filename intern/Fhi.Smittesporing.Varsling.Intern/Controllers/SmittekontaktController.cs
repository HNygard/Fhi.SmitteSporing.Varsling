using System.Collections.Generic;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Smittekontakter;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
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
    public class SmittekontaktController : ControllerBase
    {

        private readonly IMediator _mediator;

        public SmittekontaktController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedListAm<SmittekontaktListemodellAm>>> HentListe([FromQuery] SmittekontaktAm.Filter filter)
        {
            return await _mediator.Send(new HentListe.Query
            {
               Filter = filter
            });
        }

        [HttpGet("{smittekontaktId}")]
        public async Task<ActionResult<SmittekontaktAm>> HentDetaljertModell(int smittekontaktId)
        {
            var kanskjeSmittekontakt = await _mediator.Send(new HentDetaljertModell.Query
            {
                SmittekontaktId = smittekontaktId
            });

            return kanskjeSmittekontakt
                .Map(x => (ActionResult<SmittekontaktAm>) x)
                .ValueOr(() => NotFound());
        }

        [HttpGet("{smittekontaktId}/OppsummertKontaktGraf")]
        public async Task<ActionResult> HentOppsummertKontaktGraf(int smittekontaktId)
        {
            var result = await _mediator.Send(new HentOppsummertKontaktGraf.Query
            {
                SmittekontaktId = smittekontaktId,
                Brukernavn = User.Identity.Name
            });

            return result.Match<ActionResult>(
                none: NotFound,
                some: f => File(f.Bytes, f.MimeType, f.Filnavn));
        }

        [HttpGet("{smittekontaktId}/GpsHistogram")]
        public async Task<ActionResult> HentGpsHistogram(int smittekontaktId)
        {
            var result = await _mediator.Send(new HentGpsHistogram.Query
            {
                SmittekontaktId = smittekontaktId,
                Brukernavn = User.Identity.Name
            });

            return result.Match<ActionResult>(
                none: NotFound,
                some: f => File(f.Bytes, f.MimeType, f.Filnavn));
        }

        [HttpGet("{smittekontaktId}/Dager/{smittekontaktDagDetaljerId}/KartHtml")]
        public async Task<ActionResult> HentKartForDagSomHtml(int smittekontaktId, int smittekontaktDagDetaljerId)
        {
            var result = await _mediator.Send(new HentDagDetaljerKartSomHtml.Query
            {
                SmittekontaktId = smittekontaktId,
                SmittekontaktDagDetaljerId = smittekontaktDagDetaljerId,
                Brukernavn = User.Identity.Name
            });

            return result.Match<ActionResult>(
                none: NotFound,
                some: html => Content(html, "text/html"));
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpGet("{smittekontaktId}/Personopplysninger")]
        public async Task<ActionResult<SmittekontaktPersonopplysningerAm>> HentPersonopplysninger(int smittekontaktId)
        {
            var result = await _mediator.Send(new HentPersonopplysninger.Query
            {
                SmittekontaktId = smittekontaktId,
                Brukernavn = User.Identity.Name
            });

            return result.Match<ActionResult<SmittekontaktPersonopplysningerAm>>(
                none: () => NotFound(),
                some: r => r
            );
        }

        /// <summary>
        /// Kun for manipulasjon av data ifm test/verifikasjon
        /// </summary>
        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPut("{smittekontaktId}/Personopplysninger")]
        public async Task<ActionResult> OppdaterPersonopplysninger(int smittekontaktId, SmittekontaktPersonopplysningerAm personopplysninger)
        {
            await _mediator.Send(new OppdaterPersonopplysninger.Command
            {
                SmittekontaktId = smittekontaktId,
                Personopplysninger = personopplysninger
            });

            return NoContent();
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpPost("{smittekontaktId}/SendVarsel")]
        public async Task<ActionResult> SendVarsel(int smittekontaktId)
        {
            await _mediator.Send(new SendVarsel.Command
            {
                SmittekontaktId = smittekontaktId
            });

            return NoContent();
        }

        [Authorize(Policy = Tilgangsregler.Smittesporer)]
        [HttpGet("{smittekontaktId}/SmsVarsler")]
        public async Task<ActionResult<List<SmsVarselAm>>> HentVarselinfo(int smittekontaktId)
        {
            var varsler = await _mediator.Send(new HentVarselinfo.Query
            {
                SmittekontaktId = smittekontaktId
            });

            return varsler.Match<ActionResult<List<SmsVarselAm>>>(
                none: () => NotFound(),
                some: r => r
            );
        }
    }
}
