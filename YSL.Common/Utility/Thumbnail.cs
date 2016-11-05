using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 缩略图生成器
    /// </summary>
    public class Thumbnail
    {
        int ImgSize = 100;
        string ItemPrefix = "";
        int Minstock = 5;
        static int WaterMark = 1;
        int BigPicWidth = 0;
        int BigPicHeight = 0;
        int SmallPicWidth = 0;
        int SmallPicHeight = 0;
        static string WaterMarkImg = "/images/application_view_tile.png";
        static int WaterMarkPlace = 9;
        int WaterMarkAlpha = 0;
        int GoodsListSize = 14;
        int GoodsListNum = 10;
        int AutoGenImg = 1;
        int TodayOtherGroup = 3;
        int BeforViewNow = 0;
        /// <summary>
        /// Converts the jpge.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="savefilename">The savefilename.</param>
        /// <param name="quality">The quality.</param>
        public static void ConvertJpge(string filename, string savefilename, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                using (Bitmap tImage = new Bitmap(oImage.Width, oImage.Height))
                {
                    using (Graphics g = Graphics.FromImage(tImage))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.Clear(Color.Transparent);
                        g.DrawImage(oImage, new Rectangle(0, 0, oImage.Width, oImage.Height), new Rectangle(0, 0, oImage.Width, oImage.Height), GraphicsUnit.Pixel);

                        SaveFile(filename, savefilename, tImage, quality);
                    }
                }
            }
        }
        /// <summary>
        /// 生成缩略图，指定高宽裁减不变形。
        /// </summary>
        /// <param name="filename">源图路径，类型：System.String。</param>
        /// <param name="savefilename">缩略图路径，类型：System.String。</param>
        /// <param name="width">缩略图宽度，类型：System.Int32。</param>
        /// <param name="height">缩略图高度，类型：System.Int32。</param>
        /// <param name="quality">缩略图质量(0-100)，类型：System.Int32。</param>
        /// <param name="SaveMode">裁剪图片类型（裁剪，缩放）：System.Int32。</param>
        public static void Make(string filename, string savefilename, int width, int height, int quality, SaveMode mode)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, height, mode);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// 生成缩略图，指定高宽裁减不变形。
        /// </summary>
        /// <param name="filename">源图路径，类型：System.String。</param>
        /// <param name="savefilename">缩略图路径，类型：System.String。</param>
        /// <param name="width">缩略图宽度，类型：System.Int32。</param>
        /// <param name="height">缩略图高度，类型：System.Int32。</param>
        /// <param name="quality">缩略图质量(0-100)，类型：System.Int32。</param>
        public static void Make(string filename, string savefilename, int width, int height, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, height, SaveMode.HW);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// 生成缩略图，指定宽缩放。
        /// </summary>
        /// <param name="filename">源图路径，类型：System.String。</param>
        /// <param name="thumbnailPath">缩略图路径，类型：System.String。</param>
        /// <param name="width">缩略图宽度，类型：System.Int32。</param>
        /// <param name="height">缩略图高度，类型：System.Int32。</param>
        /// <param name="quality">缩略图质量(0-100)，类型：System.Int32。</param>
        public static void Make(string filename, string savefilename, int width, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, 0, SaveMode.HW);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="savefilename"></param>
        /// <param name="img"></param>
        /// <param name="quality"></param>
        private static void SaveFile(string filename, string savefilename, Image img, int quality)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }

            using (EncoderParameters encoderParams = new EncoderParameters())
            {
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                {
                    quality = 80;
                }
                qualityParam[0] = quality;

                using (EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam))
                {
                    encoderParams.Param[0] = encoderParam;
                    if (WaterMark == 1)
                    {
                        //水印
                        img = setWatermark(img);
                    }
                    if (ici != null)
                    {
                        img.Save(savefilename, ici, encoderParams);
                    }
                    else
                    {
                        img.Save(savefilename);
                    }
                    img.Dispose();
                }
            }

        }

        #region 给图片打上水印
        /// <summary>
        /// 给图片打上水印
        /// </summary>
        /// <returns></returns>
        public static Image setWatermark(Image image)
        {
            //判断是打上文字水印还是图片水印
            string _watermark = WaterMarkImg;
            if (_watermark != "" && WaterMark == 1)
            {
                try
                {
                    if (_watermark.EndsWith(".gif") || _watermark.EndsWith(".jpg") || _watermark.EndsWith(".png"))
                    {
                        //加图片水印
                        Image copyImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(_watermark));
                        Graphics g = Graphics.FromImage(image);
                        int[] xyPosition = GetPosition(image.Width, image.Height, copyImage.Width, copyImage.Height);
                        int x = xyPosition[0];
                        int y = xyPosition[1];
                        g.DrawImage(copyImage, new Rectangle(x, y, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                        g.Save();
                        g.Dispose();
                        return image;
                    }
                    else
                    {
                        //加文字水印，注意，这里的代码和以下加图片水印的代码不能共存
                        Graphics g = Graphics.FromImage(image);
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                        Font f = new Font("Verdana", 24);
                        Brush b = new SolidBrush(Color.White);
                        g.DrawString(_watermark, f, b, 10, 10);
                        g.Save();
                        g.Dispose();
                        return image;
                    }

                }
                catch
                { }

            }
            return image;
        }
        #endregion

        #region 设置水印位置
        /// <summary>
        /// 设置水印位置
        /// </summary>
        /// <param name="imageWidth">需加水印图的宽度</param>
        /// <param name="imageHeight">需加水印图的高度</param>
        /// <param name="copyImageWidth">水印图的宽度</param>
        /// <param name="copyImageHeight">水印图的高度</param>
        /// <returns>水印所在图片的X、Y坐标</returns>
        private static int[] GetPosition(int imageWidth, int imageHeight, int copyImageWidth, int copyImageHeight)
        {
            int[] positions = new int[2];
            #region 水印位置
            int x;
            int y;
            int xOffset = 10; //宽度偏移量
            int yOffset = 10; //高度偏移量
            switch (WaterMarkPlace)
            {
                case 1:
                    //左上角
                    x = xOffset;
                    y = yOffset;
                    break;
                case 2:
                    //正上方
                    x = (imageWidth - copyImageWidth) / 2;
                    y = yOffset;
                    break;
                case 3:
                    //右上角
                    x = imageWidth - copyImageWidth - xOffset;
                    y = yOffset;
                    break;
                case 4:
                    //左中
                    x = xOffset;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 5:
                    //正中
                    x = (imageWidth - copyImageWidth) / 2;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 6:
                    //右中
                    x = imageWidth - copyImageWidth - xOffset;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 7:
                    //左下角
                    x = xOffset;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
                case 8:
                    //正下方
                    x = (imageWidth - copyImageWidth) / 2;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
                case 9:
                default:
                    //右下角
                    x = imageWidth - copyImageWidth - xOffset;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
            }
            positions[0] = x;
            positions[1] = y;
            #endregion
            return positions;
        }
        #endregion

        #region 生成图片缩略图
        public enum SaveMode
        {
            HW = 0x01,
            W = 0x02,
            H = 0x03,
            Cut = 0x04
        }
        ///   <summary>   
        ///   生成缩略图   
        ///   </summary>   
        ///   <param   name="originalImage">源图</param>   
        ///   <param   name="width">缩略图宽度</param>   
        ///   <param   name="height">缩略图高度</param>   
        ///   <param   name="mode">生成缩略图的方式</param>           
        public static Image setAutoSize(Image originalImage, int width, int height, SaveMode mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch ((int)mode)
            {
                case 1://指定宽高缩放    
                    if (towidth >= ow && toheight >= oh)
                    {
                        towidth = ow;
                        toheight = oh;
                    }
                    else
                    {
                        if (ow > oh)
                        {
                            toheight = originalImage.Height * towidth / originalImage.Width;

                        }
                        else
                        {
                            towidth = originalImage.Width * toheight / originalImage.Height;
                        }
                    }

                    break;
                case 2://指定宽，高按比例
                    if (towidth > ow) towidth = ow;
                    toheight = originalImage.Height * towidth / originalImage.Width;
                    break;
                case 3://指定高，宽按比例   
                    if (toheight > oh) toheight = oh;
                    towidth = originalImage.Width * toheight / originalImage.Height;
                    break;
                case 4://指定高宽裁减（不变形）                                   
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * toheight / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片   
            Image bitmap = new System.Drawing.Bitmap(width, height);

            //新建一个画板   
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //白色填充
            g.Clear(Color.White); //Color.Transparent清空画布并以透明背景色填充   

            //在指定位置并且按指定大小绘制原图片的指定部分   
            g.DrawImage(originalImage, new Rectangle((width - towidth) / 2, (height - toheight) / 2, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

            g.Dispose();

            return bitmap;
        }
        #endregion

        #region 图片旋转函数
        /// <summary>
        /// 以顺时针为方向对图像进行旋转
        /// </summary>
        /// <param name="b">位图流</param>
        /// <param name="angle">旋转角度[0,360](前台给的)</param>
        /// <returns></returns>
        public Image Rotate(Image b, int angle)
        {
            angle = angle % 360;

            //弧度转换
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            //原图的宽和高
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));

            //目标位图
            Image dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //计算偏移量
            Point Offset = new Point((W - w) / 2, (H - h) / 2);

            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            g.TranslateTransform(center.X, center.Y);
            //g.RotateTransform(360 - angle);
            g.RotateTransform(angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);

            //重至绘图的所有变换
            g.ResetTransform();

            g.Save();
            b.Dispose();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
        }
        #endregion 图片旋转函数

        #region 压缩图片
        /// <summary>
        /// 吴旭标
        /// 压缩图片
        /// </summary>
        /// <param name="contentByte">图片</param>
        /// <param name="width">压缩宽度</param>
        /// <param name="height">压缩高度</param>
        /// <param name="fileExt">图片扩展名</param>
        /// <param name="mode">压缩模式</param>
        public static byte[] MakeThumbnail(byte[] contentByte, int width, int height, string fileExt, string mode = "HorW")
        {
            try
            {
                Stream inStream = new MemoryStream(contentByte);
                System.Drawing.Image originalImage = System.Drawing.Image.FromStream(inStream);

                int towidth = width;
                int toheight = height;

                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;

                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "HorW":
                        //缩略图宽、高计算
                        double newWidth = originalImage.Width;
                        double newHeight = originalImage.Height;

                        //宽大于高或宽等于高（横图或正方）
                        if (originalImage.Width > originalImage.Height || originalImage.Width == originalImage.Height)
                        {
                            //如果宽大于模版
                            if (originalImage.Width > width)
                            {
                                //宽按模版，高按比例缩放
                                newWidth = width;
                                newHeight = originalImage.Height * ((double)width / originalImage.Width);
                            }
                        }
                        //高大于宽（竖图）
                        else
                        {
                            //如果高大于模版
                            if (originalImage.Height > height)
                            {
                                //高按模版，宽按比例缩放
                                newHeight = height;
                                newWidth = originalImage.Width * ((double)height / originalImage.Height);
                            }
                        }
                        towidth = (int)newWidth;
                        toheight = (int)newHeight;
                        break;
                    case "W"://指定宽，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

                //新建一个画板
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //清空画布并以透明背景色填充
                g.Clear(System.Drawing.Color.Transparent);

                MemoryStream resultStream = new MemoryStream();
                try
                {
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                        new System.Drawing.Rectangle(x, y, ow, oh),
                        System.Drawing.GraphicsUnit.Pixel);

                    fileExt = fileExt.ToLower();
                    switch (fileExt)
                    {
                        case "gif":
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case "png":
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case "bmp":
                        case "jpeg":
                        case "jpg":
                        default:
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                    }
                    Byte[] resultBuffer = new Byte[resultStream.Length];
                    //从流中读取字节块并将该数据写入给定缓冲区buffer中
                    resultStream.Seek(0, SeekOrigin.Begin);
                    resultStream.Read(resultBuffer, 0, resultBuffer.Length);

                    return resultBuffer;
                }
                catch (System.Exception e)
                {
                   // logger.Error(e.Message);
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                    inStream.Close();
                    inStream.Dispose();
                    resultStream.Close();
                    resultStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }
            return null;
        }
        #endregion
    }
}
