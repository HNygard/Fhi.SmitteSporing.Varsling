namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface IArbeidskontekst
    {
        void SettBrukerkontekst(string brukernavn);
        void SettAnonymkontekst();
        void SettSystemjobbkontekst<T>();
        string HentNavn();
    }
}