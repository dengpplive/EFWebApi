using System.Web;
using Autofac;
using Autofac.Configuration;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web.Http;
using YSL.Common.Resources;
using System.Web.Compilation;
using System.Collections;
using System.Collections.Generic;
using YSL.Business;
using JSL.EFDataContext;
using YSL.Framework.DDD;
using YSL.Framework.EFRepository.UnitOfWork;
using YSL.Business.Interface;
using YSL.Repository;
using YSL.Framework.Config;
namespace YSL.Host.InitConfig
{
    public class ApiIocConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            //注册api容器的实现
            builder.RegisterApiControllers(Assembly.Load(Constant.DefaultApiDll));
            //注册UnitOfWork
            builder.RegisterType<EFUnitOfWork<XCY_DataContext>>().As<IUnitOfWork>();
            builder.RegisterType<EFUnitOfWorkRepositoryAdapter<XCY_DataContext>>().As<IUnitOfWorkRepository>();
            builder.RegisterAssemblyTypes(Assembly.Load(Constant.DefaultRepositoryDll));
            var types = builder.RegisterAssemblyTypes(Assembly.Load(Constant.DefaultBusinessDll));
            types.Where(t => t.Name.ToLower().EndsWith("business")).AsImplementedInterfaces();             
            //注册模块接口                       
            //builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
            //ContainerManager.InitContainer();

            var container = builder.Build();
            HttpRuntime.Cache["containerKey"] = container;
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
        }
    }
}
