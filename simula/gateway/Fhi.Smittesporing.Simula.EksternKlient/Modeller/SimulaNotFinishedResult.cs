using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    public class SimulaNotFinishedResult
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}