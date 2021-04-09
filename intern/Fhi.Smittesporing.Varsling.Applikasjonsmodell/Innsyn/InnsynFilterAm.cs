using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn
{
    public class InnsynFilterAm
    {
        [Required]
        [RegularExpression(@"^\+?\d{5,15}$")]
        public string Telefonnummer { get; set; }
        [Required]
        [RegularExpression("[0-9]{11}")]
        public string Fodselsnummer { get; set; }
        public int? Sideindeks { get; set; }
        public int? Sideantall { get; set; }
    }
}
