using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 二维码生成帮助类
    /// </summary>
    public class QRCodeHelper
    {
        /// <summary>
        /// 生成二维码图片 
        /// </summary>
        /// <param name="strText">图片中的文本信息</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Bitmap ToQRCode(string strText)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//设置二维码编码格式  
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;//设置错误校验 
            qrCodeEncoder.QRCodeVersion = 7;//设置编码测量度  4
            qrCodeEncoder.QRCodeScale = 4;//设置编码版本  0
            //生成图像
            return qrCodeEncoder.Encode(strText, Encoding.Default);
        }
        /// <summary>
        /// 识别二维码图片中的信息
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string FromQRCode(Bitmap img)
        {
            QRCodeDecoder decoder = new QRCodeDecoder();
            String decodedString = decoder.decode(new QRCodeBitmapImage(img), Encoding.Default);
            return decodedString;
        }   

        #region 其他的二维码方式
        /*
        public static Bitmap QRCodeByApi(int widthSize, string strText)
        {
            var imgSrc = "http://chart.apis.google.com/chart?chs=" + widthSize + "&chl=" + strText + "&choe=UTF-8&cht=qr";
            //imgSource.Source = (new BitmapImage(new Uri(imgSrc)));        
        }

        public static void Create(string strText)
        {
            QRCodeWriter qrWrite = new QRCodeWriter();
            ByteMatrix bm = qrWrite.encode(strText, BarcodeFormat.QR_CODE, 150, 150);
            this.QrImg.Source = ConvertByteMartixToWriteableBitmap(bm);
        }
        public WriteableBitmap ConvertByteMartixToWriteableBitmap(ByteMatrix bm)
        {
            WriteableBitmap wb = new WriteableBitmap(bm.Width, bm.Height);
            for (int x = 0; x <= wb.PixelWidth - 1; x++)
            {
                for (int y = 0; y <= wb.PixelHeight - 1; y++)
                {
                    if (bm.Array[y][x] == -1)
                    {
                        //白色
                        wb.Pixels[wb.PixelWidth * y + x] = BitConverter.ToInt32(BitConverter.GetBytes(0xffffffff), 0);
                    }
                    else
                    {
                        //黑色
                        wb.Pixels[wb.PixelWidth * y + x] = BitConverter.ToInt32(BitConverter.GetBytes(0xff000000), 0);
                    }
                }
            }
            return wb;
        }

        public static string ReadQRCode()
        {
            string result = string.Empty;
            WriteableBitmap wb = new WriteableBitmap(imageSource, null);
            QRCodeReader qrRead = new QRCodeReader();
            RGBLuminanceSource luminiance = new RGBLuminanceSource(wb, wb.PixelWidth, wb.PixelHeight);
            HybridBinarizer binarizer = new HybridBinarizer(luminiance);
            BinaryBitmap binBitmap = new BinaryBitmap(binarizer);
            Result results;
            try
            {
                results = qrRead.decode(binBitmap);
                result = results.Text;
            }
            catch (Exception ex)
            {
                result = "Error:" + ex.GetType() + ":" + ex.Message;
            }
            finally
            {
                return result;
            }
        }
        public void CtrlOutPut()
        {
            string helloWorld = "Hello World!";
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = qrEncoder.Encode(helloWorld);
            //二维码信息 矩阵
            Gma.QrCodeNet.Encoding.BitMatrix bitMatrix = qrCode.Matrix;

        }
        
        /// <summary>
        /// 控制台输出二维码
        /// </summary>
        public void ConsoleOutPut()
        {
            Console.Write(@"Type some text to QR code: ");
            string sampleText = Console.ReadLine();
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(sampleText);
            for (int j = 0; j < qrCode.Matrix.Width; j++)
            {
                for (int i = 0; i < qrCode.Matrix.Width; i++)
                {
                    char charToPrint = qrCode.Matrix[i, j] ? '█' : ' ';
                    Console.Write(charToPrint);
                }
                Console.WriteLine();
            }
            Console.WriteLine(@"Press any key to quit.");
            Console.ReadKey();
        }
        
        public static Bitmap QRCode(string strText)
        {
            //using ZXing;
            //using ZXing.Common;
            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            BitMatrix bm = mutiWriter.encode(strText, BarcodeFormat.QR_CODE, 300, 300);
            Bitmap img = bm.ToBitmap();
            return img;
        }
        */


        #endregion
    }
}
