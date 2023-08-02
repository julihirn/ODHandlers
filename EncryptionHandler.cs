// '======================================
// ENCRYPTION HANDLER
// ONE DESKTOP COMPONENTS 
// J.J.HIRNIAK
// COPYRIGHT (C) 2015 J.J.HIRNIAK
// '======================================
using System;
using System.IO;
using System.Security.Cryptography;

namespace Handlers {
    public class EncryptionHandler {
        public static string STR_ES(string Password, string Input) {
            var wrapper = new Simple3Des(Password);
            string cipherText = wrapper.EncryptData(Input);
            return cipherText;
        }
        public static string STR_DS(string Password, string Input) {
            var wrapper = new Simple3Des(Password);
            try {
                string plainText = wrapper.DecryptData(Input);
                return plainText;
            }
            catch (CryptographicException ex) {
                ex.Message.ToString();
                return Input;
            }
        }
    }
    public sealed class Simple3Des {
        private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
        private byte[] TruncateHash(string key, int length) {
            var sha1 = new SHA1CryptoServiceProvider();

            // Hash the key. 
            var keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            var hash = sha1.ComputeHash(keyBytes);
            var oldHash = hash;
            hash = new byte[length];

            // Truncate or pad the hash. 
            if (oldHash != null)
                Array.Copy(oldHash, hash, Math.Min(length, oldHash.Length));
            return hash;
        }
        public Simple3Des(string key) {
            // Initialize the crypto provider.
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
        }
        public string EncryptData(string plaintext) {

            // Convert the plaintext string to a byte array. 
            var plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);

            // Create the stream. 
            var ms = new MemoryStream();
            // Create the encoder to write to the stream. 
            var encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), CryptoStreamMode.Write);

            // Use the crypto stream to write the byte array to the stream.
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            encStream.FlushFinalBlock();

            // Convert the encrypted stream to a printable string. 
            return Convert.ToBase64String(ms.ToArray());
        }
        public string DecryptData(string encryptedtext) {

            // Convert the encrypted text string to a byte array. 
            var encryptedBytes = Convert.FromBase64String(encryptedtext);

            // Create the stream. 
            var ms = new MemoryStream();
            // Create the decoder to write to the stream. 
            var decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), CryptoStreamMode.Write);

            // Use the crypto stream to write the byte array to the stream.
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();

            // Convert the plaintext stream to a string. 
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }
    }
}
