using System;
using System.Collections.Generic;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Sms
{
    public class SmsUtsending
    {
        /// <summary>
        /// Mobiltelefonnummer varsel sendes til
        /// </summary>
        public string Telefonnummer { get; set; }

        /// <summary>
        /// Referanse for å koble SMS-hendelser (sendt/levert/feilet) til spesifikt varsel
        /// </summary>
        public Guid Referanse { get; set; }

        /// <summary>
        /// Flettedata til SMS som sendes
        /// </summary>
        public Dictionary<string, string> Flettedata { get; set; }
    }
}