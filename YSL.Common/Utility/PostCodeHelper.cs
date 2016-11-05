using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 快递
    /// </summary>
    public class PostCodeHelper
    {
        #region 快递查询接口
        /// <summary>
        /// 功能：快递100查询接口（此接口不稳定）
        /// </summary>
        /// <param name="code">快递code</param>
        /// <param name="nu">快递单号</param>
        /// <returns></returns>
        public static string Express100API(string code, string nu)
        {
            //string key = GetConfig.ExpressKey(); //获取快递100key值
            //string apiurl = GetConfig.ExpressUrl() + key + "&com=" + code + "&nu=" + nu + "&order=asc"; //&show=2
            //WebRequest request = WebRequest.Create(@apiurl);
            //WebResponse response = request.GetResponse();
            //Stream stream = response.GetResponseStream();
            //Encoding encode = Encoding.UTF8;
            //StreamReader reader = new StreamReader(stream, encode);
            //string detail = reader.ReadToEnd();
            //return detail;

            WebClient wClient = new WebClient();
            wClient.Encoding = Encoding.UTF8;
            var response = wClient.DownloadString("http://www.kuaidi100.com/query?type=" + code + "&postid=" + nu);
            return response;
        }

        /// <summary>
        /// 功能： HaoService快递查询接口
        /// 备注：此接口现在需要付费，有套餐可以选择       
        /// </summary>
        /// <param name="code">快递code</param>
        /// <param name="nu">快递单号</param>
        /// <returns></returns>
        public static string Express100(string code, string nu)
        {
            WebClient wClient = new WebClient();
            wClient.Encoding = Encoding.UTF8;
            var response = wClient.DownloadString("http://apis.haoservice.com/lifeservice/exp?key=4ca9a6c407134181bc7457481e936b77&com=" + code + "&no=" + nu);
            return response;
        }

        /// <summary>
        ///  功能：爱快递查询接口
        ///  备注：目前这个接口是免费的申请的（每天可以条用2000次）      
        /// </summary>
        /// <param name="code">快递code</param>
        /// <param name="nu">快递单号</param>
        /// <returns></returns>
        public static string AiKuaiDiExpress(string code, string nu)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(nu))
            {
                return "{\"errCode\": \"12\",\"message\": \"未能查到相关数据\",}";
            }
            try
            {

                WebClient wClient = new WebClient();
                wClient.Encoding = Encoding.UTF8;
                var response = wClient.DownloadString("http://www.aikuaidi.cn/rest/?key=f415d8f5d8024d3483b705be1aef4ce7&order=" + nu + "&id=" + code);
                return response;
            }
            catch (Exception ex)
            {
                return "{\"errCode\": \"12\",\"message\": \"未能查到相关数据\",}";
            }
        }


        #endregion
    }
}
