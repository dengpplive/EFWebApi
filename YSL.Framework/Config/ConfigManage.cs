using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Log;
using YSL.Common.Resources;
using YSL.Framework.Config.Models;

namespace YSL.Framework.Config
{
    /// <summary>
    /// 配置的管理
    /// </summary>
    public class ConfigManage
    {
        public static ThirdPartyPlatformSection GetThePartyPlatform()
        {
            return ConfigurationManager.GetSection(Constant.thirdPartyPlatform_Key) as ThirdPartyPlatformSection;
        }

        public static UnityConfigurationSection GetUnitySection()
        {
            return ConfigurationManager.GetSection(Constant.unityConfigurationSection_key) as UnityConfigurationSection;
        }
        /// <summary>
        /// ActiveMQ/名称 访问
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ActiveMQConfigSection GetActiveMQConfig(string name)
        {
            ActiveMQConfigSection section = null;
            try
            {
                section = (ActiveMQConfigSection)ConfigurationManager.GetSection(name);
            }
            catch (Exception ex)
            {
                LogBuilder.NLogger.Error(ex.Message + " Section " + name + " is error.");
                //throw new ConfigurationErrorsException("Section " + name + " is error.");
            }
            if (section == null)
            {
                LogBuilder.NLogger.Error("Section " + name + " is not found.");
                //throw new ConfigurationErrorsException("Section " + name + " is not found.");
            }
            return section;
        }
    }
}
