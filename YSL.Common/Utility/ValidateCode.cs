using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Web;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 验证码生成器
    /// </summary>
    public sealed class ValidateCode
    {
        private ValidateCode() { }
        private static string validateNum = null;
        /// <summary>
        /// 生成的验证码
        /// </summary>
        public string ValidateNum { get; private set; }
        /// <summary>
        /// 输出的Byte[]
        /// </summary>
        public byte[] ImgStream { get; private set; }

        #region 1  获取颜色数组 -Color[] GetColorArr()
        /// <summary>
        /// 获取颜色数组
        /// </summary>
        /// <returns> Color[]</returns>
        private static Color[] GetColorArr()
        {
            Color[] color ={ Color.Black,Color.Red,Color.Blue,Color.Tomato, Color.OrangeRed, Color.Olive, Color.Gold, Color.GreenYellow, 
                             Color.Blue, Color.LawnGreen, Color.Lime, Color.MediumSpringGreen, Color.Aqua,                                     Color.RoyalBlue, Color.MediumBlue,Color.BlueViolet, Color.MediumOrchid,                                           Color.Fuchsia, Color.DeepPink, Color.HotPink };
            return color;
        }
        #endregion

        #region 2.获取字体数组-string[] GetFontArr()
        /// <summary>
        /// 获取字体数组
        /// </summary>
        /// <returns>string[]</returns>
        private static string[] GetFontArr()
        {
            string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU" };
            return font;
        }
        #endregion

        #region 3.获取验证码的字符集-char[] GetCharacterArr()
        /// <summary>
        /// 获取验证码的字符集
        /// </summary>
        /// <returns></returns>
        private static char[] GetCharacterArr()
        {
            char[] character ={ '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                                'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            return character;
        }
        #endregion

        #region 4. 获取验证码字符串  根据需要的验证码的长度-string GetCheckCodeStr(int length)
        /// <summary>
        /// 获取验证码字符串  根据需要的验证码的长度
        /// </summary>
        /// <param name="length">验证码的长度</param>
        /// <returns>string</returns>
        private static string GetCheckCodeStr(int length)
        {
            Random rd = new Random();
            string checkCode = string.Empty;
            char[] character = GetCharacterArr();
            for (int i = 0; i < length; i++)
            {
                checkCode += character[rd.Next(character.Length)].ToString();
            }
            validateNum = checkCode.ToLower();//把验证码字符串保存到validateNum中
            return checkCode;
        }
        #endregion

        #region 5.返回对象 -ValidateCode GetValidateCode()
        public static ValidateCode GetValidateCode()
        {
            using (Bitmap bmp = new Bitmap(100, 45))//创建一个位图
            {
                Graphics g = Graphics.FromImage(bmp);//创建一幅图像
                g.Clear(Color.White);//背景色设置为白色

                Random rnd = new Random();

                #region 画噪线
                for (int i = 0; i < 7; i++)
                {
                    int x1 = rnd.Next(100);
                    int y1 = rnd.Next(40);
                    int x2 = rnd.Next(100);
                    int y2 = rnd.Next(40);
                    Color clr = GetColorArr()[rnd.Next(GetColorArr().Length)];
                    g.DrawLine(new Pen(clr), x1, y1, x2, y2);
                }
                #endregion

                #region 画验证码字符串
                string checkCode = GetCheckCodeStr(4); //在这里修改验证码的长度（如果更改了验证码的长度，一定要改                                                           画布的长度）
                for (int i = 0; i < checkCode.Length; i++)
                {
                    string fnt = GetFontArr()[rnd.Next(GetFontArr().Length)];
                    Font ft = new Font(fnt, 26);
                    //Color clr = GetColorArr()[rnd.Next(GetColorArr().Length)];
                    Color clr = GetColorArr()[rnd.Next(4)];
                    g.DrawString(checkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 8, (float)8);
                }
                #endregion

                #region 画噪点
                for (int i = 0; i < 100; i++)
                {
                    int x = rnd.Next(bmp.Width);
                    int y = rnd.Next(bmp.Height);
                    Color clr = GetColorArr()[rnd.Next(GetColorArr().Length)];
                    bmp.SetPixel(x, y, clr);
                }
                #endregion

                #region 画边框
                //Color col = GetColorArr()[rnd.Next(GetColorArr().Length)];
                //g.DrawLine(new Pen(GetColorArr()[rnd.Next(GetColorArr().Length)]), 0, 0, bmp.Width - 1, 0);
                //g.DrawLine(new Pen(GetColorArr()[rnd.Next(GetColorArr().Length)]), 0, 0, 0, bmp.Height - 1);
                //g.DrawLine(new Pen(GetColorArr()[rnd.Next(GetColorArr().Length)]), bmp.Width - 1, 0, bmp.Width - 1, bmp.Height - 1);
                //g.DrawLine(new Pen(GetColorArr()[rnd.Next(GetColorArr().Length)]), 0, bmp.Height - 1, bmp.Width - 1, bmp.Height - 1);
                #endregion

                #region 将验证码图片写入内存流，并将其以 "image/Png" 格式输出
                MemoryStream ms = new MemoryStream();
                try
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return new ValidateCode() { ImgStream = ms.ToArray(), ValidateNum = validateNum };
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    //显式释放资源 
                    bmp.Dispose();
                    g.Dispose();
                }
                #endregion
            }
        }
        #endregion

        #region 6.返回验证码条 +string GetValidateBar(string url)
        public static string GetValidateBar(string url)
        {
            return "<img id='checkCode' title='看不清，换一张！' src='" + url + "' style='cursor:pointer' alt=''   onclick=\"document.getElementById('checkCode').src='" + url + "?id='+ new Date().getTime();\" />";
        }
        #endregion


        /// <summary>
        /// 根据字符生成图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            Random rand = new Random();
            int randAngle = rand.Next(30, 60);//随机转动角度
            int iwidth = validateCode.Length * 23;
            //封装GDI+ 位图，此位图由图形图像及其属性的像素数据组成，指定的宽度和高度。以像素为单位
            Bitmap image = new Bitmap(iwidth, 28);
            //封装一个　GDI+绘图图面。无法继承此类。从指定的Image创建新的 Graphics
            Graphics g = Graphics.FromImage(image);
            try
            {
                //清除整个绘图面并以指定背景填充
                g.Clear(Color.AliceBlue);
                //画一个边框
                g.DrawRectangle(new Pen(Color.Silver, 0), 0, 0, image.Width - 1, image.Height - 1);
                //定义绘制直线和曲线的对象。（只是Pen的颜色，指示此Pen的宽度的值）
                Pen blackPen = new Pen(Color.LightGray, 0);
                //Random rand = new Random();
                //划横线的条数 可以根据自己的要求      
                for (int i = 0; i < 50; i++)
                {
                    //随机高度
                    /*绘制一条连线由坐标对指定的两个点的线条
                     线条颜色、宽度和样式，第一个点的x坐标和y坐标，第二个点的x坐标和y坐标*/
                    //g.DrawLine(blackPen, 0, y, image.Width, y);
                    int x = rand.Next(0, image.Width);
                    int y = rand.Next(0, image.Height);
                    //画矩形，坐标（x,y）宽高(1,1)
                    g.DrawRectangle(blackPen, x, y, 1, 1);
                }

                //拆散字符串成单个字符数组
                char[] chars = validateCode.ToCharArray();
                //文字居中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //定义字体
                string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "Arial Baltic" };

                for (int i = 0; i < chars.Length; i++)
                {
                    int findex = rand.Next(font.Length);
                    //font　封装在特定设备上呈现特定字体所需的纹理和资源（字体，大小，字体样式）
                    Font f = new System.Drawing.Font(font[findex], 16, System.Drawing.FontStyle.Bold);
                    /*Brush定义用于填充图形图像（如矩形、椭圆、圆形、多边形和封闭路径）的内部对象
                    SolidBrush(Color.White)初始化指定的颜色　指定画笔颜色为白色*/
                    Color color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                    Brush b = new System.Drawing.SolidBrush(color);
                    Point dot = new Point(16, 16);
                    //转动的度数
                    float angle = rand.Next(-randAngle, randAngle);
                    //移动光标到指定位置
                    g.TranslateTransform(dot.X, dot.Y);
                    g.RotateTransform(angle);
                    /*在指定的位置并且用指定的Brush和Font对象绘制指定的文本字符串
                   （指定的字符串，字符串的文本格式，绘制文本颜色和纹理，所绘制文本的左上角的x坐标，坐标）*/
                    g.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    //转回去
                    g.RotateTransform(-angle);
                    //移动光标指定位置
                    g.TranslateTransform(2, -dot.Y);
                }
                //创建存储区为内存流
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //将此图像以指定的格式保存到指定的流中（将其保存在内存流中，图像的格式）
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

    }

}
