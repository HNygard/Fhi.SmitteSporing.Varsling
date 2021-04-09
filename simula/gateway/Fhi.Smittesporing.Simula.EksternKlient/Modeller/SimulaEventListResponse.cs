using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {
    ///   "phone_number": "+4798765432",
    ///   "found_in_system": true,
    ///   "events": [
    ///     { ...TEvent }
    ///   ],
    ///   "total": 438,
    ///   "per_page": 2,
    ///   "page_number": 10,
    ///   "next": {
    ///     "page_number": 11,
    ///     "per_page": 2
    ///   }
    /// }
    /// 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class SimulaEventListResponse<TEvent>
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "found_in_system")]
        public bool FoundInSystem { get; set; }

        [JsonProperty(PropertyName = "events")]
        public IEnumerable<TEvent> Events { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "page_number")]
        public int PageNumber { get; set; }

        [JsonProperty(PropertyName = "next")]
        public SimulaPagedRequest Next { get; set; }
    }
}
