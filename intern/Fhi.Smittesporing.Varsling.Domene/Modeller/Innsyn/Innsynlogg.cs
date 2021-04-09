using System;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Innsyn.Innsynlogg
{
    public class Innsynlogg
    {

        public int InnsynloggId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public string Hva { get; set; }
        public string Hvorfor { get; set; }
        public string Hvem { get; set; }
        public string Felt { get; set; }

    }
}
