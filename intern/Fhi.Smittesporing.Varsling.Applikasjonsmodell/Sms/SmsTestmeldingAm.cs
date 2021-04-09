using System;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms
{
    public class SmsTestmeldingAm
    {
        [Required]
        public string Telefonnummer { get; set; }
        [Required]
        public Guid Referanse { get; set; }
        public int? KommuneId { get; set; }
        public string Risikokategori { get; set; }
        public DateTime? DatoSisteKontakt { get; set; }
    }
}