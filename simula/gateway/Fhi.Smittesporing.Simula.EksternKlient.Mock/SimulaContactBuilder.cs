using System;
using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaContactBuilder
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        private Option<string> _phoneNumber;
        private Option<bool> _highRisk;
        private Option<bool> _longDuration;

        public (string, SimulaContact) Build()
        {
            var highRisk = _highRisk.ValueOr(() => Rand.NextDouble() > 0.5);
            var longDuration = _longDuration.ValueOr(() => Rand.NextDouble() > 0.5);
            var contactsDaily = Enumerable.Range(0, Rand.Next(1, 20))
                .Select(i => (DateTime.Now.AddDays(-i), new SimulaContactDailyInfoBuilder().WithHighRish(highRisk).WithLongDuration(longDuration).Build()))
                .ToArray();
            return (
                _phoneNumber.ValueOr(() => "+47" + Rand.Next(90000000, 99999999)),
                new SimulaContact
                {
                    PinCode = Rand.Next(1000, 10000).ToString(),
                    Cumulative = new SimulaContactCumulativeInfoBuilder()
                        .WithDailyContacts(contactsDaily.Select(x => x.Item2))
                        .Build(),
                    VersionInfo = new SimulaContactVersionInfo
                    {
                        Pipeline = "0.0.0.1-alpha-rc-523212-develop",
                        Device = new List<string> { "Nokia 3210" }
                    },
                    Daily = contactsDaily.ToDictionary(
                        x => x.Item1.ToString("yyyy-MM-dd"),
                        x => x.Item2)
                });
        }

        public SimulaContactBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber.SomeNotNull();
            return this;
        }

        public SimulaContactBuilder WithHighRiskScore(bool highRisk = true)
        {
            _highRisk = highRisk.Some();
            return this;
        }

        public SimulaContactBuilder WithLongDuration(bool longDuration = true)
        {
            _longDuration = longDuration.Some();
            return this;
        }
    }
}