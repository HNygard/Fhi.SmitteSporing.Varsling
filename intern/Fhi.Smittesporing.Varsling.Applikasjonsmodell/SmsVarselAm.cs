using System;
using System.Collections.Generic;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell
{
    public class SmsVarselAm
    {
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public Guid? Referanse { get; set; }
        public DateTime? SistOppdatert { get; set; }
        public DateTime? SisteEksterneHendelsestidspunkt { get; set; }
        public List<SmsStatusoppdateringAm> Oppdateringer { get; set; }
    }
}