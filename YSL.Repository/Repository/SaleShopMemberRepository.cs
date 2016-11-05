using JSL.EFDataContext;
using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.DDD;
namespace YSL.Repository
{

    public class SaleShopMemberRepository : BaseRepository<SaleShop_Member>
    {
        public SaleShopMemberRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr) { }

        //扩展方法
        //...


    }
}
