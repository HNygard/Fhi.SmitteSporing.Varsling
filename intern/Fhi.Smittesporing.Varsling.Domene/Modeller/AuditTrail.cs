using System;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller
{
    public class AuditTrail
    {
        public int AuditTrailId { get; set; }
        public string Modell { get; set; }
        public string PrimarNokkel { get; set; }
        public string Egenskap { get; set; }
        public string NyVerdi { get; set; }
        public string GammelVerdi { get; set; }
        public DateTime Tidspunkt { get; set; }
        public string UtfortAv { get; set; }
    }

    public class AuditTrailOptionsAttribute : Attribute
    {
        public bool SensitivVerdi { get; set; }
        public bool BinarVerdi { get; set; }

        public string HentLoggverdi(Func<string> verdiFn)
        {
            if (SensitivVerdi)
            {
                return "<Sensitiv>";
            }
            if (BinarVerdi)
            {
                return "<Binær>";
            }
            return verdiFn();
        }

        public static AuditTrailOptionsAttribute Options => new AuditTrailOptionsAttribute
        {
            SensitivVerdi = false,
            BinarVerdi = false
        };
    }
}