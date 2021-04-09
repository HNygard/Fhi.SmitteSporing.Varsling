using System;
using System.Linq;
using Fhi.Smittesporing.Simula.EksternKlient.Modeller;

namespace Fhi.Smittesporing.Simula.EksternKlient.Mock
{
    public class SimulaEventListResponseBuilder<TEvent>
    {
        private readonly Random _rand = new Random(DateTime.Now.Millisecond);

        private readonly SimulaTransparencyRequest _request;

        private readonly Func<int, int, TEvent> _eventFactory;

        private bool? _inSystem;
        private int? _numEvents;

        public SimulaEventListResponseBuilder(SimulaTransparencyRequest request, Func<int, int, TEvent> eventFactory)
        {
            _request = request;
            _eventFactory = eventFactory;
        }

        public SimulaEventListResponseBuilder<TEvent> WithEventsCount(int numEvents)
        {
            _numEvents = numEvents;
            return this;
        }

        public SimulaEventListResponse<TEvent> Build()
        {
            var inSystem = _inSystem ?? _rand.NextDouble() > 0.25; // Antar høy dekningsgrad :)

            int totalEvents;
            if (_numEvents != null)
            {
                totalEvents = _numEvents.Value;
            }
            else if(long.TryParse(_request.PhoneNumber.Replace("+", ""), out var phoneNumeric))
            {
                totalEvents = (int) (phoneNumeric % 1000);
            }
            else
            {
                totalEvents = _rand.Next(0, 1000);
            }

            var numSkipped = _request.PerPage * (_request.PageNumber - 1);// Simula bruker 1 index paging
            var numRemaining = Math.Max(0, totalEvents - numSkipped);
            var events = Enumerable.Range(numSkipped, Math.Min(numRemaining, _request.PerPage))
                .Select(i => _eventFactory(i, totalEvents));

            return new SimulaEventListResponse<TEvent>
            {
                PhoneNumber = _request.PhoneNumber,
                FoundInSystem = inSystem,
                Events = events,
                Total = totalEvents,
                PageNumber = _request.PageNumber,
                PerPage = _request.PerPage,
                Next = new SimulaPagedRequest
                {
                    PageNumber = _request.PageNumber + 1,
                    PerPage = _request.PerPage
                }
            };
        }

        public SimulaEventListResponseBuilder<TEvent> InSystem(bool inSystem = true)
        {
            _inSystem = inSystem;
            return this;
        }
    }

    public class SimulaGpsDataEgressEventBuilder
    {
        private readonly Random _rand = new Random(DateTime.Now.Millisecond);

        public SimulaGpsDataEgressEvent Build()
        {
            return new SimulaGpsDataEgressEvent
            {
                TimeTo = DateTime.Now.AddDays(-3),
                TimeFrom = DateTime.Now.AddDays(-3).AddMinutes(5),
                Accuracy = _rand.NextDouble(),
                Altitude = 2469 * _rand.NextDouble(),
                AltitudeAccuracy = _rand.NextDouble() * 15,
                Latitude = 60.391262 + _rand.NextDouble(),
                Longitude = 5.322054 + _rand.NextDouble(),
                Speed = 42 * _rand.NextDouble()
            };
        }
    }

    public class SimulaAccessLogEventBuilder
    {
        private string _phoneNumber;

        public SimulaAccessLogEvent Build()
        {
            return new SimulaAccessLogEvent
            {
                PhoneNumber = _phoneNumber ?? "+4798765432",
                PersonId = "25043012345",
                PersonName = "Ola Nordmann",
                LegalMeans = "Innsyn via mock-API",
                PersonOrganization = "Mock ASA",
                TechnicalOrganization = "Mocktec AS",
                Timestamp = DateTime.Now.AddDays(-3)
            };
        }

        public SimulaAccessLogEventBuilder ForPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }
    }
}