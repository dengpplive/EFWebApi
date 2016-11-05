using Newtonsoft.Json;

namespace WeiXin.Public.Common.Extra
{
    /// <summary>
    /// 微信粉丝概要
    /// </summary>
    public class WxUser
    {
        /// <summary>
        /// 即fakeid
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "nick_name")]
        public string NickName { get; set; }

        [JsonProperty(PropertyName = "remark_name")]
        public string RemarkName { get; set; }

        [JsonProperty(PropertyName = "group_id")]
        public int GroupID { get; set; }
    }
}