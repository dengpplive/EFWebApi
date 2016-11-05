using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using System.IO;
using System.Drawing.Drawing2D;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 对二维码图片处理 合并图片
    /// </summary>
    public class MergerImgHelper
    {
        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="file1">模版图片</param>
        /// <param name="file2">头像图片</param>
        /// <param name="file3">生成二维码网址</param>
        /// <returns></returns>
        public static Bitmap CreateCard(string file1, string file2, string urlStr)
        {
            ///模版
            Bitmap maptemplet = (Bitmap)Bitmap.FromFile(file1);
            ///头像
            Bitmap maptitle = (Bitmap)Bitmap.FromFile(file2);
            ///二维码
            Bitmap maperwei = QRCodeHelper.ToQRCode(urlStr);//(Bitmap)Bitmap.FromFile(file3);
            //求解最大的宽度
            int maxWidth = maptemplet.Width;
            int maxheight = maptemplet.Height;
            //指定要生成的图片的长宽
            Bitmap backgroudImg = new Bitmap(maxWidth, maxheight);
            Graphics g = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色
            g.Clear(System.Drawing.Color.White);
            g.DrawImage(maptemplet, 0, 0, maxWidth, maxheight);
            g.DrawImage(maptitle, 59, 59, 121, 121);
            g.DrawImage(maperwei, 82, 440, 125, 125);
            g.Dispose();
            return backgroudImg;
        }
        /// <summary>
        /// 二维码中追加 其他的信息
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="urlStr"></param>
        /// <param name="phone"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public static Bitmap CreateCard(string file1, Stream file2, string urlStr, string phone, string realName)
        {
            //if (!string.IsNullOrEmpty(realName))
            //{
            //    realName = realName.Length > 1 ? "*" + realName.Substring(1) : "*";
            //}
            string tmp = realName;
            if (tmp.Length > 1)
            {
                realName = realName.Substring(0, 1);
                for (int i = 1; i < tmp.Length; i++)
                {
                    realName += "*";
                }
            }
            phone = phone.Length > 7 ? phone.Substring(0, 3) + "****" + phone.Substring(7, phone.Length - 7) : phone;
            ///模版
            Bitmap _maptemplet = (Bitmap)Bitmap.FromFile(file1);
            Bitmap maptemplet = new Bitmap(_maptemplet);
            _maptemplet.Dispose();
            ///头像
            Bitmap _maptitle = (Bitmap)Bitmap.FromStream(file2);
            Bitmap maptitle = new Bitmap(_maptitle);
            _maptitle.Dispose();
            ///二维码
            Bitmap maperwei = QRCodeHelper.ToQRCode(urlStr);//(Bitmap)Bitmap.FromFile(file3);
            phone = null == phone ? "null" : phone;
            realName = null == realName ? "null" : realName;
            //求解最大的宽度
            int maxWidth = maptemplet.Width;
            int maxheight = maptemplet.Height;
            //指定要生成的图片的长宽
            Bitmap backgroudImg = new Bitmap(maxWidth, maxheight);
            Font font = new Font("Arial", 16);
            SolidBrush brush = new SolidBrush(Color.Black);
            Graphics g = Graphics.FromImage(backgroudImg);
            //清除画布,背景设置为白色
            g.Clear(System.Drawing.Color.White);
            g.DrawImage(maptemplet, 0, 0, maxWidth, maxheight);
            g.DrawImage(maptitle, 73, 59, 130, 130);
            g.DrawImage(maperwei, 96, 343, 258, 258);
            g.DrawString(realName, font, brush, 220, 93);
            g.DrawString(phone, font, brush, 220, 156);
            g.Dispose();
            return backgroudImg;
        }

        /// <summary>  
        /// 调用此函数后使此两种图片合并，类似相册，有个  
        /// 背景图，中间贴自己的目标图片  
        /// </summary>  
        /// <param name="imgBack">粘贴的源图片</param>  
        /// <param name="destImg">粘贴的目标图片</param>  
        public static Image CombinImage(Image imgBack, string destImg)
        {
            Image img = Image.FromFile(destImg);        //照片图片    
            if (img.Height != 65 || img.Width != 65)
            {
                img = KiResizeImage(img, 65, 65, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);
            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);   

            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框  

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);  

            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            GC.Collect();
            return imgBack;
        }

        /// <summary>  
        /// Resize图片  
        /// </summary>  
        /// <param name="bmp">原始Bitmap</param>  
        /// <param name="newW">新的宽度</param>  
        /// <param name="newH">新的高度</param>  
        /// <param name="Mode">保留着，暂时未用</param>  
        /// <returns>处理以后的图片</returns>  
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }
    }
}
