using System;
using System.Collections.Generic;
using System.Linq;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaContactReportBuilder
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        private string _phoneNumber;
        private bool? _inSystem;
        private List<SimulaContactBuilder> _contacts;

        public SimulaContactReport Build()
        {
            var inSystem = _inSystem ?? Rand.NextDouble() > 0.25; // Antar høy dekningsgrad :)
            var phoneNumber = _phoneNumber ?? "+47" + Rand.Next(90000000, 99999999);
            var contacts = _contacts?.Select(x => x.Build()) ??
                           Enumerable.Range(0, inSystem ? Rand.Next(1, 20) : 0).Select(_ => new SimulaContactBuilder().Build());
            return new SimulaContactReport
            {
                PhoneNumber = phoneNumber,
                FoundInSystem = inSystem,
                LastActivity = DateTime.Now.AddMinutes(-Rand.Next(2, 240)),
                Contacts = new []{contacts.ToDictionary(
                    x => x.Item1,
                    x => x.Item2
                )}
            };
        }

        public SimulaContactReportBuilder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        public SimulaContactReportBuilder InSystem(bool inSystem = true)
        {
            _inSystem = inSystem;
            return this;
        }

        public SimulaContactReportBuilder WithContact(Action<SimulaContactBuilder> contactSetup = null)
        {
            _inSystem = true;
            _contacts ??= new List<SimulaContactBuilder>();
            var contactBuilder = new SimulaContactBuilder();
            contactSetup?.Invoke(contactBuilder);
            _contacts.Add(contactBuilder);
            return this;
        }

        public SimulaContactReportBuilder WithNoContacts()
        {
            _inSystem = true;
            _contacts ??= new List<SimulaContactBuilder>();
            return this;
        }
    }
}