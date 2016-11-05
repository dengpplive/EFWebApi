using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using YSL.Common.Resources;
namespace YSL.Framework.Config
{
    /// <summary>
    /// 容器管理类，负责容器的初始化
    /// </summary>
    public class ContainerManager
    {
        private static bool isinitialized = false;

        private static readonly object lockHelper = new object();

        private static IUnityContainer container = null;
        public static IUnityContainer GetContainer()
        {
            return container;
        }

        public static void InitContainer()
        {
            if (isinitialized)
            {
                return;
            }
            lock (lockHelper)
            {
                if (isinitialized) return;
                container = new UnityContainer();
                container.LoadConfiguration(ConfigManage.GetUnitySection(), Constant.unityContainerName_key);
                isinitialized = true;
            }
        }
        public static T R<T>()
        {
            try
            {
                if (container == null) throw new Exception("UnityContainer容器未初始化");
                return container.Resolve<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
