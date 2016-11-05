using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 加密解密数据类
    /// </summary>
    public class CryptionDataHelper
    {
        // The length of Encryptionstring should be 8 byte and not be a weak key
        private string EncryptionString;

        // The length of initialization vector should be 8 byte
        private static Byte[] EncryptionIV = Encoding.Default.GetBytes("abcdefgh");
        /// SecureKey
        public CryptionDataHelper(string EncryptionString)
        {
            this.EncryptionString = EncryptionString;
        }

        ///
        /// Encryption method for byte array
        ///
        /// source data
        /// byte array
        public byte[] EncryptionByteData(byte[] SourceData)
        {
            byte[] returnData = null;
            try
            {
                // Create DESCryptoServiceProvider object
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

                // Set SecureKey and IV of desProvider
                byte[] byteKey = Encoding.Default.GetBytes(EncryptionString);
                desProvider.Key = byteKey;
                desProvider.IV = EncryptionIV;

                // A MemoryStream object
                MemoryStream ms = new MemoryStream();

                // Create Encryptor
                ICryptoTransform encrypto = desProvider.CreateEncryptor();

                // Create CryptoStream object
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

                // Encrypt SourceData
                cs.Write(SourceData, 0, SourceData.Length);
                cs.FlushFinalBlock();

                // Get Encryption result
                returnData = ms.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnData;

        }
        /// source data
        /// byte array
        public byte[] DecryptionByteData(byte[] SourceData)
        {
            byte[] returnData = null;
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                byte[] byteKey = Encoding.Default.GetBytes(EncryptionString);
                desProvider.Key = byteKey;
                desProvider.IV = EncryptionIV;
                MemoryStream ms = new MemoryStream();
                ICryptoTransform encrypto = desProvider.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(SourceData, 0, SourceData.Length);
                cs.FlushFinalBlock();


                returnData = ms.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnData;

        }


        /// source data
        /// string
        public string EncryptionStringData(string SourceData)
        {
            try
            {
                byte[] SourData = Encoding.Default.GetBytes(SourceData);
                byte[] retData = EncryptionByteData(SourData);
                return Convert.ToBase64String(retData, 0, retData.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///
        /// Decryption method for string
        ///
        /// source data
        /// string
        public string DecryptionStringdata(string SourceData)
        {
            try
            {
                byte[] SourData = Convert.FromBase64String(SourceData);
                byte[] retData = DecryptionByteData(SourData);
                return Encoding.Default.GetString(retData, 0, retData.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
