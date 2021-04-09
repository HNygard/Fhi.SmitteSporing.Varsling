using System;
using System.ComponentModel.DataAnnotations;

namespace Fhi.Smittesporing.Simula.Applikasjonsmodell
{
    public class SimulaGpsData
    {
        public DateTime FraTidspunkt { get; set; }

        public DateTime TilTidspunkt { get; set; }

        public double Breddegrad { get; set; }

        public double Lengdegrad { get; set; }

        public double Noyaktighet { get; set; }

        public double Hastighet { get; set; }

        public double Hoyde { get; set; }

        public double HoydeNoyaktighet { get; set; }

        public class HentCommand : Innsynshenvendelse
        {
            [Required]
            [Range(0, int.MaxValue)]
            public int Sideindeks { get; set; }
            [Required]
            [Range(1, 1000)]
            public int Sideantall { get; set; }
            public DateTime? FraTidspunkt { get; set; }
            public DateTime? TilTidspunkt { get; set; }
        }
    }
}