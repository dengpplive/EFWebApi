using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    public class ChineseHelper
    {
        /// <summary>
        /// 随机生成指定长度的汉字编码
        /// </summary>
        /// <param name="strlength"></param>
        /// <returns></returns>
        public static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            string[] rBase = new String[16] {"0","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f"};
            Random rnd = new Random();
            object[] bytes = new object[strlength];
            for (int i = 0; i < strlength; i++)
            {
                int r1 = rnd.Next(11, 14);
                string str_r1 = rBase[r1].Trim();
                rnd = new Random(r1*unchecked((int) DateTime.Now.Ticks) + i); //更换随机数发生器的 种子避免产生重复值
                int r2;
                if (r1 == 13)
                {
                    r2 = rnd.Next(0, 7);
                }
                else
                {
                    r2 = rnd.Next(0, 16);
                }
                string str_r2 = rBase[r2].Trim();
                rnd = new Random(r2*unchecked((int) DateTime.Now.Ticks) + i);
                int r3 = rnd.Next(10, 16);
                string str_r3 = rBase[r3].Trim();
                //第4位 
                rnd = new Random(r3*unchecked((int) DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                string str_r4 = rBase[r4].Trim();
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                byte[] str_r = new byte[] {byte1, byte2};
                //将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(str_r, i);
            }
            return bytes;
        }
    }

} 
    



