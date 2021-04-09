using System;

namespace Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Sms
{
    public class SmsStatusoppdateringAm
    {
        public int Loepenummer { get; set; }
        public Guid? SmsUtsendingReferanse { get; set; }
        public string GjeldeneStatus { get; set; }
        public int? AntallSegmenter { get; set; }
        public DateTime Tidspunkt { get; set; }
        public string Beskrivelse { get; set; }
    }
}