using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CFC.Uteis;

//namespace Residencial.Uteis
//{

//    public class Crypto
//    {

//        private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

//        /// <summary>
//        /// Encrypt the given string using AES.  The string can be decrypted using 
//        /// DecryptStringAES().  The sharedSecret parameters must match.
//        /// </summary>
//        /// <param name="plainText">The text to encrypt.</param>
//        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
//        //public static string EncryptStringAES(string plainText, string sharedSecret)
//        public static string EncryptStringAES(string plainText)
//        {
//            string sharedSecret = DadosSeguros.Codigo;

//            if (string.IsNullOrEmpty(plainText))
//                throw new ArgumentNullException("plainText");
//            //if (string.IsNullOrEmpty(sharedSecret))
//            //    throw new ArgumentNullException("sharedSecret");

//            string outStr = null;                       // Encrypted string to return
//            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

//            try
//            {
//                // generate the key from the shared secret and the salt
//                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

//                // Create a RijndaelManaged object
//                aesAlg = new RijndaelManaged();
//                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

//                // Create a decryptor to perform the stream transform.
//                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

//                // Create the streams used for encryption.
//                using (MemoryStream msEncrypt = new MemoryStream())
//                {
//                    // prepend the IV
//                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
//                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
//                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
//                    {
//                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
//                        {
//                            //Write all data to the stream.
//                            swEncrypt.Write(plainText);
//                        }
//                    }
//                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
//                }
//            }
//            finally
//            {
//                // Clear the RijndaelManaged object.
//                if (aesAlg != null)
//                    aesAlg.Clear();
//            }

//             //Return the encrypted bytes from the memory stream.
//            outStr = outStr.Replace("/", "-2F-");
//            outStr = outStr.Replace("!", "-21-");
//            outStr = outStr.Replace("#", "-23-");
//            outStr = outStr.Replace("$", "-24-");
//            outStr = outStr.Replace("&", "-26-");
//            outStr = outStr.Replace("'", "-27-");
//            outStr = outStr.Replace("(", "-28-");
//            outStr = outStr.Replace(")", "-29-");
//            outStr = outStr.Replace("*", "-2A-");
//            outStr = outStr.Replace("+", "-2B-");
//            outStr = outStr.Replace(",", "-2C-");
//            outStr = outStr.Replace(":", "-3A-");
//            outStr = outStr.Replace(";", "-3B-");
//            outStr = outStr.Replace("=", "-3D-");
//            outStr = outStr.Replace("?", "-3F-");
//            outStr = outStr.Replace("@", "-40-");
//            outStr = outStr.Replace("[", "-5B-");
//            outStr = outStr.Replace("]", "-5D-");
//            return outStr;
//        }

//        /// <summary>
//        /// Decrypt the given string.  Assumes the string was encrypted using 
//        /// EncryptStringAES(), using an identical sharedSecret.
//        /// </summary>
//        /// <param name="cipherText">The text to decrypt.</param>
//        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
//        //public static string DecryptStringAES(string cipherText, string sharedSecret)
//        public static string DecryptStringAES(string cipherText)
//        {
//            string sharedSecret = DadosSeguros.Codigo;

//            if (string.IsNullOrEmpty(cipherText))
//                throw new ArgumentNullException("cipherText");
//            //if (string.IsNullOrEmpty(sharedSecret))
//            //    throw new ArgumentNullException("sharedSecret");

//            //        //URL Decrytion Avoid Reserved Characters
//            cipherText = cipherText.Replace("-2F-", "/");
//            cipherText = cipherText.Replace("-21-", "!");
//            cipherText = cipherText.Replace("-23-", "#");
//            cipherText = cipherText.Replace("-24-", "$");
//            cipherText = cipherText.Replace("-26-", "&");
//            cipherText = cipherText.Replace("-27-", "'");
//            cipherText = cipherText.Replace("-28-", "(");
//            cipherText = cipherText.Replace("-29-", ")");
//            cipherText = cipherText.Replace("-2A-", "*");
//            cipherText = cipherText.Replace("-2B-", "+");
//            cipherText = cipherText.Replace("-2C-", ",");
//            cipherText = cipherText.Replace("-3A-", ":");
//            cipherText = cipherText.Replace("-3B-", ";");
//            cipherText = cipherText.Replace("-3D-", "=");
//            cipherText = cipherText.Replace("-3F-", "?");
//            cipherText = cipherText.Replace("-40-", "@");
//            cipherText = cipherText.Replace("-5B-", "[");
//            cipherText = cipherText.Replace("-5D-", "]");

//            // Declare the RijndaelManaged object
//            // used to decrypt the data.
//            RijndaelManaged aesAlg = null;

//            // Declare the string used to hold
//            // the decrypted text.
//            string plaintext = null;

//            try
//            {
//                // generate the key from the shared secret and the salt
//                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

//                // Create the streams used for decryption.                
//                byte[] bytes = Convert.FromBase64String(cipherText);
//                using (MemoryStream msDecrypt = new MemoryStream(bytes))
//                {
//                    // Create a RijndaelManaged object
//                    // with the specified key and IV.
//                    aesAlg = new RijndaelManaged();
//                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
//                    // Get the initialization vector from the encrypted stream
//                    aesAlg.IV = ReadByteArray(msDecrypt);
//                    // Create a decrytor to perform the stream transform.
//                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
//                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//                    {
//                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

//                            // Read the decrypted bytes from the decrypting stream
//                            // and place them in a string.
//                            plaintext = srDecrypt.ReadToEnd();
//                    }
//                }
//            }
//            finally
//            {
//                // Clear the RijndaelManaged object.
//                if (aesAlg != null)
//                    aesAlg.Clear();
//            }

//            return plaintext;
//        }

//        private static byte[] ReadByteArray(Stream s)
//        {
//            byte[] rawLength = new byte[sizeof(int)];
//            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
//            {
//                throw new SystemException("Stream did not contain properly formatted byte array");
//            }

//            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
//            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
//            {
//                throw new SystemException("Did not read byte array properly");
//            }

//            return buffer;
//        }
//    }
//}