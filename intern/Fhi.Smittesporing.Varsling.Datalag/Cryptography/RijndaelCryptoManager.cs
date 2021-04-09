using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Fhi.Smittesporing.Varsling.Datalag.Cryptography
{

/* 
 * Kopi av kode fra FHI.Felles.Cryptography prosjekt  
 * Kode  Unit testet i dette prosjekt 
*/

public class RijndaelCryptoManager
{
    public string RijndaelKeyPath { get; private set; }
    public string RijndaelVectorPath { get; private set; }
    private RijndaelManagedContainer Keys { get; set; }

    /// <summary>
    /// The files should be protected with the current users credentials. Try the parameterless constructor to use this class with embedded keys
    /// </summary>
    /// <param name="rijndaelKeyPath">Protected Key</param>
    /// <param name="rijndaelVectorPath">Protected Vector</param>
    public RijndaelCryptoManager(string rijndaelKeyPath, string rijndaelVectorPath)
    {
        RijndaelKeyPath = rijndaelKeyPath;
        RijndaelVectorPath = rijndaelVectorPath;
        Keys = UnProtectRijndael();
    }

    public RijndaelCryptoManager(byte[] key, byte[] vector)
    {
        Keys = new RijndaelManagedContainer(key, vector);
    }


   
    public byte[] EncryptRijndael(byte[] data)
    {
        return RijndaelManagedTransform(data, Keys.EncryptorTransform);
    }

    public byte[] DecryptRijndael(byte[] data)
    {
        return RijndaelManagedTransform(data, Keys.DecryptorTransform);
    }

    /// <summary>
    /// Krypterer inn-data med gitt encoding til en streng på hex-format
    /// </summary>
    /// <param name="data">Klartekst</param>
    /// <param name="encoding">Encoding</param>
    /// <returns>Kryptert streng på hex-format</returns>
    public string EncryptRijndaelToHexString(string data, Encoding encoding)
    {
        var klartekstBytes = encoding.GetBytes(data);
        var kryptertTekstBytes = EncryptRijndael(klartekstBytes);
        return BitConverter.ToString(kryptertTekstBytes).Replace("-", "");
    }

    /// <summary>
    /// Dekrypterer en string på hex-format til tekst med gitt encoding
    /// </summary>
    /// <param name="hexData">Kryptert tekst på hex-format</param>
    /// <param name="encoding">Encoding</param>
    /// <returns>Dekryptert tekst</returns>
    public string DecryptRijndaelFromHexString(string hexData, Encoding encoding)
    {
        var kryptertTekstBytes = HexStringToByteArray(hexData);
        var dekryptertTekstBytes = DecryptRijndael(kryptertTekstBytes);
        return encoding.GetString(dekryptertTekstBytes);
    }

    public static byte[] HexStringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    /// <summary>
    /// Uses embedded testkeys
    /// </summary>
    public RijndaelCryptoManager()
    {
        Keys = EmbeddedTestKeys;
    }

    public byte[] RijndaelManagedTransform(byte[] buffer, ICryptoTransform transform)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        byte[] transformBuffer;

        using (var ms = new MemoryStream())
        {
            CryptoStream cs = null;
            try
            {
                cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                transformBuffer = ms.ToArray();
            }
            finally
            {
                if (cs != null)
                {
                    cs.Close();
                    ((IDisposable)cs).Dispose();
                } // Close is not called by Dispose
            }
        }

        return transformBuffer;
    }

    private RijndaelManagedContainer EmbeddedTestKeys
    {
        get
        {
            var libraryName = Assembly.GetExecutingAssembly().GetName().Name;

            var key = Assembly.GetExecutingAssembly().GetManifestResourceStream(libraryName + ".Resources.Key.dat");
            var vector = Assembly.GetExecutingAssembly().GetManifestResourceStream(libraryName + ".Resources.IV.dat");

            return new RijndaelManagedContainer(ReadFully(key), ReadFully(vector));
        }
    }

    private RijndaelManagedContainer UnProtectRijndael()
    {
        var protectedKey = FromFile(RijndaelKeyPath);
        var protectedVector = FromFile(RijndaelVectorPath);

        return new RijndaelManagedContainer(UnProtect(protectedKey), UnProtect(protectedVector));
    }

    //CurrentUser krever impersonering og loaduserprofile
    private byte[] UnProtect(byte[] data, byte[] additionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
    {
        return ProtectedData.Unprotect(data, additionalEntropy, scope);
    }

    private byte[] FromFile(string path)
    {
        return File.ReadAllBytes(path);
    }

    private static byte[] ReadFully(Stream input)
    {
        using (var ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public class RijndaelManagedContainer
    {
        public byte[] Key { get; private set; }
        public byte[] Vector { get; private set; }
        public ICryptoTransform EncryptorTransform;
        public ICryptoTransform DecryptorTransform;

        public RijndaelManagedContainer(byte[] key, byte[] vector)
        {
            var rijndaelManaged = new RijndaelManaged();

            Key = key;
            Vector = vector;
            EncryptorTransform = rijndaelManaged.CreateEncryptor(Key, Vector);
            DecryptorTransform = rijndaelManaged.CreateDecryptor(Key, Vector);
        }

      
    }
}
}
