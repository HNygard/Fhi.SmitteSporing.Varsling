using System;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Simula.Applikasjonsmodell
{
    public class SimulaDataBruk
    {
        public DateTime Tidspunkt { get; set; }

        public string TilknyttetTelefonnummer { get; set; }

        public string PersonNavn { get; set; }

        public string PersonOrganisasjon { get; set; }

        public string PersonIdentifikator { get; set; }

        public string TekniskOrganisasjon { get; set; }

        public string RettsligFormal { get; set; }

        public class HentCommand : Innsynshenvendelse
        {
            [Required]
            [Range(0, int.MaxValue)]
            public int Sideindeks { get; set; }
            [Required]
            [Range(1, 1000)]
            public int Sideantall { get; set; }
        }
    }
}