using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Domene.Utils
{
    public class Arbeidskontekst : IArbeidskontekst
    {
        private string _kontekstNavn;
        public void SettBrukerkontekst(string brukernavn)
        {
            _kontekstNavn = brukernavn;
        }

        public void SettAnonymkontekst()
        {
            _kontekstNavn = "Anonym";
        }

        public void SettSystemjobbkontekst<T>()
        {
            _kontekstNavn = typeof(T).Name;
        }

        public string HentNavn()
        {
            return _kontekstNavn;
        }
    }
}