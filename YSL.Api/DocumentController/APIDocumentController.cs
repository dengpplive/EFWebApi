using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using YSL.Common.Resources;
using YSL.Api.Document;
using System.Collections.ObjectModel;
namespace YSL.Api.DocumentController
{
    /// <summary>
    /// API文档的文档信息
    /// </summary>
    [RoutePrefix("api/doc")]
    public class APIDocumentController : ApiController
    {
        /// <summary>
        /// 获取webApi接口信息
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        [HttpGet, Route("getdoc/{controllerName?}/{actionName?}")]
        public HttpResponseMessage WebApi(string controllerName = "SaleShopMemberApi", string actionName = "testApi")
        {
            return GetDocument(controllerName, actionName, "api");
        }
        #region 私有方法
        private HttpResponseMessage GetDocument(string controllerName, string actionName, string apiPrefix)
        {
            var explorer = HttpRuntime.Cache["ApiExploer"] as IApiExplorer;
            //过滤指定的控制器          
            var appactions = explorer.ApiDescriptions.Where(a => a.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().Contains(apiPrefix))
                .Where(p => !p.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().Contains("apidocument"));
              
            var actions = explorer.ApiDescriptions.Where(a => a.ActionDescriptor.ControllerDescriptor.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

            #region head html
            var html = "<html>" +
                       "<head>" +
                       "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" +
                       "<title>" + Constant.DocumentTitle + "</title>" +
                       "<style>" +
                       "html{color:#000;background:#fff;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%; } body,div,dl,dt,dd,ul,ol,li,h1,h2,h3,h4,h5,h6,pre,code,form,fieldset,legend,input,textarea,p,blockquote,th,td,hr,button{margin:0;padding:0;}body,button,input,select,textarea{font:12px/1.5 \"微软雅黑\", tahoma,arial,\\5b8b\\4f53;}input,select,textarea{font-size:100%;} table{border-collapse:collapse;border-spacing:0;}a:hover {text-decoration:underline;} .fn-clear:after {visibility:hidden;display:block;font-size:0;content:\" \";clear:both;height:0;}.fn-clear {zoom:1; /* for IE6 IE7 */} img{ border:none; _display:inline-block;}.header {position: relative;background-color: #f3f3f3;box-shadow: 0 2px 3px rgba(0,0,0,0.15),inset 0 -1px 0 0 #fcfcfc;-moz-box-shadow: 0 2px 3px rgba(0,0,0,0.15),inset 0 -1px 0 0 #fcfcfc;-webkit-box-shadow: 0 2px 3px rgba(0,0,0,0.15),inset 0 -1px 0 0 #fcfcfc;border-top: 4px solid #11b6f1;border-bottom: 1px solid #ddd;}.header .inner {height: 60px;width: 1000px;margin-left: auto;margin-right: auto;}.header .logo{ display:block; margin-top:10px;line-height:58px;}.wrapper {width: 1200px;margin: 35px 0;margin-left: auto;margin-right: auto;}.wrapper .inner {position: relative;min-height: 730px;overflow: hidden;border: 1px solid #ddd;background-color: #f3f3f3;box-shadow: 0 2px 2px 0 #e3e3e3;-moz-box-shadow: 0 2px 2px 0 #e3e3e3;-webkit-box-shadow: 0 2px 2px 0 #eee;border-radius:3px;-moz-border-radius: 3px;-webkit-border-radius: 3px;}.left_menu{font-size: 14px;position: absolute;top: 0;left: 0;width: 30 0px;float: left;padding: 0 0 100px;border-right: 1px solid #ECEDE8;margin-right: -1px;}.portal h5 {height: 35px;line-height: 35px;margin: -1px 15px 0;border: 1px solid #D9D9D9;border-width: 1px 0;font-size: 14px;text-indent: 32px;color: #333;cursor: pointer;}.portal h5.active{color: #70ABD5;}.active .portal_arrow {border-top-color: #70ABD5;border-left-color: transparent;margin: 0 6px -3px -18px;}.portal_arrow  {width: 0;height: 0;border: 6px solid transparent;border-left-color: #aaa;display: inline-block;margin-left: -12px;font-size: 1px;line-height: 0;}.portal ul {margin: 3px 15px;padding: 0;list-style: none;}.portal li a {color: #666;display: block;text-indent:32px;text-decoration: none;height: 30px;line-height: 30px;font-size: 12px;border-radius: 3px; font:\"宋体\"; outline:none;}.portal li a:hover{ background-color:#eee;}.content{background-color: #ffffff;border-color: #d3d3d3;float: none;width: auto;min-height: 730px;margin-left: 300px;border-left: 1px solid #ECEDE8;padding: 0;padding-bottom: 100px;  }.content_title{border-bottom: 1px solid #d3d3d3;font-size: 14px;color: #545454;box-shadow: inset 0 1px 0 0 #fcfcfc;-moz-box-shadow: inset 0 1px 0 0 #fcfcfc;-webkit-box-shadow: inset 0 1px 0 0 #fcfcfc;background-color: #e9e9e9;line-height: 36px;height: 36px;}.content_title  h2 { font-weight:400; font-style:normal;font-size: 14px;padding-left: 20px;padding-right: 20px;margin: 0;}.bodyContent{padding-top: 18px;padding-bottom: 40px; margin: 0 35px;font-size: 14px;color: #333;line-height: 1.75;}.foot{ line-height:30px; background-color:#eee; text-align:center;}.explanation h2{ text-align:center; line-height:40px; margin-bottom:20px;}.explanation h3{}.explanation p{ font-size:12px; font:\"宋体\"; line-height:25px; margin:10px 0; text-indent:2em;}.mcontainer {overflow: hidden;word-wrap: break-word;padding: 20px 10px 20px 0px;background: #fff;width: 710px;line-height: 21px;margin: auto;}.mcontainer h1 {font-weight: 800;padding: 0 0 10px;border-bottom: 1px solid #e5e5e5;font-size: 16px;font-family: \"微软雅黑\";cursor: pointer;}.mcontainer .mcatalog {color: #0088cc;border: 1px solid #e4e4e4;padding: 10px 20px;margin: 20px 0;overflow: hidden;.mcontainer h2 {font-weight: 800;padding: 6px;background: #eef4f8;color: #353735;font-size: 14px;}}.mcontainer .mcontent p {margin-left: 8px;}.mcontainer p {margin: 6px 0;}.mcontainer pre {padding: 1em;border: 1px dashed #2F6FAB;color: #0A8021;background: #FAFAFA;line-height: 1.5em;font-family: \"Courier New\";overflow: auto;word-break: break-all;word-wrap:break-word;}.mplacetable {cellspacing: 0;border-collapse: collapse;font-size: 12px;margin: 8px 0 5px 0;}table {display: table;border-collapse: separate;border-spacing: 2px;border-color: grey;}tbody {display:table-row-group;vertical-align: middle;border-color: inherit;}tr {display: table-row;vertical-align: inherit;border-color: inherit;}.mplacetable th {border: solid 1px #CCC;height: 30px;line-height: 30px;background:#eef4fe;text-align: left;padding: 0 6px 0 10px;}.mplacetable td {border: solid 1px #CCC;height: 30px;line-height: 20px;padding: 0 6px 0 10px;}.mcode {color: #0A8021;font-family: consolas,'courier new',monospace;white-space: nowrap;word-break: nowrap;}.foot {line-height: 30px;background-color: #eee;text-align: center;}.mcontainer .mcatalog li {margin: 6px 0;list-style: none;}.mcatalog .mcatalog_odd_item {width: 300px;float: left;}.mcontainer ol li {margin-left: 8px;line-height: 21px;}body, div, dl, dt, dd, ul, ol, li, h1, h2, h3, h4, h5, h6, pre, code, form, fieldset, legend, input, textarea, p, blockquote, th, td, hr, button {margin: 0;padding: 0;}li {list-style: none;}body, html, li, ul, dd, h1, dl, h2, h3, h4, ol {padding: 0;margin: 0;}user agent stylesheetli {display: list-item;text-align: -webkit-match-parent;}.mcontainer a, .mcontainer a:hover {color: #0088cc;}a {text-decoration: none;blr: expression(this.onFocus=this.blur());}user agent stylesheeta:-webkit-any-link {color: -webkit-link;text-decoration: underline;cursor: auto;}ol {display: block;list-style-type: decimal;-webkit-margin-before: 1em;-webkit-margin-after: 1em;-webkit-margin-start: 0px;-webkit-margin-end: 0px;-webkit-padding-start: 40px;}.mcatalog .mcatalog_even_item {width: 300px;float: left;}.mcontainer h2 {font-weight: 800;padding: 6px;background: #eef4f8;color: #353735;font-size: 14px;}" +
                       "</style>" +
                       "</head>" +
                       "<body>" +
                       "<div class=\"header\">" +
                       "<div class=\"inner fn-clear\">" +
                       "<span style=\"vertical-align: middle;font-size: 24px;\">" + Constant.DocumentTitle + "</span>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"wrapper\" style=\"width:1400px;\">" +
                                "<div class=\"inner\">";
            #endregion

            if (actions.Any())
            {
                ApiDescription actionDescription;
                if (string.IsNullOrWhiteSpace(actionName))
                {
                    actionDescription = actions.FirstOrDefault();
                }
                else
                {
                    actionDescription = actions.FirstOrDefault(a => a.ActionDescriptor.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
                }
                if (actionDescription == null)
                {
                    return new HttpResponseMessage();
                }
                var actionList = GetActionList(appactions);
                var Model = new ApiDocument(actionDescription);
                Model.ReturnValueDocumentation = GetReturnValueDocumentation(actionDescription.ActionDescriptor);

                #region body html
                html += "<div class=\"left_menu\">" +
                        "<div class=\"portal\">" +
                            "<h5><span class=\"portal_arrow\"></span>接口开发</h5>" +
                            "<ul id=\"ul_menu\" target=\"mcontainer\">";
                html = actionList.Aggregate(html, (current, item) => current + ("<li><a href=\"/api/doc/getdoc/" + item.Item2 + "/" + item.Item3 + "\"" + "class=\"select\">" + item.Item1 + "</a></li>"));

                html += "</ul>" +
                        "</div>" +
                        "</div>" +
                        "<div class=\"content\">" +
                        "<div class=\"bodyContent\">" +
                        "<div class=\"mcontainer\" style=\"width:1000px;\">" +
                        "<div id=\"div_getProduct\"><h1>" + Model.Name + "接口</h1>" +
                        "<div class=\"mcatalog\">" +
                        "<ol>" +
                        "<li class=\"mcatalog_odd_item\"><a href=\"#api_introduce\">1.接口说明</a></li>" +
                        "<li class=\"mcatalog_odd_item\"><a href=\"#api_url\">2.接口地址</a></li>" +
                        "<li class=\"mcatalog_even_item\"><a href=\"#api_httpmethod\">3.接口调用方式</a></li>" +
                        "<li class=\"mcatalog_odd_item\"><a href=\"#api_paras\">4.接口参数说明</a></li>" +
                        "<li class=\"mcatalog_even_item\"><a href=\"#api_resposne\">5.返回结果</a></li>" +
                        "<li class=\"mcatalog_even_item\"><a href=\"#api_jsonResponse\">6.返回json格式的数据</a></li>" +
                        "</ol>" +
                        "</div>" +
                        "<div class=\"mcontent\">" +
                        "<h2 id=\"api_introduce\">接口说明</h2>" +
                        "<p>" + Model.Name + "<p>" +
                        "<h2 id=\"api_url\">接口地址</h2>" +
                        "<pre>" + "http://" + ActionContext.Request.RequestUri.Authority + "/" + Model.ApiDescription.RelativePath;
                html += "</pre>" +
                        "<br />" +
                        "<h2 id=\"api_httpmethod\">接口调用方式</h2>" +
                        "<pre>" + Model.Method + "</pre>" +
                        "<br />" +
                        "<h2 id=\"api_paras\">接口参数说明</h2>" +
                        "<table class=\"mplacetable\">" +
                        "<tbody>" +
                            "<tr>" +
                                "<th width=\"60\">参数类型</th>" +
                                "<th width=\"150\">参数名称</th>" +
                                "<th width=\"60\">是否必须</th>" +
                                "<th>具体描述</th>" +
                            "</tr>";
                foreach (var item in Model.RequestParameters)
                {
                    html += "<tr>" +
                            "<td class=\"mcode\">" + item.Type + "</td>" +
                            "<td>" + item.Name + "</td>";
                    if (item.IsOptional)
                    {
                        html += "<td>false</td>";
                    }
                    else
                    {
                        html += "<td>true</td>";
                    }
                    html += "<td>" + item.Description + "</td></tr>";
                }
                html += "</tbody></table>";
                var requestJsonString = Model.GetRequestSampleJsonString();
                if (!string.IsNullOrWhiteSpace(requestJsonString))
                {
                    html += "<h2>请求参数的json格式</h2>" +
                            "<pre>" + Model.GetRequestSampleJsonString() + "</pre>" +
                            "<br />";
                }
                html += "<h2 id=\"api_resposne\">返回结果</h2>" +
                         "<p>" + Model.ReturnValueDocumentation + "</p>";
                if (Model.ResponseParameters.Any())
                {
                    html += "<table class=\"mplacetable\">" +
                                            "<tbody>" +
                                                "<tr>" +
                                                    "<th width=\"60\" colspan=\"2\">参数名</th>" +
                                                    "<th width=\"200\">类型</th>" +
                                                    "<th>说明</th>" +
                                                "</tr>";
                    html = Model.ResponseParameters.Aggregate(html, (current, t) => current + ("<tr>" + "<td class=\"mcode\" colspan=\"2\">" + t.Name + "</td>" + "<td>" + t.Type + "</td>" + "<td>" + t.Description + "</td>" + "</tr>"));
                    if (Model.ResultTypeIsResponsePackage)
                    {
                        html += "<tr>" +
                                "<th class=\"mcode\" colspan=\"2\">ExtensionData</th>" +
                                "<th></th>" +
                                "<th>安全校验，响应状态等信息</th>" +
                            "</tr>" +
                            "<tr>" +
                                "<td class=\"mcode\" colspan=\"2\">ExtensionData.Key</td>" +
                                "<td>String</td>" +
                                "<td>第三方系统开启接口权限时填写的自定义key，用于第三方进行验证请求响应</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td class=\"mcode\" colspan=\"2\">ExtensionData.ModelValidateErrors</td>" +
                                "<td>数组</td>" +
                                "<td>数据验证异常数据,格式为{key: value}</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td class=\"mcode\" colspan=\"2\">ExtensionData.CallResult</td>" +
                                "<td>Int32</td>" +
                                "<td>返回结果状态信息 1:请求成功,2:参数验证失败,3:参数错误,4：身份验证未通过,5：账户信息错误,6.请求失败,7.业务逻辑错误</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td class=\"mcode\" colspan=\"2\">ExtensionData.RetMsg</td>" +
                                "<td>string</td>" +
                                "<td>返回结果描述</td>" +
                            "</tr>";
                    }
                    html += "</tbody></table>";
                }
                html += "<h2 id=\"api_jsonResponse\">返回json格式的数据</h2><pre>" + Model.GetResponseSampleJsonString() + "</pre></div></div></div></div></div>";

                #endregion

            }
            else
            {
                html += "<span style=\"vertical-align: middle;font-size: 22px;\">暂无接口数据</span>";
            }

            #region foot html
            #endregion

            //返回结果
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(html, Encoding.GetEncoding("UTF-8"), "text/html")
            };
        }

        private string GetReturnValueDocumentation(HttpActionDescriptor httpActionDescriptor)
        {
            var documentationProvider = GlobalConfiguration.Configuration.Services.GetService(typeof(IDocumentationProvider)) as XmlDocumentationProvider;
            if (documentationProvider == null)
            {
                return string.Empty;
            }
            var methodNode = documentationProvider.GetMethodNode(httpActionDescriptor);
            if (methodNode != null)
            {
                XPathNavigator summaryNode = methodNode.SelectSingleNode("returns");
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }
            }
            return string.Empty;
        }

        private IEnumerable<Tuple<string, string, string>> GetActionList(IEnumerable<ApiDescription> apiDescriptions)
        {
            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            string documentation = null;
            string controller = null;
            string action = null;
            foreach (var item in apiDescriptions)
            {
                // var islogin = ((System.Web.Http.Controllers.ReflectedHttpActionDescriptor)(item.ActionDescriptor)).GetFilters().Contains(new CheckAppLoginAttribute()) ? "(需要登录)" : "";
                documentation = item.Documentation;// +islogin;
                controller = item.ActionDescriptor.ControllerDescriptor.ControllerName;
                action = item.ActionDescriptor.ActionName;
                if (string.IsNullOrWhiteSpace(documentation))
                {
                    documentation = item.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + item.ActionDescriptor.ActionName;
                }
                tuples.Add(new Tuple<string, string, string>(documentation, controller, action));
            }
            return tuples.Distinct().OrderByDescending(w => w.Item1);
        }

        #endregion
    }
}