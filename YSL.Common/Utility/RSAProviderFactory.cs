using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace YSL.Common.Utility
{
    internal static class RSAProviderFactory
    {
        /// <summary>
        /// 解析java生成的pem文件私钥
        /// </summary>
        /// <param name="pemstr"></param>
        /// <returns></returns>
        internal static RSACryptoServiceProvider DecodePemPrivateKey(string pemstr)
        {
            RSACryptoServiceProvider rsa = null;
            byte[] pkcs8PrivteKey = Convert.FromBase64String(pemstr);
            if (pkcs8PrivteKey != null)
            {
                rsa = RSAProviderFactory.DecodePrivateKeyInfo(pkcs8PrivteKey);
            }
            return rsa;
        }

        internal static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            RSACryptoServiceProvider rsacsp = null;

            int lenStream = (int)mem.Length;
            BinaryReader binReader = new BinaryReader(mem);
            byte bt = 0;
            ushort twoBytes = 0;

            try
            {
                twoBytes = binReader.ReadUInt16();
                if (twoBytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binReader.ReadByte();	//advance 1 byte
                else if (twoBytes == 0x8230)
                    binReader.ReadInt16();	//advance 2 bytes
                else
                    return null;

                bt = binReader.ReadByte();
                if (bt != 0x02)
                    return null;

                twoBytes = binReader.ReadUInt16();

                if (twoBytes != 0x0001)
                    return null;

                seq = binReader.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, seqOID))	//make sure Sequence for OID is correct
                    return null;

                bt = binReader.ReadByte();
                if (bt != 0x04)	//expect an Octet string 
                    return null;

                bt = binReader.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binReader.ReadByte();
                else
                    if (bt == 0x82)
                        binReader.ReadUInt16();

                // at this stage, the remaining sequence should be the RSA private key
                byte[] rsaprivkey = binReader.ReadBytes((int)(lenStream - mem.Position));
                rsacsp = RSAProviderFactory.DecodeRSAPrivateKey(rsaprivkey);

                return rsacsp;
            }
            catch
            {
                return null;
            }
            finally
            {
                binReader.Close();
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        internal static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] modulus, e, d, p, q, dp, dq, iq;

            // Set up stream to decode the asn.1 encoded RSA private key
            MemoryStream mem = new MemoryStream(privkey);

            // wrap Memory Stream with BinaryReader for easy reading
            BinaryReader binReader = new BinaryReader(mem);
            byte bt = 0;
            ushort twoBytes = 0;
            int elems = 0;
            try
            {
                twoBytes = binReader.ReadUInt16();
                if (twoBytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                {
                    binReader.ReadByte(); // advance 1 byte
                }
                else if (twoBytes == 0x8230)
                {
                    binReader.ReadInt16(); // advance 2 byte
                }
                else
                {
                    return null;
                }

                twoBytes = binReader.ReadUInt16();
                if (twoBytes != 0x0102) // version number
                    return null;
                bt = binReader.ReadByte();
                if (bt != 0x00)
                    return null;

                // all private key components are Integer sequences
                elems = GetIntegerSize(binReader);
                modulus = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                e = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                d = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                p = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                q = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                dp = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                dq = binReader.ReadBytes(elems);

                elems = GetIntegerSize(binReader);
                iq = binReader.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters rsaParams = new RSAParameters();
                rsaParams.Modulus = modulus;
                rsaParams.Exponent = e;
                rsaParams.D = d;
                rsaParams.P = p;
                rsaParams.Q = q;
                rsaParams.DP = dp;
                rsaParams.DQ = dq;
                rsaParams.InverseQ = iq;
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binReader.Close();
            }
        }

        internal static int GetIntegerSize(BinaryReader binReader)
        {
            byte bt = 0;
            byte lowByte = 0x00;
            byte highByte = 0x00;
            int count = 0;
            bt = binReader.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binReader.ReadByte();

            if (bt == 0x81)
            {
                count = binReader.ReadByte();	// data size in next byte
            }
            else
            {
                if (bt == 0x82)
                {
                    highByte = binReader.ReadByte();	// data size in next 2 bytes
                    lowByte = binReader.ReadByte();
                    byte[] modint = { lowByte, highByte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;		// we already have the data size
                }
            }

            while (binReader.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binReader.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #region 解析.net 生成的Pem
        internal static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        internal static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion
    }
}