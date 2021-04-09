using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Simula.Applikasjonsmodell
{
    public class SimulaKontaktrapport
    {
        public bool Ferdig { get; set; }
        public string StatusMelding { get; set; }
        public string Telefonnummer { get; set; }
        public DateTime? SistAktivTidspunkt { get; set; }
        public List<SimulaKontakt> Kontakter { get; set; }

        public class OpprettCommand
        {
            [Required]
            [RegularExpression(@"^\+?\d{5,15}$")]
            public string Telefonnummer { get; set; }
            [Required]
            public DateTime FraTidspunkt { get; set; }
            [Required]
            public DateTime TilTidspunkt { get; set; }
        }

        public class OpprettResult
        {
            public Guid Id { get; set; }
        }
    }
}