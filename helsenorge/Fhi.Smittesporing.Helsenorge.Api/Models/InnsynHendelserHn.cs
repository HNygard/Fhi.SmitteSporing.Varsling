using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fhi.Smittesporing.Helsenorge.Api.Models
{
    public class InnsynHendelserHn
    {
        public string Telefonnummer { get; set; }
        public string Fodselsnummer { get; set; }
        public IEnumerable<InnsynHendelseHn> Hendelser { get; set; }
    }

    public class InnsynHendelseHn
    {
        public DateTime Dato { get; set; }
        public string Hendelse { get; set; }
        public string Beskrivelse { get; set; }
    }
}
