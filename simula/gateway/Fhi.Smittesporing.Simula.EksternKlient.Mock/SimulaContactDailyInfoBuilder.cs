using System;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaContactDailyInfoBuilder
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);
        private const double HighRiskThreshold = 9000;
        private const double LongDurationThreshold = 15 * 60;

        private Option<bool> _highRisk;
        private Option<bool> _longDuration;

        public SimulaContactDailyInfo Build()
        {
            var highRisk = _highRisk.ValueOr(() => Rand.NextDouble() > 0.5);
            var risk = highRisk
                ? HighRiskThreshold * (1 + Rand.NextDouble())
                : HighRiskThreshold * Rand.NextDouble();
            var riskBt = risk * Rand.NextDouble();
            var riskGps = risk - riskBt;

            var longDuration = _longDuration.ValueOr(() => Rand.NextDouble() > 0.5);
            var duration = longDuration
                ? LongDurationThreshold * (1 + Rand.NextDouble())
                : LongDurationThreshold * Rand.NextDouble();
            var durationBt = duration * Rand.NextDouble();
            var durationGps = duration - durationBt;

            return new SimulaContactDailyInfo
            {
                AllContacts = new SimulaContactDailyInfo.All
                {
                    SummaryPlot = SimulaGraphMocks.MapHtml,
                    PointsOfInterest = new SimulaPointsOfInterestBuilder().Build(),
                },
                GpsContacts = new SimulaContactDailyInfo.PerType
                {
                    CumulativeDuration = durationGps,
                    CumulativeRiskScore = riskGps,
                    MedianDistance = null
                },
                BtContacts = new SimulaContactDailyInfo.BtInfo
                {
                    CumulativeDuration = durationBt,
                    CumulativeRiskScore = riskBt,
                    MedianDistance = Rand.NextDouble() * 50,
                    BtCloseDuration = Rand.NextDouble() * 60,
                    BtVeryCloseDuration = Rand.NextDouble() * 60,
                    BtRelativelyCloseDuration = Rand.NextDouble() * 60
                }
            };
        }

        public SimulaContactDailyInfoBuilder WithHighRish(bool highRisk = true)
        {
            _highRisk = highRisk.Some();
            return this;
        }
        public SimulaContactDailyInfoBuilder WithLongDuration(bool longduration = true)
        {
            _longDuration = longduration.Some();
            return this;
        }
    }
}