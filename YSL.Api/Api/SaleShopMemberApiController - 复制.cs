using JSL.DataEntity.ApiModel;
using JSL.DataEntity.ApiModel.Request;
using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using YSL.Business.Interface;
using YSL.Common.Extender;
using YSL.Common.MessagePackage;
//api调用业务层接口方法
namespace YSL.Api
{
    /// <summary>
    /// 例子
    /// </summary>   
    public class SaleShopMemberApiController : ApiControllerBase
    {
        ISaleShopMemberBusiness _iSaleShopMemberBusiness;
        /// <summary>
        /// 构造函数
        /// </summary>      
        public SaleShopMemberApiController(ISaleShopMemberBusiness iSaleShopMemberBusiness)
        {
            this._iSaleShopMemberBusiness = iSaleShopMemberBusiness;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]       
        public Result testApi()
        {
            return CommonResult(() =>
           {
               //调用业务层 接口
               var result = this._iSaleShopMemberBusiness.FindAll();
               return new Result()
               {
                   Data = result
               };
           }
            , r => Console.WriteLine(r.ToJSON()));
        }

        /// <summary>
        /// POST查询 基本类型       
        /// </summary>
        /// <returns></returns>
        /* var mm = WebApiHelper.PostAsync<Result>("SaleShopMemberApi/testPOSTApi", new RequestPackage<int>() { Data = 12 }).Result;/// */
        [HttpPost]
        public Result testPOSTApi([FromBody]RequestPackage<int> Data)
        {
            return CommonResult(() =>
            {
                //调用业务层 接口
                var result = this._iSaleShopMemberBusiness.FindAll();
                return new Result()
                {
                    Data = result
                };
            }
            , r => Console.WriteLine(r.ToJSON()));
        }

        /// <summary>
        /// 带实体的参数POST              
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        /* var mm = WebApiHelper.PostAsync<Result>("SaleShopMemberApi/testPostModelApi", new
               {
                  Data = new UserView
                  {
                      UserName = "张三",
                     Password = "12121212"
                 }
            }).Result;*/
        [HttpPost]
        public Result testPostModelApi([FromBody]RequestPackage<UserView> Data)
        {
            return CommonResult(() =>
            {
                //调用业务层 接口
                var result = this._iSaleShopMemberBusiness.FindAll();
                return new Result()
                {
                    Data = result
                };
            }
            , r => Console.WriteLine(r.ToJSON()));
        }
        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Result testAddApi()
        {
            return CommonResult(() =>
            {
                SaleShop_Member sm = new SaleShop_Member();
                sm.UserName = "dengjiyuan";
                sm.Password = "123456";

                //调用业务层 接口
                this._iSaleShopMemberBusiness.Insert(sm);


                //返回结果
                return new Result()
                {
                    Data = null
                };
            }
            , r => Console.WriteLine(r.ToJSON()));
        }


    }
}
