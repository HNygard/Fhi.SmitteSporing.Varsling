using System;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    public class SimulaGpsDataEgressRequest : SimulaTransparencyRequest
    {
        [JsonProperty(PropertyName = "time_from")]
        public DateTime? TimeFrom { get; set; }

        [JsonProperty(PropertyName = "time_to")]
        public DateTime? TimeTo { get; set; }
    }
}