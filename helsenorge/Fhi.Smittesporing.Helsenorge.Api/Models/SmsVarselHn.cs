using System;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
    public class SmsVarselHn
    {
        public DateTime Tidspunkt { get; set; }
        public string Status { get; set; }
        public string Kode { get; set; }
        public string Verifiseringskode { get; set; }
    }
}