using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using Fhi.Smittesporing.Varsling.Domene.Modeller;
using MediatR;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async.Extensions;
using Optional.Collections;

namespace Fhi.Smittesporing.Varsling.Domene.Indekspasienter
{
    public class LastInnSmittekontakter
    {
        public class Command : IRequest<int>
        {
            public int MaksAntallDagerBakover { get; set; } = 14;
            public int AntallDagerForProvedato { get; set; } = 7;
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly IIndekspasientRepository _indekspasientRepository;
            private readonly ISmittekontaktRespository _smittekontaktRespository;
            private readonly ITelefonRespository _telefonRespository;
            private readonly ISimulaFacade _simulaFacade;
            private readonly ITelefonNormalFacade _telefonManager;
            private readonly ICryptoManagerFacade _cryptoManager;
            private readonly ILogger<Handler> _logger;

            public Handler(IIndekspasientRepository indekspasientRepository, ISmittekontaktRespository smittekontaktRespository, ISimulaFacade simulaFacade, ITelefonRespository telefonRespository, ICryptoManagerFacade cryptoManager, ITelefonNormalFacade telefonManager, ILogger<Handler> logger)
            {
                _indekspasientRepository = indekspasientRepository;
                _smittekontaktRespository = smittekontaktRespository;
                _simulaFacade = simulaFacade;
                _telefonRespository = telefonRespository;
                _cryptoManager = cryptoManager;
                _telefonManager = telefonManager;
                _logger = logger;
            }

            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var indekspasienter = await _indekspasientRepository.HentIndekspasienterTilKontaktsjekk();
                var eldsteTillatteDatoForKontaktsjekk = DateTime.Today.AddDays(-request.MaksAntallDagerBakover);

                foreach (var indekspasient in indekspasienter)
                {
                    _logger.LogInformation("Starter henting av kontakrapport for indekspasient #" + indekspasient.IndekspasientId);

                    // Henter kontakter relativt til prøvedato
                    // - gammel prøvedato -> ikke hent kontakter
                    if (indekspasient.Provedato == null || indekspasient.Provedato.Value < eldsteTillatteDatoForKontaktsjekk)
                    {
                        // For gammelt koronatilfelle -> slett indekspasient
                        await SlettDataForIndekspasient(indekspasient);
                        continue;
                    }

                    // Fra og med 7 dager før prøvedato (men ikke lenger tilbake enn maks)
                    var fraTidspunkt = indekspasient.Provedato.Value.Date.AddDays(-request.AntallDagerForProvedato);
                    if (fraTidspunkt < eldsteTillatteDatoForKontaktsjekk)
                    {
                        fraTidspunkt = eldsteTillatteDatoForKontaktsjekk;
                    }

                    // Til og med hele prøvedato
                    var fremTilTidspunkt = indekspasient.Provedato.Value.Date.AddDays(1);

                    var dekryptertPasientTlf =
                        _cryptoManager.DekrypterUtenBrukerinnsyn(indekspasient.Telefon.Telefonnummer);
                    var simulaResponse = await _simulaFacade.GetSmittekontakter(dekryptertPasientTlf, fraTidspunkt, fremTilTidspunkt);

                    await simulaResponse.MatchAsync(
                        none: () => SlettDataForIndekspasient(indekspasient),
                        some: r => LagreSimulaResultatForPasient(indekspasient, r)
                    );
                }

                return indekspasienter.Count;
            }

            /// <summary>
            /// Hvis pasient ikke finnes i simula skal alle data for pasienten slettes,
            /// Men vi beholder metadata for å ha sporbarhet på spørringer vi har kjørt mot simula
            /// </summary>
            private Task SlettDataForIndekspasient(Indekspasient indekspasient)
            {
                indekspasient.SlettData();
                return _smittekontaktRespository.Lagre();
            }

