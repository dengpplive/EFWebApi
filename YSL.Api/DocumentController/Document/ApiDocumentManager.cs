using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YSL.Common.MessagePackage;

namespace YSL.Api.Document
{
    /// <summary>
    /// Api文档管理器
    /// </summary>
    public class ApiDocumentManager
    {
        static Dictionary<Type, object> _ObjectSampleDictonary = new Dictionary<Type, object>();

        static ApiDocumentManager()
        {
            var documentTypes = typeof(IDocument).Assembly.GetTypes().Where(t => typeof(IDocument).IsAssignableFrom(t));
            foreach (var documentType in documentTypes)
            {
                if (documentType.IsInterface)
                {
                    continue;
                }
                var instance = (IDocument)Activator.CreateInstance(documentType);
                RegisterSampleObject(instance.GetSampleObject());
            }
            RegisterCommonSampleObject();
        }

        internal static void RegisterCommonSampleObject()
        {
            RegisterSampleObject(new DataPackage<decimal> { Data = 120, ExtensionData = new ResponseExtensionData { CallResult = CallResult.Success } });
            RegisterSampleObject(new DataPackage<double> { Data = 120.0, ExtensionData = new ResponseExtensionData { CallResult = CallResult.Success } });
            RegisterSampleObject(new DataPackage<object> { Data = null, ExtensionData = new ResponseExtensionData { CallResult = CallResult.Success } });
        }

        public static void RegisterSampleObject<T>(T sample)
        {
            var type = typeof(T);
            if (_ObjectSampleDictonary.ContainsKey(type))
            {
                return;
            }
            _ObjectSampleDictonary.Add(type, sample);
        }

        public static void RegisterSampleObject(object sample)
        {
            var type = sample.GetType();
            if (_ObjectSampleDictonary.ContainsKey(type))
            {
                return;
            }
            _ObjectSampleDictonary.Add(type, sample);
        }

        /// <summary>
        /// 获取样例对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnNullIfNotExists"></param>
        /// <returns></returns>
        public static object GetSampleObject(Type type, bool returnNullIfNotExists = false)
        {
            if (_ObjectSampleDictonary.ContainsKey(type))
            {
                return _ObjectSampleDictonary[type];
            }
            if (returnNullIfNotExists)
            {
                return null;
            }
            if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                var genericType = type.GetGenericArguments()[0];
                var array = Array.CreateInstance(genericType, 1);
                array.SetValue(CreateInstance(genericType), 0);
                return array;
            }
            return CreateInstance(type);
        }

        private static object CreateInstance(Type type)
        {
            if (type == typeof(string))
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }
    }
}
