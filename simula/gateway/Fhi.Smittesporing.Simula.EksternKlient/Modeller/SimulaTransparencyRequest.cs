using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    public class SimulaTransparencyRequest : SimulaPagedRequest
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }


        [JsonProperty(PropertyName = "person_id")]
        public string PersonId { get; set; }


        [JsonProperty(PropertyName = "person_name")]
        public string PersonName { get; set; }


        [JsonProperty(PropertyName = "person_organization")]
        public string PersonOrganization { get; set; }


        [JsonProperty(PropertyName = "legal_means")]
        public string LegalMeans { get; set; }
    }
}