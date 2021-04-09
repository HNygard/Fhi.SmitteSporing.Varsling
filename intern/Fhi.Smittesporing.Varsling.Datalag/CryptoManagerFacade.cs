
using Fhi.Smittesporing.Varsling.Datalag.Cryptography;
using Fhi.Smittesporing.Varsling.Domene.Grensesnitt;
using System.Text;
using Microsoft.Extensions.Options;

namespace Fhi.Smittesporing.Varsling.Datalag
{
    /// <summary>
    /// En forenkelt facde til RijndaelCryptoManager (som implementert i FHI.Felles.Cryptography)
    /// lokal kopi i Fhi.Smittesporing.Varsling.Datalag.Cryptography
    /// Basert på .Net implementasjon: System.Security.Cryptography;
    /// </summary>
    public sealed class CryptoManagerFacade: ICryptoManagerFacade
    {
        private readonly RijndaelCryptoManager _cryptoManager;
        private readonly Encoding _encoding;
        private readonly IInnsynloggRespository _innsynloggRespositry;

        /// <summary>
        /// Kan brukes med 
        ///   - (dev/test)  lokale test-nokkel ( embedded i Resources)
        ///   - (stage/prod)Key/Vector verdier i filer, bekyttet med ProtectedData (installeres med passord og applikasjonskonto)
        /// </summary>
        public CryptoManagerFacade(IInnsynloggRespository innsynloggRespositry, IOptions<Konfig> konfig)
        {
            var keyPath = konfig.Value.EncryptionKeyPath;
            var vectorPath = konfig.Value.EncryptionVectorPath;

            var useEmbeddedTestKey = konfig.Value.UseEmbeddedTestKey;

            _cryptoManager = useEmbeddedTestKey ? new RijndaelCryptoManager() : new RijndaelCryptoManager(keyPath, vectorPath);
            _encoding = Encoding.GetEncoding("ISO-8859-1");
            _innsynloggRespositry = innsynloggRespositry;
        }

        public byte[] KrypterUtenBrukerinnsyn(byte[] data)
        {
            return _cryptoManager.EncryptRijndael(data);
        }

        public byte[] DekrypterUtenBrukerinnsyn(byte[] data)
        {
            return _cryptoManager.DecryptRijndael(data);
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
            var kryptertTekst = _cryptoManager.EncryptRijndaelToHexString(klartekst, _encoding);

            //logger bruk av dekryptering
            _ = _innsynloggRespositry.OpprettOgLagre(new Domene.Modeller.Innsyn.Innsynlogg.Innsynlogg()
            {
                Felt = felt,
                Hva = kryptertTekst,
                Hvorfor = hvorfor,
                Hvem = hvem,
            }).Result;

            return kryptertTekst;
        }

        public string DekrypterForBruker(string kryptertTekst, string felt, string hvorfor, string hvem)
        {
            //logger bruk av dekryptering
            _ = _innsynloggRespositry.OpprettOgLagre(new Domene.Modeller.Innsyn.Innsynlogg.Innsynlogg()
            {
                Felt = felt,
                Hva = kryptertTekst,
                Hvorfor = hvorfor,
                Hvem = hvem,
            }).Result;

            return _cryptoManager.DecryptRijndaelFromHexString(kryptertTekst, _encoding);
        }

        public string DekrypterDataTilknyttet(string kryptertTekst, string tilknyttetKryptertTekst, string felt, string hvorfor, string hvem)
        {
            //logger bruk av dekryptering
            _ = _innsynloggRespositry.OpprettOgLagre(new Domene.Modeller.Innsyn.Innsynlogg.Innsynlogg
            {
                Felt = felt,
                Hva = tilknyttetKryptertTekst,
                Hvorfor = hvorfor,
                Hvem = hvem,
            }).Result;

            return _cryptoManager.DecryptRijndaelFromHexString(kryptertTekst, _encoding);
        }

        public byte[] DekrypterDataTilknyttet(byte[] kryptertData, string tilknyttetKryptertTekst, string felt, string hvorfor, string hvem)
        {
            //logger bruk av dekryptering
            _ = _innsynloggRespositry.OpprettOgLagre(new Domene.Modeller.Innsyn.Innsynlogg.Innsynlogg()
            {
                Felt = felt,
                Hva = tilknyttetKryptertTekst,
                Hvorfor = hvorfor,
                Hvem = hvem,
            }).Result;

            return _cryptoManager.DecryptRijndael(kryptertData);
        }



        public class Konfig
        {
            public bool UseEmbeddedTestKey { get; set; }
            public string EncryptionKeyPath { get; set; }
            public string EncryptionVectorPath { get; set; }
        }

    }
}
