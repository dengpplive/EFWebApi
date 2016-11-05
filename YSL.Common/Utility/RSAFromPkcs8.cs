using System;
using System.Security.Cryptography;
using System.Text;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 功能：RSA解密、签名、验签
    /// 详细：该类对Java生成的密钥进行解密和签名以及验签专用类
    /// </summary>
    public sealed class RSAFromPkcs8
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">需要签名的内容</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="inputCharset">编码格式</param>
        /// <returns>返回签名字符串</returns>
        public static string Sign(string content, string privateKey, string inputCharset)
        {
            Encoding code = Encoding.GetEncoding(inputCharset);
            byte[] data = code.GetBytes(content);
            RSACryptoServiceProvider rsa = RSAProviderFactory.DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();

            byte[] signData = rsa.SignData(data, sh);
            return Convert.ToBase64String(signData);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="content">需要验证的内容</param>
        /// <param name="signedString">签名结果</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="inputCharset">编码格式</param>
        /// <returns>验签结果</returns>
        public static bool Verify(string content, string signedString, string publicKey, string inputCharset)
        {
            bool result = false;

            Encoding code = Encoding.GetEncoding(inputCharset);
            byte[] data = code.GetBytes(content);
            byte[] soureData = Convert.FromBase64String(signedString);
            RSAParameters paraPub = RSAProviderFactory.ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);

            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(data, sh, soureData);
            return result;
        }

        /// <summary>
        /// 用RSA解密
        /// </summary>
        /// <param name="resData">待解密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="inputCharset">编码格式</param>
        /// <returns>解密结果</returns>
        public static string DecryptData(string resData, string privateKey, string inputCharset)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(resData);
            string result = "";

            for (int j = 0; j < dataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {
                    buf[i] = dataToDecrypt[i + 128 * j];
                }
                result += Decrypt(buf, privateKey, inputCharset);
            }
            return result;
        }

        public static string Decrypt(byte[] data, string privateKey, string inputCharset)
        {
            string result = "";
            RSACryptoServiceProvider rsa = RSAProviderFactory.DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] source = rsa.Decrypt(data, false);
            Encoding code = Encoding.GetEncoding(inputCharset);

            char[] asciiChars = new char[code.GetCharCount(source, 0, source.Length)];
            code.GetChars(source, 0, source.Length, asciiChars, 0);
            result = new string(asciiChars);
            return result;
        }
    }
}