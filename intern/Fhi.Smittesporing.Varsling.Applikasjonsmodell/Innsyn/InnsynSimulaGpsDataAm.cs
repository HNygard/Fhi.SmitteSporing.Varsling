using System;
using System.Collections.Generic;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn
{
	public class InnsynSimulaGpsDataAm
	{
        public DateTime FraTidspunkt { get; set; }
        public DateTime TilTidspunkt { get; set; }
        public double Breddegrad { get; set; }
        public double Lengdegrad { get; set; }
        public double Noyaktighet { get; set; }
        public double Hastighet { get; set; }
        public double Hoyde { get; set; }
        public double HoydeNoyaktighet { get; set; }
    }
}
