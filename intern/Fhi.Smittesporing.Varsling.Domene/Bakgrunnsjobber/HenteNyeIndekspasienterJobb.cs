using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Indekspasienter;
using Fhi.Smittesporing.Varsling.Domene.Konstanter;
using Fhi.Smittesporing.Varsling.Domene.Modeller.Msis;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Domene.Bakgrunnsjobber
{
    public class HenteNyeIndekspasienterJobb : IPeriodiskJobb
    {
        private readonly Konfig _konfig;
        private readonly ILogger<HenteNyeIndekspasienterJobb> _logger;
        private readonly IMediator _mediator;
        private readonly IApplikasjonsinnstillingRepository _appInnstillingRepository;

        public HenteNyeIndekspasienterJobb(IOptions<Konfig> konfig, ILogger<HenteNyeIndekspasienterJobb> logger, IMediator mediator, IApplikasjonsinnstillingRepository appInnstillingRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _appInnstillingRepository = appInnstillingRepository;
            _konfig = konfig.Value;
        }

        public async Task<bool> UtforJobb(CancellationToken stoppingToken)
        {
            //step 0: finn startpunkt for gjeldende sync
            // - Last inn state fra tidligere sync
            var sisteBehandletTidspunkt =
                await _appInnstillingRepository.HentInnstilling<DateTime>(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt);
            var sisteBehandletHash =
                await _appInnstillingRepository.HentInnstilling<string>(Applikasjonsinnstillinger.SisteBehandletMsisHash);

            // - Angi startdato for gjeldende sync
            var fraDato = sisteBehandletTidspunkt
                .ValueOr(() =>
                {
                    var fraMaksBakover = DateTime.Now - _konfig.MaksTidBakover;
                    return fraMaksBakover > _konfig.StartHentingFra
                        ? fraMaksBakover
                        : _konfig.StartHentingFra;
                });

            _logger.LogDebug($"HentFraMsis smittetilfeller fra Dato: {fraDato}");

            //step 1 : hent nye smittetilfeller fra MSIS
            var msisTilfeller = (await _mediator.Send(new HentFraMsis.Query
            {
                FraDato = fraDato
            }, stoppingToken)).ToList();

            _logger.LogDebug($"Antall indekspasienter fra MSIS: {msisTilfeller.Count}");

            var antallBehandlet = 0;
            var antallOpprettet = 0;
            var forrigeSyncIkkeFunnetSlutten = sisteBehandletTidspunkt.HasValue &&
                                               sisteBehandletHash.Filter(x => !string.IsNullOrEmpty(x)).HasValue;
            var sammenlignMedHash = sisteBehandletHash.ValueOr(string.Empty);
            var scanVidereTilSisteBehandletFunnet = sisteBehandletTidspunkt.HasValue;
            //step 2 Registrert alle nye indekspasienter i db
            foreach (var sm in msisTilfeller)
            {
                var tilfelleHash = HashMsisTilfelle(sm);
                var tilfelleOpprettetTidspunkt = sm.Opprettettidspunkt;
                if (scanVidereTilSisteBehandletFunnet)
                {
                    // Duplikatsjekk
                    // - Vi får tilfeller fom dato vi spør etter og vi får dermed med siste behandlete tilfelle på nytt
                    // - Siste behandlete tilfelle ligger ikke nødvendigvis lagret i dette systemet, men vi holder på hash
                    //   til siste behandlet for å håndtere at flere tilfeller har samme OpprettetTidspunkt
                    if (tilfelleOpprettetTidspunkt > fraDato)
                    {
                        // Nytt opprettet tidspunkt -> avslutt duplikatsjekk
                        scanVidereTilSisteBehandletFunnet = false;
                    }
                    else if(tilfelleHash == sammenlignMedHash)
                    {
                        // Siste behandlet funnet, hopp til neste, men avslutt duplikatsjekk
                        scanVidereTilSisteBehandletFunnet = false;
                        continue;
                    }
                    else
                    {
                        // Siste behandlet er lenger ut i listen, hopp til neste
                        continue;
                    }
                }

                antallBehandlet++;

                var bleOpprettet = await _mediator.Send(new ForsokOpprett.Command
                {
                    Fodselsnummer = sm.Fodselsnummer,
                    Opprettettidspunkt = sm.Opprettettidspunkt,
                    Provedato = sm.Provedato,
                    Bostedkommunenummer = sm.Bostedkommunenummer,
                    Bostedkommune = sm.Bostedkommune
                }, stoppingToken);

                if (bleOpprettet)
                {
                    antallOpprettet++;
                }

                // Lagre MSIS-sync state
                await _appInnstillingRepository.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisHash, tilfelleHash);
                await _appInnstillingRepository.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt, tilfelleOpprettetTidspunkt);
            }

            if (antallBehandlet > 0)
            {
                _logger.LogInformation($"Antall MSIS-tilfeller behandlet {antallBehandlet}. Antall nye Indekspasient opprettet: {antallOpprettet}");
            }
            else if (forrigeSyncIkkeFunnetSlutten)
            {
                // Lagre MSIS-sync state akkurat forbi forrige sync for å unngå å hente siste tilfelle på nytt hver gang
                await _appInnstillingRepository.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisHash, string.Empty);
                await _appInnstillingRepository.SettInnstilling(Applikasjonsinnstillinger.SisteBehandletMsisTidspunkt, fraDato.AddMilliseconds(1));
            }

            // Arbeid utført så lenge minst ett tilfelle behandlet (uavhengig av hvor mange som ble opprettet)
            return antallBehandlet > 0;
        }

        public class Konfig : IPeriodiskJobbKonfig
        {
            public JobbIntervallKonfig JobbIntervaller { get; set; }
            public TimeSpan MaksTidBakover { get; set; } = TimeSpan.FromDays(30);
            public DateTime StartHentingFra { get; set; }
        }

        public static string HashMsisTilfelle(MsisSmittetilfelle msisTilfelle)
        {
            string s = $"{msisTilfelle.Opprettettidspunkt}{msisTilfelle.Provedato.GetValueOrDefault()}{msisTilfelle.Fodselsnummer}";
            return Hash(s);
        }

        private static string Hash(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2"))).Substring(0, 50);
            }
        }
    }
}