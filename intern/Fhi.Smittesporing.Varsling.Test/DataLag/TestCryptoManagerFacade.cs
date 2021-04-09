using System.Text;
using Fhi.Smittesporing.Varsling.Datalag.Cryptography;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;

namespace Fhi.Smittesporing.Varsling.Test.DataLag
{
    //kun for test
    public sealed class TestCryptoManagerFacade : ICryptoManagerFacade
    {
        private readonly RijndaelCryptoManager _cryptoManager;
        private readonly Encoding _encoding;

        //kun for test
        public TestCryptoManagerFacade()
        {
                _cryptoManager = new RijndaelCryptoManager();
                _encoding = Encoding.GetEncoding("ISO-8859-1");
        }

   
        public string KrypterUtenBrukerinnsyn(string klartekst)
        {
            return _cryptoManager.EncryptRijndaelToHexString(klartekst, _encoding);
        }

        public string DekrypterUtenBrukerinnsyn(string kryptertTekst)
        {
            return _cryptoManager.DecryptRijndaelFromHexString(kryptertTekst, _encoding);
        }

        public string KrypterForBruker(string klartekst, string felt, string hvorfor, string hvem)
        {
            return _cryptoManager.EncryptRijndaelToHexString(klartekst, _encoding);
        }

        public byte[] KrypterUtenBrukerinnsyn(byte[] klartekstData)
        {
            return _cryptoManager.EncryptRijndael(klartekstData);
        }

        public byte[] DekrypterUtenBrukerinnsyn(byte[] kryptertData)
        {
            return _cryptoManager.DecryptRijndael(kryptertData);
        }

        public string DekrypterDataTilknyttet(string kryptertTekst, string tilknyttetKryptertTekst, string felt, string hvorfor,
            string hvem)
        {
            return _cryptoManager.DecryptRijndaelFromHexString(kryptertTekst, _encoding);
        }

        public byte[] DekrypterDataTilknyttet(byte[] kryptertData, string tilknyttetKryptertTekst, string felt, string hvorfor,
            string hvem)
        {
            return _cryptoManager.DecryptRijndael(kryptertData);
        }

        public string DekrypterForBruker(string kryptertTekst, string felt, string hvorfor, string hvem)
        {
            return _cryptoManager.DecryptRijndaelFromHexString(kryptertTekst, _encoding);
        }

        public byte[] Dekrypter(byte[] kryptertData)
        {
            return _cryptoManager.DecryptRijndael(kryptertData);
        }
    }
}
