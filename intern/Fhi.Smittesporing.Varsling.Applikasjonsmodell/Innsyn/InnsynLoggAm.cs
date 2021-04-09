using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn
{
    public class InnsynLoggAm
    {
        public DateTime Created { get; set; }

        public string Hva { get; set; }
        public string Hvorfor { get; set; }
        public string Hvem { get; set; }
        public string Felt { get; set; }
    }
}
