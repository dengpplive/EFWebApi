using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http.Description;
using YSL.Api.Attributes;
namespace YSL.Api.Document
{
    /// <summary>
    /// Api文档
    /// </summary>
    public class ApiDocument
    {
        private readonly Type _mReturnType;

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 返回类型名称
        /// </summary>
        public string ReturnTypeName { get; private set; }

        /// <summary>
        /// 请求类型名称
        /// </summary>
        public string PostRequestTypeName
        {
            get
            { 
                return ApiDescription.ParameterDescriptions.FirstOrDefault().ParameterDescriptor.ParameterType.Name;
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public ApiDescription ApiDescription { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public List<ApiDocumentParameter> RequestParameters { get; set; }

        /// <summary>
        /// 响应参数
        /// </summary>
        public List<ApiDocumentParameter> ResponseParameters { get; set; }

        /// <summary>
        /// API接口名称
        /// </summary>
        public string Name
        {
            get
            {
                return ApiDescription.Documentation;
            }
        }

        /// <summary>
        /// 请求方法
        /// </summary>
        public System.Net.Http.HttpMethod Method
        {
            get
            {
                return ApiDescription.HttpMethod;
            }
        }

        /// <summary>
        /// Get请求的查询字符
        /// </summary>
        public string RequestQueryString
        {
            get
            {
                var containsQuestionMark = false;
                var sbParameters = new StringBuilder();
                for (var i = 0; i < ApiDescription.ParameterDescriptions.Count; i++)
                {
                    var type = ApiDescription.ParameterDescriptions[i].ParameterDescriptor.ParameterType;
                    if (type.IsValueType || type == typeof(string))
                    {
                        if (containsQuestionMark)
                        {
                            sbParameters.Append("&");
                        }
                        else
                        {
                            sbParameters.Append("?");
                            containsQuestionMark = true;
                        }
                        sbParameters.AppendFormat("{0}={{{1}}}", ApiDescription.ParameterDescriptions[i].Name, i);
                    }
                    else
                    {
                        var sampleObject = ApiDocumentManager.GetSampleObject(type);
                        foreach (var prop in type.GetProperties())
                        {
                            if (containsQuestionMark)
                            {
                                sbParameters.Append("&");
                            }
                            else
                            {
                                sbParameters.Append("?");
                                containsQuestionMark = true;
                            }
                            sbParameters.AppendFormat("{0}={1}", prop.Name, prop.GetValue(sampleObject, null));
                        }
                    }
                }
                return sbParameters.ToString();
            }
        }

        /// <summary>
        /// 返回值的文档描述
        /// </summary>
        public string ReturnValueDocumentation { get; set; }

        /// <summary>
        /// 返回的结果是否为ResponsePackage类型
        /// </summary>
        public bool ResultTypeIsResponsePackage { get; set; }

        /// <summary>
        /// 创建API文档模型
        /// </summary>
        /// <param name="apiDescription">apiDescription</param>
        public ApiDocument(ApiDescription apiDescription)
        {
            if (apiDescription == null)
            {
                throw new ArgumentNullException("explorer");
            }
            ApiDescription = apiDescription;
            var actionDescriptor = ApiDescription.ActionDescriptor;
            _mReturnType = actionDescriptor.ReturnType;

            Url = actionDescriptor.ControllerDescriptor.ControllerName + "/" + actionDescriptor.ActionName;
            ReturnTypeName = GetTypeName(_mReturnType);

            RequestParameters = new List<ApiDocumentParameter>();
            SetRequestParameters();

            ResponseParameters = new List<ApiDocumentParameter>();
            SetResponseParameters(_mReturnType);
        }

        /// <summary>
        /// 获取Json响应结果样例字符串
        /// </summary>
        /// <returns></returns>
        public string GetResponseSampleJsonString()
        {
            var sampleObject = ApiDocumentManager.GetSampleObject(_mReturnType);
            return Newtonsoft.Json.JsonConvert.SerializeObject(sampleObject, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// 获取Json请求样例字符串
        /// </summary>
        /// <returns></returns>
        public string GetRequestSampleJsonString()
        {
            // wandelfor:有的方法参数列表为空
            if (!ApiDescription.ParameterDescriptions.Any())
            {
                return string.Empty;
            }
            var sampleObject = ApiDocumentManager.GetSampleObject(ApiDescription.ParameterDescriptions[0].ParameterDescriptor.ParameterType, true);
            if (sampleObject != null)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(sampleObject, Newtonsoft.Json.Formatting.Indented);
            }
            return string.Empty;
        }

        private void SetRequestParameters()
        {
            foreach (var item in ApiDescription.ParameterDescriptions)
            {
                if (item.ParameterDescriptor.ParameterType.IsValueType || item.ParameterDescriptor.ParameterType == typeof(string))
                {
                    RequestParameters.Add(new ApiDocumentParameter
                    {
                        Type = item.ParameterDescriptor.ParameterType.Name,
                        Name = item.Name,
                        IsOptional = item.ParameterDescriptor.IsOptional,
                        Description = item.Documentation
                    });
                }
                else
                {
                    SetRequestParameters(item.ParameterDescriptor.ParameterType);
                }
            }
        }

        private void SetRequestParameters(Type type, string parentName = null)
        {
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    RequestParameters.Add(new ApiDocumentParameter
                    {
                        Type = GetTypeName(prop.PropertyType),
                        Name = (parentName == null ? string.Empty : parentName + ".") + prop.Name,
                        IsOptional = !prop.IsDefined(typeof(ParaRequiredAttribute), false),
                        Description = GetDisplayName(prop)
                    });
                }
                else
                {
                    RequestParameters.Add(new ApiDocumentParameter
                    {
                        Type = prop.PropertyType.Name,
                        Name = prop.Name,
                        IsOptional = !prop.IsDefined(typeof(ParaRequiredAttribute), false),
                        Description = GetDisplayName(prop)
                    });
                    SetRequestParameters(prop.PropertyType, prop.Name);
                }
            }
        }

        private void SetResponseParameters(Type type, int level = 0)
        {
            var props = type.GetProperties();
            if (type.IsValueType || type == typeof(string))
            {
                ResponseParameters.Add(new ApiDocumentParameter
                {
                    Type = type.Name,
                    Description = "返回结果"
                });
                return;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName.EndsWith("ResponsePackage`1"))
            {
                ResultTypeIsResponsePackage = true;
            }
            foreach (var prop in props.Where(prop => prop.Name != "ExtensionData"))
            {
                if (prop.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    ResponseParameters.Add(new ApiDocumentParameter
                    {
                        Type = GetTypeName(prop.PropertyType),
                        Name = prop.Name,
                        Description = GetDisplayName(prop)
                    });
                    var realType = prop.PropertyType.GetGenericArguments()[0];
                    ResponseParameters.Add(new ApiDocumentParameter
                    {
                        Name = GetSpace(++level) + realType.Name,
                        Type = realType.Name
                    });
                    SetResponseParameters(realType, ++level);
                }
                else if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    ResponseParameters.Add(new ApiDocumentParameter
                    {
                        Type = prop.PropertyType.Name,
                        Name = GetSpace(level) + prop.Name,
                        Description = GetDisplayName(prop)
                    });
                }
                else
                {
                    var name = GetTypeName(prop.PropertyType);
                    ResponseParameters.Add(new ApiDocumentParameter
                    {
                        Type = name,
                        Name = GetSpace(level) + prop.Name,
                        Description = GetDisplayName(prop)
                    });
                    SetResponseParameters(prop.PropertyType, ++level);
                }
            }
        }

        private static string GetDisplayName(PropertyInfo prop)
        {
            var displayAttributes = prop.GetCustomAttributes(typeof(DisplayAttribute), false);
            {
                var displayAttribute = displayAttributes.FirstOrDefault() as DisplayAttribute;
                if (displayAttribute != null)
                {
                    return displayAttribute.Name;
                }
            }
            return string.Empty;
        }

        private static string GetTypeName(Type type)
        {
            var name = type.Name;
            while (type.IsGenericType)
            {
                name = name.Replace("`1", "<" + type.GetGenericArguments()[0].Name + ">");
                type = type.GetGenericArguments()[0];
            }
            return name;
        }

        private static string GetSpace(int level)
        {
            var sb = new StringBuilder(level * 6);
            for (var i = 0; i < level; i++)
            {
                sb.Append("&nbsp;");
            }
            return sb.ToString();
        }
    }
}
