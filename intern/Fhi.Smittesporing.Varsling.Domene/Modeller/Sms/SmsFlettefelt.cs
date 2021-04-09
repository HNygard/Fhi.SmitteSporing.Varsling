using System;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Sms
{
    public class SmsFlettefelt
    {
        private readonly Func<Smittekontakt, SmsFletteinnstillinger, string> _valueFn;

        public SmsFlettefelt(string navn, string beskrivelse, string eksempelverdi, Func<Smittekontakt, SmsFletteinnstillinger, string> valueFn)
        {
            _valueFn = valueFn;
            Navn = navn;
            Beskrivelse = beskrivelse;
            Eksempelverdi = eksempelverdi;
        }

        public string Navn { get; }
        public string Placeholder => "{" + Navn + "}";
        public string Beskrivelse { get; }
        public string Eksempelverdi { get; }

        public string HentVerdi(Smittekontakt smittekontakt, SmsFletteinnstillinger innstillinger)
        {
            return _valueFn(smittekontakt, innstillinger);
        }
    }
}