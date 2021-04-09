using System;
using Newtonsoft.Json;

namespace Fhi.Smittesporing.Simula.EksternKlient.Modeller
{
    /// <summary>
    /// {
    ///   "time_from": "2020-04-07T10:54:11Z",
    ///   "time_to": "2020-04-07T10:54:35Z",
    ///   "latitude": 59.835908,
    ///   "longitude": 10.799224,
    ///   "accuracy": 4.936999797821045,
    ///   "speed": 1.1480000019073486,
    ///   "speed_accuracy": 0,
    ///   "altitude": 90.31900024414062,
    ///   "altitude_accuracy": 2.984999895095825
    /// }
    /// </summary>
    public class SimulaGpsDataEgressEvent
    {
        [JsonProperty(PropertyName = "time_from")]
        public DateTime TimeFrom { get; set; }
        
        [JsonProperty(PropertyName = "time_to")]
        public DateTime TimeTo { get; set; }
        
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }
        
        [JsonProperty(PropertyName = "accuracy")]
        public double Accuracy { get; set; }
        
        [JsonProperty(PropertyName = "speed")]
        public double Speed { get; set; }

        [JsonProperty(PropertyName = "altitude")]
        public double Altitude { get; set; }

        [JsonProperty(PropertyName = "altitude_accuracy")]
        public double AltitudeAccuracy { get; set; }
    }
}
