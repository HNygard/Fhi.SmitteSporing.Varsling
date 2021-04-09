using System;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {
    ///   "phone_number": "+4798765432",
    ///   "time_from": "2020-04-22T06:05:38.611319Z",
    ///   "time_to": "2020-04-22T06:05:38.611319Z"
    /// }
    /// </summary>
    public class SimulaStartContactRequest
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "time_from")]
        public DateTime TimeFrom { get; set; }

        [JsonProperty(PropertyName = "time_to")]
        public DateTime TimeTo { get; set; }
    }
}