using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn
{
    public class InnsynHelsenorgeRequestAm
    {
        [Required]
        public string Telefonnummer { get; set; }
        [Required]
        public string Fodselsnummer { get; set; }
        public string AktorFodselsnummer { get; set; }
        public string AktorName { get; set; }
    }
}
