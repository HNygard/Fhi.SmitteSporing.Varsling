using System;
using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional.Collections;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaContactCumulativeInfoBuilder
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        private IEnumerable<SimulaContactDailyInfo> _dailyContacts;

        private static string[] RiskCats = { "low", "medium", "high" };

        public SimulaContactCumulativeInfo Build()
        {
            var dailyContacts = _dailyContacts ?? Enumerable
                                    .Range(0, Rand.Next(1, 10))
                                    .Select(i => new SimulaContactDailyInfoBuilder().Build());

            var numContacts = 0;
            var btDuration = 0.0;
            var btRisk = 0.0;
            var gpsDuration = 0.0;
            var gpsRisk = 0.0;
            var pois = new Dictionary<string, double>();

            var antallDager = 0;
            foreach (var daily in dailyContacts)
            {
                numContacts++;
                btDuration += daily.BtContacts.CumulativeDuration;
                btRisk += daily.BtContacts.CumulativeRiskScore;
                gpsDuration += daily.GpsContacts.CumulativeDuration;
                gpsRisk += daily.GpsContacts.CumulativeRiskScore;
                foreach (var poi in daily.AllContacts.PointsOfInterest)
                {
                    pois[poi.Key] = pois.GetValueOrNone(poi.Key).ValueOr(0.0) + poi.Value;
                }

                antallDager++;
            }

            return new SimulaContactCumulativeInfo
            {
                AllContacts = new SimulaContactCumulativeInfo.All
                {
                    RiskCat = RiskCats[Math.Min((int)(btRisk + gpsRisk) / 3000, RiskCats.Length-1)],
                    NumberOfContacts = numContacts,
                    BarPlot = SimulaGraphMocks.BarPlot,
                    PointsOfInterest = pois,
                    DaysInContact = antallDager
                },
                BtContacts = new SimulaContactCumulativeInfo.BtInfo
                {
                    CumulativeDuration = btDuration,
                    CumulativeRiskScore = btRisk,
                    BtCloseDuration = Rand.NextDouble() * 60,
                    BtVeryCloseDuration = Rand.NextDouble() * 60,
                    BtRelativelyCloseDuration = Rand.NextDouble() * 60,
                    DaysInContact = antallDager
                },
                GpsContacts = new SimulaContactCumulativeInfo.GpsInfo
                {
                    CumulativeDuration = gpsDuration,
                    CumulativeRiskScore = gpsRisk,
                    DaysInContact = antallDager,
                    HistPlot = SimulaGraphMocks.BarPlot // TODO: replace mock
                }
            };
        }

        public SimulaContactCumulativeInfoBuilder WithDailyContacts(IEnumerable<SimulaContactDailyInfo> dailyContacts)
        {
            _dailyContacts = dailyContacts;
            return this;
        }
    }
}