using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Simula.Applikasjonsmodell
{
    public class Innsynshenvendelse
    {
        [Required]
        public string TilknyttetTelefonnummer { get; set; }
        [Required]
        public string PersonNavn { get; set; }
        [Required]
        public string PersonOrganisasjon { get; set; }
        [Required]
        public string PersonIdentifikator { get; set; }
        [Required]
        public string TekniskOrganisasjon { get; set; }
        [Required]
        public string RettsligFormal { get; set; }
    }
}