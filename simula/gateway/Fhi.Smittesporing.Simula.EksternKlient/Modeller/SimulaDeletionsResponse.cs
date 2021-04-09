using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {"deleted_phone_numbers": ["+4798765432"]}
    /// </summary>
    public class SimulaDeletionsResponse
    {
        [JsonProperty(PropertyName = "deleted_phone_numbers")]
        public IEnumerable<string> DeletedPhoneNumbers { get; set; }
    }
}