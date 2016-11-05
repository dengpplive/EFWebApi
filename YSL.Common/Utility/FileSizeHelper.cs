using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 文件大小帮助类
    /// </summary>
    public class FileSizeHelper
    {
        /// <summary>
        /// 字节自动转换为指定单位
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String ToLongByteString(object s)
        {
            float Size = Convert.ToSingle(s);
            int Level = (int)Math.Log10(Convert.ToDouble(Size));
            String Unit = " 字节";
            float ret = 0.0f;
            //" TB", " PB", " EB", " ZB", " YB");
            if (Level >= 24)
            {
                ret = Size / (1024f * 1024f * 1024f * 1024f * 1024f * 1024f * 1024f * 1024f);
                Unit = "Y 字节";
            }
            else if (Level >= 21)
            {
                ret = Size / (1024f * 1024f * 1024f * 1024f * 1024f * 1024f * 1024f);
                Unit = "Z 字节";
            }
            else if (Level >= 18)
            {
                ret = Size / (1024f * 1024f * 1024f * 1024f * 1024f * 1024f);
                Unit = "E 字节";
            }
            else if (Level >= 15)
            {
                ret = Size / (1024f * 1024f * 1024f * 1024f * 1024f);
                Unit = "P 字节";
            }
            else if (Level >= 12)
            {
                ret = Size / (1024f * 1024f * 1024f * 1024f);
                Unit = "T 字节";
            }
            else if (Level >= 9)
            {
                ret = Size / (1024f * 1024f * 1024f);
                Unit = "G 字节";
            }
            else if (Level >= 6)
            {
                ret = Size / (1024f * 1024f);
                Unit = "M 字节";
            }
            else if (Level >= 3)
            {
                ret = Size / (1024f);
                Unit = "K 字节";
            }
            else
            {
                ret = (float)Size;
                Unit = " 字节";
            }
            return ret.ToString("##.##") + Unit;
        }

        public static long StringToByteLeng(float filesize, FormatUnit fm)
        {
            long Len = 0;
            if (fm == FormatUnit.字节)
            {
                Len = (int)filesize;
            }
            else if (fm == FormatUnit.K字节)
            {
                Len = (int)(filesize * 1024);
            }
            else if (fm == FormatUnit.M字节)
            {
                Len = (int)(filesize * 1024 * 1024);
            }
            else if (fm == FormatUnit.G字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024);
            }
            else if (fm == FormatUnit.T字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024 * 1024);
            }
            else if (fm == FormatUnit.P字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024 * 1024 * 1024);
            }
            else if (fm == FormatUnit.E字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024 * 1024 * 1024 * 1024);
            }
            else if (fm == FormatUnit.Z字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024);
            }
            else if (fm == FormatUnit.Y字节)
            {
                Len = (int)(filesize * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024);
            }
            return Len;
        }
    }
    public enum FormatUnit
    {
        字节 = 0,
        K字节 = 1,
        M字节 = 2,
        G字节 = 3,
        T字节 = 4,
        P字节 = 5,
        E字节 = 6,
        Z字节 = 7,
        Y字节 = 8
    }
}
