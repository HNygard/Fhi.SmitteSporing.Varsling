using System;
using System.IO;
using System.Security.Cryptography;

namespace Fhi.Smittesporing.Varsling.LagKryptoKeyFiler
{
    class Program
    {
        public static void Main(string [] args)
        {

            string password = args[0];
            string path = args[1];

            try
            {
                using (var myRijndael = InitSymmetric(Rijndael.Create(), password, 256))
                {

                    var fileKey = new FileInfo($"{path}\\Key.dat");
                    var fileVector = new FileInfo($"{path}\\Vector.dat");

                    var proctedKey = Protect(myRijndael.Key);
                    var proctedVector = Protect(myRijndael.IV);

                    Console.WriteLine($"Lager fil : {fileKey}");
                    File.WriteAllBytes(fileKey.FullName, proctedKey);

                    Console.WriteLine($"Lager fil : {fileVector}");
                    File.WriteAllBytes(fileVector.FullName, proctedVector);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }


        public static SymmetricAlgorithm InitSymmetric(SymmetricAlgorithm algorithm, string password, int keyBitLength)
        {
            var salt = new byte[] { 1, 2, 23, 234, 37, 48, 134, 63, 248, 4 };

            const int Iterations = 234;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                if (!algorithm.ValidKeySize(keyBitLength))
                    throw new InvalidOperationException("Invalid size key");

                algorithm.Key = rfc2898DeriveBytes.GetBytes(keyBitLength / 8);
                algorithm.IV = rfc2898DeriveBytes.GetBytes(algorithm.BlockSize / 8);
                return algorithm;
            }
        }

        public static byte[] Protect(byte[] data)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                // only by the same current user.
                return ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

       

    }
}