            /// <summary>
            /// Pasient finnes i simula sitt system. Beholdes i systemet med enten status
            /// SmitteKontakt eller IkkeSmitteKontakt avhengig av innhold i rapporten
            /// </summary>
            private async Task LagreSimulaResultatForPasient(Indekspasient indekspasient, SimulaKontaktrapport rapport)
            {
                var simulaKontakter = await rapport
                        .SomeWhen(r => r.Kontakter?.Any() ?? false)
                        .Map(r => r.Kontakter)
                        .MapAsync(async kontakter =>
                        {
                            var smittekontaktListe = new List<Smittekontakt>(kontakter.Count);
                            // Ved flere enheter for samme person kan samme TLF-nummer oppstå flere
                            // ganger i samme rapport, så vi må tracke duplikater som er funnet innad i rapporten også
                            var innlastetTlfMap = new Dictionary<string, Telefon>();
                            foreach (var kontakt in kontakter)
                            {
                                var normTlf = _telefonManager.Normaliser(kontakt.Telefonnummer);
                                var kryptertTlfNummer = _cryptoManager.KrypterUtenBrukerinnsyn(normTlf);
                                var telefon = await innlastetTlfMap
                                    .GetValueOrNone(kryptertTlfNummer)
                                    .MatchAsync(
                                        none: async () =>
                                        {
                                            var tlf = (await _telefonRespository.FinnForTelefonnummer(kryptertTlfNummer))
                                                .ValueOr(() => new Telefon
                                                {
                                                    Telefonnummer = kryptertTlfNummer
                                                });
                                            innlastetTlfMap.Add(kryptertTlfNummer, tlf);
                                            return tlf;
                                        },
                                        some: Task.FromResult);

                                smittekontaktListe.Add(
                                    new Smittekontakt
                                    {
                                        Indekspasient = indekspasient,
                                        Telefon = telefon,
                                        Verifiseringskode = kontakt.Verifiseringskode,
                                        Interessepunkter = kontakt.Oppsummering.AllKontakt.Interessepunkter,
                                        AntallKontakter = kontakt.Oppsummering.AllKontakt.AntallKontakter,
                                        AntallDagerMedKontakt = kontakt.Oppsummering.AllKontakt.AntallDagerMedKontakt,
                                        Risikokategori = kontakt.Oppsummering.AllKontakt.Risikokategori,
                                        SoyleDiagram = kontakt.Oppsummering.AllKontakt.SoylePlotBase64Png != null ? new SmittekontaktDiagram
                                        {
                                            Data = _cryptoManager.KrypterUtenBrukerinnsyn(
                                                Convert.FromBase64String(kontakt.Oppsummering.AllKontakt.SoylePlotBase64Png)),
                                        } : null,
                                        GpsHistogram = kontakt.Oppsummering.GpsKontakt.HistogramBase64Png != null ? new SmittekontaktGpsHistogram
                                        {
                                            Data = _cryptoManager.KrypterUtenBrukerinnsyn(
                                                Convert.FromBase64String(kontakt.Oppsummering.GpsKontakt.HistogramBase64Png)),
                                        } : null,
                                        BluetoothAkkumulertRisikoscore = kontakt.Oppsummering.BluetoothKontakt.AkkumulertRisikoscore,
                                        BluetoothAkkumulertVarighet = kontakt.Oppsummering.BluetoothKontakt.AkkumulertVarighet,
                                        BluetoothNarVarighet = kontakt.Oppsummering.BluetoothKontakt.NarVarighet,
                                        BluetoothRelativtNarVarighet = kontakt.Oppsummering.BluetoothKontakt.RelativtNarVarighet,
                                        BluetoothVeldigNarVarighet = kontakt.Oppsummering.BluetoothKontakt.VeldigNarVarighet,
                                        BluetoothAntallDagerMedKontakt = kontakt.Oppsummering.BluetoothKontakt.AntallDagerMedKontakt,
                                        GpsAkkumulertRisikoscore = kontakt.Oppsummering.GpsKontakt.AkkumulertRisikoscore,
                                        GpsAkkumulertVarighet = kontakt.Oppsummering.GpsKontakt.AkkumulertVarighet,
                                        GpsAntallDagerMedKontakt = kontakt.Oppsummering.GpsKontakt.AntallDagerMedKontakt,

                                        PipelineVersjon = kontakt.Versjonsinfo.Pipeline,
                                        Enhetsinfo = kontakt.Versjonsinfo.Enhet,

                                        Detaljer = kontakt.Detaljer.Select(d => new SmittekontaktDetaljer
                                        {
                                            Dato = d.Dato,
                                            OppsummertPlotDetaljerHtml = d.AllKontakt.OppsummertPlotHtml != null ? new SmittekontaktDetaljerHtmlKart
                                            {
                                                Innhold = _cryptoManager.KrypterUtenBrukerinnsyn(d.AllKontakt.OppsummertPlotHtml),
                                            } : null,
                                            Interessepunkter = d.AllKontakt.Interessepunkter,
                                            GpsAkkumulertRisiko = d.GpsKontakt.AkkumulertRisiko,
                                            GpsAkkumulertVarighet = d.GpsKontakt.AkkumulertVarighet,
                                            GpsMedianavstand = d.GpsKontakt.Medianavstand,
                                            BluetoothAkkumulertRisiko = d.BluetoothKontakt.AkkumulertRisiko,
                                            BluetoothAkkumulertVarighet = d.BluetoothKontakt.AkkumulertVarighet,
                                            BluetoothMedianavstand = d.BluetoothKontakt.Medianavstand,
                                            BluetoothNarVarighet = d.BluetoothKontakt.NarVarighet,
                                            BluetoothRelativtNarVarighet = d.BluetoothKontakt.RelativtNarVarighet,
                                            BluetoothVeldigNarVarighet = d.BluetoothKontakt.VeldigNarVarighet,
                                        }).ToList()
                                    });
                            }
                            return smittekontaktListe;
                        });

                await simulaKontakter.MatchAsync(
                    none: () => _indekspasientRepository.OppdaterStatus(indekspasient.IndekspasientId, IndekspasientStatus.IkkeSmitteKontakt),
                    some: async kontakter =>
                    {
                        await _smittekontaktRespository.Opprett(kontakter);
                        await _indekspasientRepository.OppdaterStatus(indekspasient.IndekspasientId,
                            IndekspasientStatus.SmitteKontakt);
                    }
                );
            }
        }
    }
}