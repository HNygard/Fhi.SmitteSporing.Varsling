namespace Fhi.Smittesporing.Varsling.Domene.Grensesnitt
{
    public interface ICryptoManagerFacade
    {
        string KrypterUtenBrukerinnsyn(string klartekst);
        string DekrypterUtenBrukerinnsyn(string kryptertTekst);
        byte[] KrypterUtenBrukerinnsyn(byte[] data);
        byte[] DekrypterUtenBrukerinnsyn(byte[] data);

        string KrypterForBruker(string klartekst, string felt, string hvorfor, string hvem);
        string DekrypterForBruker(string kryptertTekst, string felt, string hvorfor, string hvem);

        string DekrypterDataTilknyttet(string kryptertTekst, string tilknyttetKryptertTekst, string felt, string hvorfor, string hvem);
        byte[] DekrypterDataTilknyttet(byte[] kryptertData, string tilknyttetKryptertTekst, string felt, string hvorfor, string hvem);
    }
}
