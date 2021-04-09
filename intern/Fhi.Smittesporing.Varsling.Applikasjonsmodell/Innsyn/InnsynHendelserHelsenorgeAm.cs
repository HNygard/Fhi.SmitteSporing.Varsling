using System;
using System.Collections.Generic;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn
{
    public class InnsynHendelserHelsenorgeAm
    {
        public string Telefonnummer { get; set; }
        public string Fodselsnummer { get; set; }
        public IEnumerable<InnsynHendelseHelseNorgeAm> Hendelser { get; set; }
    }

    public class InnsynHendelseHelseNorgeAm
    {
        public DateTime Dato { get; set; }
        public string Hendelse { get; set; }
        public string Beskrivelse { get; set; }
    }
}
