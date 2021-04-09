using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fhi.Smittesporing.Simula.Applikasjonsmodell;
using Fhi.Smittesporing.Simula.EksternKlient;
using Fhi.Smittesporing.Simula.InternApi.Requests;
using MediatR;
using Optional;

namespace Fhi.Smittesporing.Simula.GatewayServer.Handlers
{
    public class HentKontaktrapportHandler : IRequestHandler<HentKontaktrapportQuery, Option<SimulaKontaktrapport>>
    {
        private readonly ISimulaEksternApiKlient _simulaKlient;

        public HentKontaktrapportHandler(ISimulaEksternApiKlient simulaKlient)
        {
            _simulaKlient = simulaKlient;
        }

        public async Task<Option<SimulaKontaktrapport>> Handle(HentKontaktrapportQuery request, CancellationToken cancellationToken)
        {
            var simulaApiResponse = await _simulaKlient.HentKontaktresultat(request.Id);

            return simulaApiResponse.Map(response => response.Match(
                none: r => new SimulaKontaktrapport
                {
                    Ferdig = false,
                    StatusMelding = r.Message
                },
                some: r => new SimulaKontaktrapport
                {
                    Ferdig = true,
                    Telefonnummer = r.PhoneNumber,
                    SistAktivTidspunkt = r.LastActivity,
                    Kontakter = r.Contacts?.SelectMany(x => x.Select(c => new SimulaKontakt
                    {
                        Telefonnummer = c.Key,
                        Verifiseringskode = c.Value.PinCode,
                        Versjonsinfo = new SimulaKontaktVersjonsinfo
                        {
                            Pipeline = c.Value.VersionInfo.Pipeline,
                            Enhet = c.Value.VersionInfo.Device
                        },
                        Oppsummering = new SimulaKontaktOppsummering
                        {
                            AllKontakt = new SimulaKontaktOppsummering.All
                            {
                                AntallKontakter = c.Value.Cumulative.AllContacts.NumberOfContacts,
                                AntallDagerMedKontakt = c.Value.Cumulative.AllContacts.DaysInContact,
                                Interessepunkter = c.Value.Cumulative.AllContacts.PointsOfInterest,
                                Risikokategori = c.Value.Cumulative.AllContacts.RiskCat,
                                SoylePlotBase64Png = c.Value.Cumulative.AllContacts.BarPlot
                            },
                            BluetoothKontakt = new SimulaKontaktOppsummering.BluetoothInfo
                            {
                                AkkumulertVarighet = c.Value.Cumulative.BtContacts.CumulativeDuration,
                                AkkumulertRisikoscore = c.Value.Cumulative.BtContacts.CumulativeRiskScore,
                                NarVarighet = c.Value.Cumulative.BtContacts.BtCloseDuration,
                                RelativtNarVarighet = c.Value.Cumulative.BtContacts.BtRelativelyCloseDuration,
                                VeldigNarVarighet = c.Value.Cumulative.BtContacts.BtVeryCloseDuration,
                                AntallDagerMedKontakt = c.Value.Cumulative.BtContacts.DaysInContact
                            },
                            GpsKontakt = new SimulaKontaktOppsummering.GpsInfo
                            {
                                AkkumulertVarighet = c.Value.Cumulative.GpsContacts.CumulativeDuration,
                                AkkumulertRisikoscore = c.Value.Cumulative.GpsContacts.CumulativeRiskScore,
                                AntallDagerMedKontakt = c.Value.Cumulative.GpsContacts.DaysInContact,
                                HistogramBase64Png = c.Value.Cumulative.GpsContacts.HistPlot
                            }
                        },
                        Detaljer = c.Value.Daily?.Select(d => new SimulaKontaktdetaljer
                        {
                            Dato = DateTime.ParseExact(d.Key, "yyyy-MM-dd", CultureInfo.CurrentCulture),
                            AllKontakt = new SimulaKontaktdetaljer.All
                            {
                                OppsummertPlotHtml = d.Value.AllContacts.SummaryPlot,
                                Interessepunkter = d.Value.AllContacts.PointsOfInterest
                            },
                            GpsKontakt = new SimulaKontaktdetaljer.PerType
                            {
                                AkkumulertVarighet = d.Value.GpsContacts.CumulativeDuration,
                                AkkumulertRisiko = d.Value.GpsContacts.CumulativeRiskScore,
                                Medianavstand = d.Value.GpsContacts.MedianDistance
                            },
                            BluetoothKontakt = new SimulaKontaktdetaljer.BluetoothInfo
                            {
                                AkkumulertVarighet = d.Value.BtContacts.CumulativeDuration,
                                AkkumulertRisiko = d.Value.BtContacts.CumulativeRiskScore,
                                Medianavstand = d.Value.BtContacts.MedianDistance,
                                NarVarighet = c.Value.Cumulative.BtContacts.BtCloseDuration,
                                RelativtNarVarighet = c.Value.Cumulative.BtContacts.BtRelativelyCloseDuration,
                                VeldigNarVarighet = c.Value.Cumulative.BtContacts.BtVeryCloseDuration
                            }
                        }).ToArray() ?? new SimulaKontaktdetaljer[0]
                    })).ToList() ?? new List<SimulaKontakt>()
                }
            ));
        }
    }
}