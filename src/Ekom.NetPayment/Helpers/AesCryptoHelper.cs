using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Umbraco.NetPayment.Helpers
{
    /// <summary>
    /// Helps with AES crypto calculations
    /// </summary>
    public static class AesCryptoHelper
    {
        /// <summary>
        /// Encrypt chosen input with the provided base64 encoded secret key.
        /// Returns a string of format {IV}-{cipherText}
        /// </summary>
        /// <param name="secret">Base64 encoded 64/128/256bit encryption key</param>
        /// <param name="input"></param>
        /// <returns>string of format {IV}-{cipherText}</returns>
        public static string Encrypt(string secret, string input)
        {
            byte[] encryptionKey = ValidateAndParseKey(secret, input);

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(encryptionKey, aes.IV);

                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] encryptedBytes = PerformCryptography(encryptor, inputBytes);

                return $"{Convert.ToBase64String(aes.IV)} {Convert.ToBase64String(encryptedBytes)}";
            }
        }

        /// <summary>
        /// Decrypt chosen input with the provided base64 encoded secret key.
        /// Takes a string of format {IV}-{cipherText}
        /// </summary>
        /// <param name="secret">Base64 encoded 64/128/256bit encryption key</param>
        /// <param name="input">string of format {IV}-{cipherText}</param>
        /// <returns>Plaintext</returns>
        public static string Decrypt(string secret, string input)
        {
            byte[] encryptionKey = ValidateAndParseKey(secret, input);

            var inputArray = input.Split(' ');

            if (inputArray.Length != 2)
                throw new ArgumentException("Input must be of format {IV}-{cipherText}", "input");

            using (Aes aes = Aes.Create())
            {
                var IV = Convert.FromBase64String(inputArray.First());

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(encryptionKey, IV);

                byte[] inputBytes = Convert.FromBase64String(inputArray.Last());

                var decryptedBytes = PerformCryptography(decryptor, inputBytes);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private static byte[] ValidateAndParseKey(string secret, string input)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");

            byte[] encryptionKey = Convert.FromBase64String(secret);

            if (encryptionKey.Length != 8
            && encryptionKey.Length != 16
            && encryptionKey.Length != 32)
                throw new ArgumentException("Encryption key must be 64/128/256bit", "secret");

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            return encryptionKey;
        }

        private static byte[] PerformCryptography(ICryptoTransform cryptoTransform, byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
