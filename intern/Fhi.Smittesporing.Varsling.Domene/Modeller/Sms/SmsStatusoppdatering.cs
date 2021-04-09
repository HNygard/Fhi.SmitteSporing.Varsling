using System;
using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Sms
{
    public class SmsStatusoppdatering
    {
        public int Loepenummer { get; set; }
        public Option<Guid> SmsUtsendingReferanse { get; set; }
        public SmsStatus GjeldeneStatus { get; set; }
        public Option<int> AntallSegmenter { get; set; }
        public DateTime Tidspunkt { get; set; }
        public string Beskrivelse { get; set; }
    }
}