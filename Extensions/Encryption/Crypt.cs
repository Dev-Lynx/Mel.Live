using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mel.Live.Extensions.Encryption
{
    public static class Crypt
    {
        #region Properties
        const int IvSize = 16;
        const int KeySize = 128;
        const int DerivationIterations = 1000;
        const string Key = "AAECAwQFBgcICQoLDA0ODw==";//"SafT is sure";
        #endregion

        #region Methods
        public static CryptoStream CreateEncryptionStream(Stream outputStream) => CreateEncryptionStream(Convert.FromBase64String(Key), outputStream);
        public static CryptoStream CreateDecryptionStream(Stream inputStream) => CreateDecryptionStream(Convert.FromBase64String(Key), inputStream);

        public static CryptoStream CreateEncryptionStream(byte[] key, Stream outputStream)
        {
            byte[] iv = new byte[IvSize];

            using (var rng = new RNGCryptoServiceProvider())
                // Using a cryptographic random number generator
                rng.GetNonZeroBytes(iv);

            // Write IV to the start of the stream
            outputStream.Write(iv, 0, iv.Length);

            Rijndael rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.None;
            rijndael.KeySize = KeySize;

            CryptoStream encryptor = new CryptoStream(
                outputStream,
                rijndael.CreateEncryptor(key, iv),
                CryptoStreamMode.Write);
            return encryptor;
        }

        public static CryptoStream CreateDecryptionStream(byte[] key, Stream inputStream)
        {
            byte[] iv = new byte[IvSize];


            if (inputStream.Read(iv, 0, iv.Length) != iv.Length)
                throw new ApplicationException("Failed to read IV from stream.");

            Rijndael rijndael = new RijndaelManaged();
            rijndael.Padding = PaddingMode.None;
            rijndael.KeySize = KeySize;

            CryptoStream decryptor = new CryptoStream(
                inputStream,
                rijndael.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);

            return decryptor;
        }

        public static string EncryptString(this string text, string passphrase = Key)
        {
            if (string.IsNullOrWhiteSpace(text)) text = "Empty";
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = { };

            using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, DerivationIterations))
            using (var symmetricKey = NewSymmetricalKey())
            using (var encryptor = symmetricKey.CreateEncryptor(password.GetBytes(KeySize / 8), ivStringBytes))
            using (var stream = new MemoryStream())
            using (var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                crypto.Write(plainTextBytes, 0, plainTextBytes.Length);
                crypto.FlushFinalBlock();

                cipherBytes = saltStringBytes.Concat(ivStringBytes).Concat(stream.ToArray()).ToArray();
            }
            return Convert.ToBase64String(cipherBytes);
        }

        public static string DecryptString(this string text, string passphrase = Key)
        {
            try
            {
                var cipherBytes = Convert.FromBase64String(text);
                var saltBytes = cipherBytes.Take(KeySize / 8).ToArray();
                var ivBytes = cipherBytes.Skip(KeySize / 8).Take(KeySize / 8).ToArray();

                var cleanCipherBytes = cipherBytes.Skip((KeySize / 8) * 2).Take(cipherBytes.Length - ((KeySize / 8) * 2)).ToArray();
                byte[] plainTextBytes = { };

                using (var password = new Rfc2898DeriveBytes(passphrase, saltBytes, DerivationIterations))
                using (var symmetricKey = NewSymmetricalKey())
                using (var decryptor = symmetricKey.CreateDecryptor(password.GetBytes(KeySize / 8), ivBytes))
                using (var stream = new MemoryStream(cleanCipherBytes))
                using (var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                {
                    plainTextBytes = new byte[cleanCipherBytes.Length];
                    var decryptedByteCount = crypto.Read(plainTextBytes, 0, plainTextBytes.Length);
                }
                return Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length).Trim('\0');
            }
            catch (Exception e)
            {
                Core.Log.Error(e);
                return "";
            }
        }

        /// <summary>
        /// Encrypt or Decrypt text using XOR encryption
        /// </summary>
        /// <param name="text">Text to encrypt or decrypt</param>
        /// <param name="key">Key to use during this process</param>
        /// <remarks>
        /// Thanks to Daniel Earwicker (https://stackoverflow.com/users/27423/daniel-earwicker)
        /// for his answer on stackoverflow (https://stackoverflow.com/a/2532704/8058709)
        /// </remarks>
        /// <returns>A refined version of the given text</returns>
        public static string XOR(string text, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }

        static byte[] Generate128BitsOfRandomEntropy()
        {
            var random = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(random);
            return random;
        }

        static RijndaelManaged NewSymmetricalKey(int blockSize = 128, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            return new RijndaelManaged() { BlockSize = blockSize, Mode = mode, Padding = padding };
        }

        #endregion
    }
}
