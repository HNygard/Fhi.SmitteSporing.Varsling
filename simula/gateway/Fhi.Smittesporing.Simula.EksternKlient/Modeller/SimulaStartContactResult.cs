using System;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {"request_id": "00763dc4-086a-431f-8e80-bfdf1943e023", "result_url": "https://api-smittestopp-dev.azure-api.net/fhi/lookup/00763dc4-086a-431f-8e80-bfdf1943e023"}
    /// </summary>
    public class SimulaStartContactResult
    {
        [JsonProperty(PropertyName = "request_id")]
        public Guid RequestId { get; set; }
        [JsonProperty(PropertyName = "result_url")]
        public string ResultUrl { get; set; }
    }
}