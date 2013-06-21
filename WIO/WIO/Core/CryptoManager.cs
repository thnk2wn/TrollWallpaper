using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WIO.Core
{
    public class CryptoManager
    {
        #region Declarations

        // Generated via TripleDesCryptoServiceProvider's GenerateKey / Key members
        private static readonly Byte[] DEFAULT_KEY = {
                                                         133, 222, 3, 221, 63, 120, 51, 77, 47, 215, 238, 15, 107, 7, 102,
                                                         237, 241, 71, 203, 18, 172, 146, 18, 186
                                                     };

        // Generated via TripleDesCryptoServiceProvider's GenerateIV / IV members
        private static readonly Byte[] DEFAULT_INIT_VECTOR = { 173, 251, 78, 70, 104, 83, 187, 225 };

        private enum EncryptMode
        {
            Encrypt = 0,
            Decrypt = 1
        }

        #endregion

        #region Private Static Methods

        private static ICryptoTransform GetCryptoTransform(EncryptMode mode, Byte[] key)
        {
            ICryptoTransform cryptoTransform = null;
            var cryptProvider = new TripleDESCryptoServiceProvider { Key = key, IV = DEFAULT_INIT_VECTOR };

            cryptoTransform = (mode == EncryptMode.Encrypt)
                                  ? cryptProvider.CreateEncryptor()
                                  : cryptProvider.CreateDecryptor();

            return cryptoTransform;
        }

        private static MemoryStream CryptoTransformToMemoryStream(
            ICryptoTransform cryptoTransform, Byte[] inputBytes)
        {
            var encryptedMemStream = new MemoryStream();
            var cryptStream = new CryptoStream(
                encryptedMemStream,
                cryptoTransform, CryptoStreamMode.Write);
            cryptStream.Write(inputBytes, 0, inputBytes.Length);
            cryptStream.FlushFinalBlock();
            encryptedMemStream.Position = 0;
            return encryptedMemStream;
        }

        private static string MemoryStreamToHexString(MemoryStream encMemStream)
        {
            var ret = new StringBuilder();
            foreach (var b in encMemStream.ToArray())
            {
                ret.AppendFormat("{0:X2}", b); //Format as hex
            }

            var result = ret.ToString();
            return result;
        }

        private static string MemoryStreamToString(MemoryStream encMemStream)
        {
            var ret = new StringBuilder();
            foreach (var b in encMemStream.ToArray())
            {
                ret.Append((char)b);
            }

            var result = ret.ToString();
            return result;
        }

        private static byte[] HexStringToByteArray(String hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];

            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }

        #endregion

        #region Public Static Encryption Methods

        public static string Encrypt3DES(string textToEncrypt)
        {
            return Encrypt3DES(textToEncrypt, DEFAULT_KEY);
        }

        public static string Encrypt3DES(string textToEncrypt, string key)
        {
            var keyBytes = TextToByteArray(key);
            return Encrypt3DES(textToEncrypt, keyBytes);
        }

        public static string Encrypt3DES(string textToEncrypt, Byte[] key)
        {
            var inputBytes = TextToByteArray(textToEncrypt);
            var cryptoTransform = GetCryptoTransform(EncryptMode.Encrypt, key);
            var encMemStream = CryptoTransformToMemoryStream(cryptoTransform, inputBytes);
            var result = MemoryStreamToHexString(encMemStream);
            return result;
        }

        #endregion Public Static Encryption Methods

        #region Public Static Decryption Methods

        public static string Decrypt3DES(string textToDecrypt)
        {
            return Decrypt3DES(textToDecrypt, DEFAULT_KEY);
        }

        public static string Decrypt3DES(string textToDecrypt, string key)
        {
            Byte[] keyBytes = TextToByteArray(key);
            return Decrypt3DES(textToDecrypt, keyBytes);
        }

        public static string Decrypt3DES(string hexTextToDecrypt, Byte[] key)
        {
            var inputBytes = HexStringToByteArray(hexTextToDecrypt);
            var cryptoTransform = GetCryptoTransform(EncryptMode.Decrypt, key);
            var encMemStream = CryptoTransformToMemoryStream(cryptoTransform, inputBytes);
            var result = MemoryStreamToString(encMemStream);
            return result;
        }

        #endregion Public Static Decryption Methods

        #region Public Static Utility Methods

        public static string ObfuscateString(string secret)
        {
            var bytes = TextToByteArray(secret);
            return ByteArrayToText(bytes);
        }

        public static Byte[] TextToByteArray(string textToConvert)
        {
            var encoder = new UTF8Encoding();
            var bytes = encoder.GetBytes(textToConvert);
            return bytes;
        }

        public static string ByteArrayToText(Byte[] byteArray)
        {
            var text = byteArray.Aggregate("{", (current, b) => current + (b + ","));

            if (text.EndsWith(","))
            {
                text = text.Substring(0, text.Length - 1);
            }

            text += "}";

            return text;
        }

        #endregion Public Static Utility Methods
    }
}