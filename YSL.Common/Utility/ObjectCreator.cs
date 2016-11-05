using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 对象创建器，提高通过反射创建对象的性能
    /// </summary>
    public static class ObjectCreator {
        private static object clocker = new object();
        private static object dclocker = new object();
        private static Dictionary<ConstructorInfo, Func<object[], object>> creators = new Dictionary<ConstructorInfo, Func<object[], object>>();
        private static Dictionary<int, Func<object>> defaultCreators = new Dictionary<int, Func<object>>();

        private static Func<object> CreateDefaultConstructor(Type type) {
            if (!defaultCreators.ContainsKey(type.MetadataToken)) {
                lock (dclocker) {
                    if (!defaultCreators.ContainsKey(type.MetadataToken)) {
                        defaultCreators.Add(type.MetadataToken, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
                    }
                }
            }

            return defaultCreators[type.MetadataToken];
        }
        private static Func<object[], object> CreateConstructor(ConstructorInfo ctor) {

            if (ctor == null) {
                throw new ArgumentNullException("ctor");
            }
            var token = ctor.MetadataToken;
            if (!creators.ContainsKey(ctor)) {
                lock (clocker) {
                    if (!creators.ContainsKey(ctor)) {

                        // 创建一个针对构造函数 .ctor(arg0,arg1,arg2,...) 的调用委托
                        var parameters = ctor.GetParameters();
                        // 调用委托时传入的是一个参数数组：args
                        var args = Expression.Parameter(typeof(object[]), "args");
                        // 委托调用构造函数时需要：arg0,arg1,arg2,.......
                        var argList = new List<Expression>();
                        for (var i = 0; i < parameters.Length; i++) {
                            argList.Add(Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i)),parameters[i].ParameterType));
                        }
                        var call = Expression.New(ctor, argList);
                        creators.Add(ctor, Expression.Lambda<Func<object[], object>>(call, args).Compile());
                    }
                }
            }
            return creators[ctor];
        }

        private static ConstructorInfo MatchConstructor(Type type, params object[] args) {
            if (type == null) { throw new ArgumentNullException("type"); }
            var ctors = type.GetConstructors().Where(c => c.GetParameters().Length == (args == null ? 0 : args.Length));

            foreach (var ctor in ctors) {
                var pms = ctor.GetParameters();
                var matched = true;
                for (var i = 0; i < pms.Length; i++) {
                    //if (!args[i].GetType().IsAssignableFrom(pms[i].ParameterType)) {
                    if(!pms[i].ParameterType.IsAssignableFrom(args[i].GetType())){
                        matched = false;
                        break;
                    }
                }
                if (matched) { return ctor; }
            }
            return null;
        }
        private static ConstructorInfo FindConstructor(Type type, params object[] args) {
            var types = args.Select(a => a.GetType()).ToArray();
            var ctor = type.GetConstructor(types);
            //var ctor = MatchConstructor(type, args);
            if (ctor == null) {
                var sb = new StringBuilder();
                sb.Append(".ctor(");
                for (var i = 0; i < types.Length; i++) {
                    if (i > 0) { sb.Append(','); }
                    sb.Append(types[i].FullName);
                }
                sb.Append(")");
                throw new InvalidOperationException(string.Format("类型：\"{0}\" 中，未找到与签名 \"{1}\" 匹配的构造函数。", type.FullName, sb.ToString()));
            }
            return ctor;
        }
        /// <summary>
        /// 创建指定类型的对象实例。
        /// </summary>
        /// <param name="type">要创建实例的类型。</param>
        /// <param name="args">调用构造函数需要传递的参数。</param>
        /// <returns></returns>
        public static object Create(Type type, params object[] args) {
            return args == null || args.Length == 0
                ? CreateDefaultConstructor(type)()
                : CreateConstructor(FindConstructor(type, args))(args);
        }
        /// <summary>
        /// 创建指定类型的对象实例。
        /// </summary>
        /// <typeparam name="T">要创建实例的类型。</typeparam>
        /// <param name="args">调用构造函数需要传递的参数。</param>
        /// <returns></returns>
        public static T Create<T>(params object[] args) {
            return (T)Create(typeof(T), args);
        }
    }
}
