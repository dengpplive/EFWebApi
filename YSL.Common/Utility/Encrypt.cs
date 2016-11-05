using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace YSL.Common.Utility
{
    /// <summary> 
    /// AES加密
    /// </summary> 
    public class AES
    {
        //默认密钥向量
        private static byte[] Keys = { 0x4E, 0x13, 0x4D, 0x36, 0x43, 0x4D, 0x4A, 0x15, 0x23, 0x3E, 0x34, 0x27, 0x10, 0x48, 0x13, 0x46 };

        /// <summary>
        /// AES加密字符串。
        /// </summary>
        /// <param name="encryptString">待加密的字符串，类型：System.String。</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串。</returns>
        public static string Encode(string encryptString)
        {
            string encryptKey = "";
            encryptKey = Converter.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            return Encode(encryptString, encryptKey);
        }

        /// <summary>
        /// AES加密字符串。
        /// </summary>
        /// <param name="encryptString">待加密的字符串，类型：System.String。</param>
        /// <param name="encryptKey">加密密钥，要求为8位；类型：System.String。</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串。</returns>
        public static string Encode(string encryptString, string encryptKey)
        {
            encryptKey = Converter.GetSubString(encryptKey, 32, "");
            encryptKey = encryptKey.PadRight(32, ' ');
            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// AES解密字符串。
        /// </summary>
        /// <param name="decryptString">待解密的字符串，类型：System.String。</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串。</returns>
        public static string Decode(string decryptString)
        {
            try
            {
                string decryptKey = "";
                decryptKey = Converter.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                return Decode(decryptString, decryptKey);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// AES解密字符串。
        /// </summary>
        /// <param name="decryptString">待解密的字符串，类型：System.String。</param>
        /// <param name="decryptKey">解密密钥，要求为8位和加密密钥相同；类型：System.String。</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串。</returns>
        public static string Decode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = Converter.GetSubString(decryptKey, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');
                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();
                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary> 
    /// DES加密
    /// </summary> 
    public class DES
    {
        //默认密钥向量
        private static byte[] Keys = { 0x19, 0x3E, 0x15, 0x43, 0x2D, 0x56, 0x14, 0x3C };
        /// <summary>
        /// DES加密字符串。
        /// </summary>
        /// <param name="encryptString">待加密的字符串，类型：System.String。</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串。</returns>
        public static string Encode(string encryptString)
        {
            string encryptKey = "";
            encryptKey = Converter.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            return Encode(encryptString, encryptKey);
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey)
        {
            encryptKey = Converter.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// DES解密字符串。
        /// </summary>
        /// <param name="decryptString">待解密的字符串，类型：System.String。</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串。</returns>
        public static string Decode(string decryptString)
        {
            try
            {
                string decryptKey = "";
                decryptKey = Converter.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                return Decode(decryptString, decryptKey);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = Converter.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary> 
    /// ASCII加密
    /// </summary> 
    public class ASCII
    {
        /// <summary>
        /// 字符串位移加密
        /// </summary>
        /// <param name="character">待加密的字符串，类型：System.String。</param>
        /// <returns></returns>
        public static string EncryptStr(string character)
        {
            string asciiCode = "";
            for (int i = 0; i < character.Length; i++)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character.Substring(i))[0] + 10;
                if (i > 0)
                {
                    asciiCode += "," + intAsciiCode.ToString();
                }
                else
                {
                    asciiCode = intAsciiCode.ToString();
                }
            }
            return asciiCode;
        }

        /// <summary>
        /// 字符串位移加密，不能解密。
        /// </summary>
        /// <param name="character">待加密的字符串，类型：System.String。</param>
        /// <returns></returns>
        public static string NotEncryptStr(string character)
        {
            int asciiCode = 0;
            for (int i = 0; i < character.Length; i++)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character.Substring(i))[0];
                asciiCode += intAsciiCode;
            }
            return "No." + asciiCode.ToString();
        }

        /// <summary>
        /// 字符串位移解密
        /// </summary>
        /// <param name="character">待解密的字符串，类型：System.String。</param>
        /// <returns></returns>
        public static string DecryptStr(string character)
        {
            string[] asciiCode = character.Split(',');
            string strCharacter = "";
            for (int i = 0; i < asciiCode.Length; i++)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)(Int32.Parse(asciiCode[i]) - 10) };
                strCharacter += asciiEncoding.GetString(byteArray);
            }
            return strCharacter;
        }
    }

    /// <summary>
    /// C#/PHP/JSP 3DES 加密与解密（只支持UTF-8编码）
    /// </summary>
    public class Crypto3DES
    {       
        /// <summary>
        /// 默认密钥
        /// </summary>
        private string Keys;

        /// <summary>
        /// 密钥与加密字符串不足8字符时的填充字符
        /// </summary>
        private char paddingChar = ' ';

        /// <summary>
        /// 实例化 Crypto3DES 类
        /// </summary>
        /// <param name="key">密钥</param>
        public Crypto3DES(string key)
        {
            this.Keys = key;
        }

        /// <summary>
        /// 获取密钥，不足8字符的补满8字符，超过8字符的截取前8字符
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        private string GetKeyCode(string key)
        {
            if (key.Length > 8)
                return key.Substring(0, 8);
            else
                return key.PadRight(8, paddingChar);
        }

        /// <summary>
        /// 获取加密字符串，不足8字符的补满8字符
        /// </summary>
        /// <param name="strString">The STR string.</param>
        /// <returns></returns>
        private string GetString(string strString)
        {
            if (strString.Length < 8)
                return strString.PadRight(8, paddingChar);
            return strString;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="strString">加密字符串</param>
        /// <returns></returns>
        public string Encrypt(string strString)
        {
            try
            {
                strString = this.GetString(strString);
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = Encoding.UTF8.GetBytes(this.GetKeyCode(this.Keys));
                DES.Mode = CipherMode.ECB;
                DES.Padding = PaddingMode.Zeros;
                ICryptoTransform DESEncrypt = DES.CreateEncryptor();
                byte[] Buffer = Encoding.UTF8.GetBytes(strString);
                return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="strString">解密字符串</param>
        /// <returns></returns>
        public string Decrypt(string strString)
        {
            try
            {
                DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                DES.Key = Encoding.UTF8.GetBytes(this.GetKeyCode(this.Keys));
                DES.Mode = CipherMode.ECB;
                DES.Padding = PaddingMode.Zeros;
                ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(strString);
                return UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length)).Replace("\0", "").Trim();
            }
            catch (Exception ex) { return ex.Message; }
        }

        #region DESEnCode DESDeCode
        /// <summary>
        /// 加密 与Java通用加密
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符</param>
        /// <param name="cryptKey">密钥，8位的ASCII字符</param>
        /// <returns></returns>
        public string DESEnCode(string pToEncrypt)
        {
            if (string.IsNullOrEmpty(pToEncrypt)) return string.Empty;

            try
            {
                pToEncrypt = System.Web.HttpContext.Current.Server.UrlEncode(pToEncrypt);
                string key = this.Keys;

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }

                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();

                return ret.ToString();
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }

        }
        /// <summary> 
        /// 解密数据  与Java通用解密
        /// </summary> 
        /// <param name="pToEncrypt">解密的字符</param>
        /// <param name="cryptKey">密钥，8位的ASCII字符</param>
        /// <returns></returns> 
        public string DESDeCode(string pToEncrypt)
        {
            if (string.IsNullOrEmpty(pToEncrypt)) return string.Empty;

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                int len = pToEncrypt.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;

                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(pToEncrypt.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }

                string key = this.Keys;

                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                string ret = System.Web.HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));

                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();

                return ret;
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
