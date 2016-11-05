using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.Config.Models
{
    /*
<configSections>
<sectionGroup name="ActiveMQ">
    <section name="MQConfig" type="PinMall.Core.Config.MQConfigSection, PinMall.Core" allowDefinition="Everywhere" allowLocation="true"/>
</sectionGroup>
</configSections>
<ActiveMQ>
    <MQConfig Host="192.168.16.34" Port="61616" UserName="admin" Password="admin" Debug="true"></MQConfig>
</ActiveMQ>
     */
    public sealed class ActiveMQConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 消息队列服务器IP
        /// </summary>
        [ConfigurationProperty("Host", IsRequired = true)]
        public string Host
        {
            get
            {
                return (string)base["Host"];
            }
            set
            {
                base["Host"] = value;
            }
        }

        /// <summary>
        /// 消息队列服务器端口
        /// </summary>
        [ConfigurationProperty("Port", IsRequired = false, DefaultValue = 61616)]
        public int Port
        {
            get
            {
                return (int)base["Port"];
            }
            set
            {
                base["Port"] = value;
            }
        }

        /// <summary>
        /// 消息队列服务器用户名
        /// </summary>
        [ConfigurationProperty("UserName", IsRequired = false)]
        public string UserName
        {
            get
            {
                return (string)base["UserName"];
            }
            set
            {
                base["UserName"] = value;
            }
        }

        /// <summary>
        /// 消息队列服务器用户名密码
        /// </summary>
        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }

        /// <summary>
        /// 自动重启
        /// </summary>
        [ConfigurationProperty("Debug", IsRequired = false, DefaultValue = false)]
        public bool Debug
        {
            get
            {
                return (bool)base["Debug"];
            }
            set
            {
                base["Debug"] = value;
            }
        }

        /// <summary>
        /// 消息队列名
        /// </summary>
        [ConfigurationProperty("TopicName", IsRequired = false)]
        public string TopicName
        {
            get
            {
                return (string)base["TopicName"];
            }
            set
            {
                base["TopicName"] = value;
            }
        }

        /// <summary>
        /// 消息ClientId
        /// </summary>
        [ConfigurationProperty("ClientId", IsRequired = false)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        public string ToConnectString()
        {
            return "failover:tcp://" + this.Host + ":" + this.Port;
        }

    }
}
