using Optional;

namespace Fhi.Smittesporing.Varsling.Domene.Modeller.Sms
{
    public class SmsFletteinnstillinger
    {
        public string KommuneinfoFallback { get; set; } = "Kontakt din kommunelege ved spørsmål";
        public string LavRisikokategori { get; set; } = "Din kontakt var klassifisert som lav risiko.";
        public string MiddelsRisikokategori { get; set; } = "Din kontakt var klassifisert som middels risiko.";
        public string HoyRisikokategori { get; set; } = "Din kontakt var klassifisert som høy risiko.";

        public Option<string> TekstForRisiko(string risiko)
        {
            switch (risiko)
            {
                case "low":
                {
                    return LavRisikokategori.SomeNotNull();
                }
                case "medium":
                {
                    return MiddelsRisikokategori.SomeNotNull();
                }
                case "high":
                {
                    return HoyRisikokategori.SomeNotNull();
                }

                default:
                {
                    return Option.None<string>();
                }
            }
        }
    }
}
