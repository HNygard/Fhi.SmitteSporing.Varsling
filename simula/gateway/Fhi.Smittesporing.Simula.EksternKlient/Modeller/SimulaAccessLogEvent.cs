using System;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {
    ///   "timestamp": "2020-04-12T12:40:20.907000Z",
    ///   "phone_number": "+4798765432",
    ///   "person_name": "Ola Nordmann",
    ///   "person_organization": "",
    ///   "person_id": "01014298765",
    ///   "technical_organization": "Norsk Helsenett",
    ///   "legal_means": "Innsynsoppslag fra helsenorge.no"
    /// }
    /// </summary>
    public class SimulaAccessLogEvent
    {
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "person_name")]
        public string PersonName { get; set; }

        [JsonProperty(PropertyName = "person_organization")]
        public string PersonOrganization { get; set; }

        [JsonProperty(PropertyName = "person_id")]
        public string PersonId { get; set; }

        [JsonProperty(PropertyName = "technical_organization")]
        public string TechnicalOrganization { get; set; }

        [JsonProperty(PropertyName = "legal_means")]
        public string LegalMeans { get; set; }
    }
}