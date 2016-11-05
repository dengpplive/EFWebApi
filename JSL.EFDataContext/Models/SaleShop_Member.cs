using System;
using System.Collections.Generic;
using JSL.EFDataContext;
using YSL.Framework.DDD;
namespace JSL.EFDataContext.Models
{
    public partial class SaleShop_Member : IAggregationRoot
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Password { get; set; }
        public Nullable<int> UserType { get; set; }
        public Nullable<long> CorpId { get; set; }
        public Nullable<long> ShopId { get; set; }
        public Nullable<long> UGId { get; set; }
        public string ICON { get; set; }
        public string ICON_Old { get; set; }
        public Nullable<int> Gender { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string QQ { get; set; }
        public string WeiXin { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string OtherContactInfo { get; set; }
        public Nullable<decimal> FundUseable { get; set; }
        public Nullable<decimal> FundFreeze { get; set; }
        public Nullable<decimal> VirtualCoin { get; set; }
        public Nullable<int> IntegralValue { get; set; }
        public Nullable<int> EmpiricValue { get; set; }
        public Nullable<long> GradeId { get; set; }
        public string Friends { get; set; }
        public Nullable<long> RecommenderId { get; set; }
        public Nullable<bool> IsReceiveDiscountSms { get; set; }
        public Nullable<bool> IsReceiveDeliveSms { get; set; }
        public string LastLoginIp { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public string ThisLoginIp { get; set; }
        public Nullable<System.DateTime> ThisLoginTime { get; set; }
        public Nullable<System.DateTime> RegTime { get; set; }
        public string RegIp { get; set; }
        public string LoginAppId { get; set; }
        public Nullable<System.DateTime> ForgetPWDTime { get; set; }
        public string IdCard { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public Nullable<long> ProvinceId { get; set; }
        public Nullable<long> CityId { get; set; }
        public Nullable<long> AreaId { get; set; }
        public string Career { get; set; }
        public Nullable<bool> IsBindIdCard { get; set; }
        public Nullable<bool> IsBindMobile { get; set; }
        public Nullable<bool> IsBindEmail { get; set; }
        public Nullable<bool> IsBindBank { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string PayPassword { get; set; }
        public Nullable<bool> HasSecureQus { get; set; }
        public string SecureQus { get; set; }
        public string SecureQusAns { get; set; }
        public Nullable<bool> IsInfoPerfect { get; set; }
        public Nullable<int> InfoIntegrity { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<bool> IsAddRealNameScore { get; set; }
        public Nullable<bool> IsAddAddressScore { get; set; }
        public Nullable<bool> IsAddEmailScore { get; set; }
        public Nullable<bool> IsAddWxScore { get; set; }
        public Nullable<bool> IsAddAppScore { get; set; }
        public Nullable<bool> IsImport { get; set; }
        public Nullable<System.DateTime> EditTime { get; set; }
        public Nullable<System.DateTime> SignTime { get; set; }
        public Nullable<bool> AppPushMsg { get; set; }
    }
}
