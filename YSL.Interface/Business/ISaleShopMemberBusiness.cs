using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.DDD;


namespace YSL.Business.Interface
{
    /// <summary>
    /// 终端机接口的声明
    /// </summary>
    public interface ISaleShopMemberBusiness : IDependency
    {
        IList<SaleShop_Member> FindAll();

        void Insert(SaleShop_Member saleShop_Member);



    }
}
