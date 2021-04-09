using System;
using System.Collections.Generic;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn
{
	public class InnsynSimulaDatabrukAm
	{
        public DateTime Tidspunkt { get; set; }

        public string TilknyttetTelefonnummer { get; set; }

        public string PersonNavn { get; set; }

        public string PersonOrganisasjon { get; set; }

        public string PersonIdentifikator { get; set; }

        public string TekniskOrganisasjon { get; set; }

        public string RettsligFormal { get; set; }
	}
}
