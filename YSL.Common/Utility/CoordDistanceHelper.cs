using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 弧度和经纬度距离计算
    /// </summary>
    public class CoordDistanceHelper
    {
        private const double EARTH_RADIUS = 6378137.0;//地球半径(米)
        /// <summary>
        /// 角度数转换为弧度公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double radians(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 弧度转换为角度数公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double degrees(double d)
        {
            return d * (180 / Math.PI);
        }
        /// <summary>
        /// 计算两个经纬度之间的直接距离
        /// </summary>
        public static double GetDistance(Degree Degree1, Degree Degree2)
        {
            double radLat1 = radians(Degree1.X);
            double radLat2 = radians(Degree2.X);
            double a = radLat1 - radLat2;
            double b = radians(Degree1.Y) - radians(Degree2.Y);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        /// <summary>
        /// 计算两个经纬度之间的直接距离(google 算法)
        /// </summary>
        public static double GetDistanceGoogle(Degree Degree1, Degree Degree2)
        {
            double radLat1 = radians(Degree1.X);
            double radLng1 = radians(Degree1.Y);
            double radLat2 = radians(Degree2.X);
            double radLng2 = radians(Degree2.Y);
            double s = Math.Acos(Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Cos(radLng1 - radLng2) + Math.Sin(radLat1) * Math.Sin(radLat2));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        /// <summary>
        /// 以一个经纬度为中心计算出四个顶点
        /// </summary>
        /// <param name="Degree1">经纬度(经度，纬度)</param>
        /// <param name="distance">半径(米)</param>
        /// <returns>dd[0].X(最小), dd[3].X(最大), dd[0].Y(最小), dd[3].Y(最大)</returns>
        public static Degree[] GetDegreeCoordinates(Degree Degree1, double distance)
        {
            double dlng = 2 * Math.Asin(Math.Sin(distance / (2 * EARTH_RADIUS)) / Math.Cos(Degree1.X));
            dlng = degrees(dlng);//一定转换成角度数
            double dlat = distance / EARTH_RADIUS;
            dlat = degrees(dlat);//一定转换成角度数
            return new Degree[] { new Degree(Math.Round(Degree1.X + dlat,6), Math.Round(Degree1.Y - dlng,6)),//left-top
                                  new Degree(Math.Round(Degree1.X - dlat,6), Math.Round(Degree1.Y - dlng,6)),//left-bottom
                                  new Degree(Math.Round(Degree1.X + dlat,6), Math.Round(Degree1.Y + dlng,6)),//right-top
                                  new Degree(Math.Round(Degree1.X - dlat,6), Math.Round(Degree1.Y + dlng,6)) //right-bottom
            };
        }


        /// <summary>
        ///  获取距离SQL语句根据经纬度
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="lonField">经度字段</param>
        /// <param name="latField">纬度字段</param>
        /// <returns></returns>
        public static string GetDistanceSql(decimal lon, decimal lat, string lonField, string latField)
        {
            if (lon == 0 && lat == 0) return " -1 ";
            return string.Format(" IF({2}=0 && {3}=0 ,-1,(2 * 6378.137* ASIN(SQRT(POW(SIN(PI()*({0}-{2})/360),2)+COS(PI()*{1}/180) * COS({3} * PI()/180)*POW(SIN(PI()*({1}-{3})/360),2)))) * 1000 )", lon, lat, lonField, latField);
        }
    }
    // <summary>
    /// 经纬度坐标
    /// </summary>   
    public class Degree
    {
        public Degree(double x, double y)
        {
            X = x;
            Y = y;
        }
        private double x;
        /// <summary>
        /// 经度
        /// </summary>
        public double X
        {
            get { return x; }
            set { x = value; }
        }
        private double y;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
