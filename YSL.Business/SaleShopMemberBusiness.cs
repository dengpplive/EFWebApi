using JSL.DataEntity.ApiModel;
using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Business.Interface;
using YSL.Framework.DDD;
using YSL.Repository;
namespace YSL.Business
{
    /// <summary>
    /// 终端机接口的实现
    /// </summary>
    public class SaleShopMemberBusiness : ISaleShopMemberBusiness
    {
        IUnitOfWork _iUnitOfWork;
        IUnitOfWorkRepository _iUnitOfWorkRepository;

        SaleShopMemberRepository _iSaleShopMemberRepository;

        public SaleShopMemberBusiness(IUnitOfWork uow, IUnitOfWorkRepository uowr,
            SaleShopMemberRepository iSaleShopMemberRepository)
        {
            this._iUnitOfWork = uow;
            this._iUnitOfWorkRepository = uowr;
            this._iSaleShopMemberRepository = iSaleShopMemberRepository;
        }

        //编写接口


        public IList<SaleShop_Member> FindAll()
        {
            return this._iSaleShopMemberRepository.FindAllNoTracking().ToList();
        }

        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="saleShop_Member"></param>
        public void Insert(SaleShop_Member saleShop_Member)
        {
            //各个接口之间相互调用使用
            //[Interface] A _a = ((IContainer)HttpRuntime.Cache["containerKey"]).Resolve<A>();

            //this._iUnitOfWorkRepository.PersistCreationOf(saleShop_Member);
            this._iSaleShopMemberRepository.Create(saleShop_Member);
            this._iUnitOfWork.Commit();
        }
    }
}
