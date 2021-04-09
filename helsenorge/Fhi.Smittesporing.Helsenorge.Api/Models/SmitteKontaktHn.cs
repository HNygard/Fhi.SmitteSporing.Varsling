using System;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
    public class SmitteKontaktHn
    {
        public DateTime Dato { get; set; }
        public DateTime? Varslet { get; set; }
        public string Risikokategori { get; set; }
        public string Verifiseringskode { get; set; }
    }
}