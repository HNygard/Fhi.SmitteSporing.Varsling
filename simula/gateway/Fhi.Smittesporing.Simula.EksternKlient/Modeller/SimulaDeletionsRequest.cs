using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    public class SimulaDeletionsRequest
    {
        [JsonProperty(PropertyName = "phone_numbers")]
        public IEnumerable<string> PhoneNumbers { get; set; }
    }
}