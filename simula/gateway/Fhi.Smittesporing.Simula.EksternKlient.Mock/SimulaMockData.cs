using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;
using Optional;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaMockData
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);
        private readonly Dictionary<string, SimulaContactReportBuilder> _testData = new Dictionary<string, SimulaContactReportBuilder>
        {
            {"93280061", Scenario1Kari},
            {"92806500", Scenario2Pernille},
            {"94723524", Scenario3Bjorn},
            {"90180876", Scenario4Per},
            {"92015790", Scenario5Ola}
        };

        private static readonly SimulaContactReportBuilder Scenario1Kari = new SimulaContactReportBuilder()
            .WithPhoneNumber("93280061")
            .WithContact(c => c.WithPhoneNumber("94723524").WithHighRiskScore().WithLongDuration())
            .WithContact(c => c.WithPhoneNumber("90180876").WithHighRiskScore().WithLongDuration())
            .WithContact(c => c.WithPhoneNumber("92015790").WithHighRiskScore().WithLongDuration());

        private static readonly SimulaContactReportBuilder Scenario2Pernille = new SimulaContactReportBuilder()
            .WithPhoneNumber("92806500").InSystem();

        private static readonly SimulaContactReportBuilder Scenario3Bjorn = new SimulaContactReportBuilder()
            .WithPhoneNumber("94723524")
            .InSystem().WithNoContacts();

        private static readonly SimulaContactReportBuilder Scenario4Per = new SimulaContactReportBuilder()
            .WithPhoneNumber("90180876")
            .InSystem().WithNoContacts();

        private static readonly SimulaContactReportBuilder Scenario5Ola = new SimulaContactReportBuilder()
            .WithPhoneNumber("92015790")
            .InSystem().WithNoContacts();

        public Option<SimulaContactReport> LookupMockdata(string phoneNumber)
        {
            var variants = PhoneFormatVariants(phoneNumber);

            foreach (var variant in variants)
            {
                if (_testData.ContainsKey(variant))
                {
                    return _testData[variant].Build().Some();
                }
            }

            return Option.None<SimulaContactReport>();
        }

        public Option<SimulaContactReport> CreateRandomResponse(string phoneNumber)
        {
            if (Rand.NextDouble() > 0.25)
            {
                return new SimulaContactReportBuilder().WithPhoneNumber(phoneNumber).Build().Some();
            }
            return Option.None<SimulaContactReport>();
        }

        private IEnumerable<string> PhoneFormatVariants(string phoneNumber)
        {
            var norwegianRegex = new Regex(@"^(\+47|0047|)(\d{8}$)");
            var match = norwegianRegex.Match(phoneNumber);
            if (match.Success)
            {
                var baseNumber = match.Groups[2].Value;
                return new[] { "+47" + baseNumber, "0047" + baseNumber, baseNumber };
            }

            return new[] { phoneNumber };
        }
    }
}