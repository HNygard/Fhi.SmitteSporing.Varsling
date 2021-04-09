using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.InternKlient;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using MoreLinq;
using Optional;
using Optional.Async.Extensions;

namespace Fhi.Smittesporing.Varsling.Eksternetjenester
{
    public class SimulaFacade : ISimulaFacade
    {
        private readonly ISimulaInternKlient _simulaKlient;
        private readonly ILogger<SimulaFacade> _logger;
        private readonly Konfig _konfig;

        public SimulaFacade(ISimulaInternKlient simulaInternKlient, ILogger<SimulaFacade> logger, IOptions<Konfig> konfig)
        {
            _simulaKlient = simulaInternKlient;
            _logger = logger;
            _konfig = konfig.Value;
        }

        public async Task<Option<SimulaKontaktrapport>> GetSmittekontakter(string telefonnummer,
            DateTime fraTidspunkt, DateTime tilTidspunkt)
        {
            try
            {
                var opprettetId = await _simulaKlient.OpprettKontaktrapport(new SimulaKontaktrapport.OpprettCommand
                {
                    Telefonnummer = telefonnummer,
                    FraTidspunkt = fraTidspunkt,
                    TilTidspunkt = tilTidspunkt
                });

                return await opprettetId.MapAsync(
                    async id =>
                    {
                        var startTid = DateTime.Now;
                        var antallForsok = 0;
                        var rapportErFerdig = false;
                        SimulaKontaktrapport kontaktRapport = null;
                        while (antallForsok < _konfig.MaksAntallForsok && !rapportErFerdig)
                        {
                            await Task.Delay(_konfig.RapportHentingPause);
                            kontaktRapport = (await _simulaKlient.HentKontaktrapport(id))
                                .ValueOr(() => throw new Exception("Simula-rapport ble slettet før ferdig versjon kunne hentes - ID: " + id));
                            antallForsok++;
                            rapportErFerdig = kontaktRapport.Ferdig;
                        }

                        if (!rapportErFerdig)
                        {
                            throw new Exception(
                                $"Klarte ikke hente ferdig rapport innenfor tillatt tid (Maks forsøk: {_konfig.MaksAntallForsok}, tid brukt: {DateTime.Now - startTid}, Request-ID: {id})");
                        }

                        return kontaktRapport;
                    }
                );
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, $"SimulaFacade Exception {exp.Message} ");
                 throw; 
            }
        }

        public async Task<SimulaApiInfoAm> GetSimulaApiInfo()
        {
            string proxyVersjon;
            string gatewayVersjon;

            try
            {
                proxyVersjon = (await _simulaKlient.HentProxyVersjon()).AssemblyVersjon;
            }
            catch
            {
                proxyVersjon = "Utilgjengelig";
            }

            try
            {
                gatewayVersjon = (await _simulaKlient.HentVersjon()).AssemblyVersjon;
            }
            catch
            {
                gatewayVersjon = "Utilgjengelig";
            }

            return new SimulaApiInfoAm
            {
                GatewayVersjon = gatewayVersjon,
                ProxyVersjon = proxyVersjon
            };
        }

        public async Task<bool> SjekkFinnes(string telefonnummer)
        {
            var slettetMatch = await _simulaKlient.HentSlettinger(new []{telefonnummer});

            // Hvis telefonen ikke oppgis som slettet finnes den i systemet
            return !slettetMatch.Any();
        }

        public async Task<IEnumerable<string>> SjekkSlettinger(IEnumerable<string> telefonnummerListe)
        {
            var slettedeTlfer = new List<string>();
            foreach (var tlfBatch in telefonnummerListe.Batch(1000))
            {
                slettedeTlfer.AddRange(await _simulaKlient.HentSlettinger(tlfBatch));
            }
            return slettedeTlfer;
        }

        public Task<PagedListAm<SimulaGpsData>> HentGpsData(SimulaGpsData.HentCommand command)
        {
            return _simulaKlient.HentGpsData(command);
        }

        public Task<PagedListAm<SimulaDataBruk>> HentLoggOverBruk(SimulaDataBruk.HentCommand command)
        {
            return _simulaKlient.HentLoggOverBruk(command);
        }

        public class Konfig
        {
            public int MaksAntallForsok { get; set; } = 100;
            public TimeSpan RapportHentingPause { get; set; } = TimeSpan.FromMinutes(1);
        }
    }
}
