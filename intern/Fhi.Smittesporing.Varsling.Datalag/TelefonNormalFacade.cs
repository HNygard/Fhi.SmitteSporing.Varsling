using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using System.Linq;
using Optional;

namespace Fhi.Smittesporing.Varsling.Datalag
{
    public class TelefonNormalFacade : ITelefonNormalFacade
    {
        public Option<string, string> NormaliserStrict(string telefonnummer)
        {
            telefonnummer = Normaliser(telefonnummer);

            if (string.IsNullOrEmpty(telefonnummer))
                return Option.None<string, string>("Tomt telefonnummer");

            return telefonnummer.StartsWith("+")
                ? telefonnummer.Some<string, string>()
                : Option.None<string, string>("Ugyldig telefonnummer");
        }

        public string Normaliser(string telefonnummer)
        {
            if (string.IsNullOrEmpty(telefonnummer))
                return telefonnummer;

            // Fjerner mellomrom, bindestreker, etc
            telefonnummer = KunGyldigeTegn(telefonnummer);

            // Normaliser landkodeprefiks til +
            if (telefonnummer.StartsWith("+"))
            {
                // Return as is
                return telefonnummer;
            }
            if (telefonnummer.StartsWith("00"))
            {
                // Bytt 00 med +
                return "+" + telefonnummer.Substring(2);
            }
            if (telefonnummer.Length == 8)
            {
                // Antar norsk nummer hvis 8 siffer
                return "+47" + telefonnummer;
            }

            return telefonnummer;
        }


        private string KunGyldigeTegn(string telefonnummer)
        {
            var telefonnummerKunSiffer = new string(telefonnummer.ToCharArray().Where(c => char.IsDigit(c) || c == '+').ToArray());
            return telefonnummerKunSiffer;
        }
    }
}
