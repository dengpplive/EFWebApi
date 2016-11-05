using System.IO;
using System.Web;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using YSL.Api.DocumentController;
using YSL.Common.Resources;
namespace YSL.Host.InitConfig
{
    public class ApiExplorerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constant.DefaultApiDll + ".XML");
            config.Services.Replace(typeof(IDocumentationProvider), new XmlDocumentationProvider(path));
            HttpRuntime.Cache["ApiExploer"] = config.Services.GetApiExplorer();
        }
    }
}
