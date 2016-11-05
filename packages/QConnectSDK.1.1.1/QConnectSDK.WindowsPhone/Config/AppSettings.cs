using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace QConnectSDK.Config
{
    public class AppSettings : Dictionary<string, string>, IAppSettings
    {
        private string appConfigFile = "/{0};component/qzone.config";
        public AppSettings(string namespaceMainPage)
        {
            var streamInfo = Application.GetResourceStream(new Uri(string.Format(appConfigFile, namespaceMainPage), UriKind.Relative));

            using (var reader = new StreamReader(streamInfo.Stream))
            {
                var settings = XElement.Load(reader);

                foreach (var setting in settings.Elements())
                {
                    Add(setting.Attribute("name").Value, setting.Attribute("value").Value);
                }
            }
        }
    }
}
