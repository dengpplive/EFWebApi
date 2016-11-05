#region File Comments

// ////////////////////////////////////////////////////////////////////////////////////////////////
// file：Izual.Core.ReflectionExtensions.cs
// description：
// 
// create by：Izual ,2012/06/11
// last modify：Izual ,2012/07/05
// ////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 在使用反射的场景，针对 Type，Object 类型的扩展
    /// </summary>
    public static class ReflectionExtensions {
        private static readonly Dictionary<int, Dictionary<int, object>> cache = new Dictionary<int, Dictionary<int, object>>();
        //private static Dictionary<int, Dictionary<int, bool>> attrExists = new Dictionary<int, Dictionary<int, bool>>();
        private static readonly object cachelock = new object();
        private static readonly object itemlock = new object();

        /// <summary>
        /// 创建一个表示调用构造函数的委托（带有一个object[] 类型的参数，并且返回一个 object 类型的结果）
        /// </summary>
        /// <param name="ctorInfo"> 通过反射获取到的构造函数信息 </param>
        /// <returns> 用于调用构造函数的委托 </returns>
        private static Func<object[], object> CreateConstructor(ConstructorInfo ctorInfo) {
            // 返回类型 Func<object[],object> , 表示一个具有一个 object[] 类型的参数，返回类型为 object 的方法。 
            int typeToken = ctorInfo.ReflectedType.MetadataToken;
            int ctorToken = ctorInfo.MetadataToken;

            if (cache.ContainsKey(typeToken) && cache[typeToken].ContainsKey(ctorToken)) {
                return cache[typeToken][ctorToken] as Func<object[], object>;
            }

            // 创建一个表示参数 object[] 的表达式
            ParameterExpression paramsExpression = Expression.Parameter(typeof(object[]), "parameters");

            // 创建一个表示被调用方法的参数的表达式列表
            var paramExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = ctorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++) {
                // 创建一个表示对一维数组进行索引运算的表达式（例如：sampleArray[0]）
                BinaryExpression valueObj = Expression.ArrayIndex(paramsExpression, Expression.Constant(i)); // Expression.Constant(i) : 创建一个值为 i 的常量表达式

                // 创建一个表示参数类型转换的表达式（在调用时，所有传入的参数都是 object 类型，所以在此需要转换为被调用方法的参数实际的类型）
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

                paramExpressions.Add(valueCast);
            }

            // 创建一个调用带指定参数的构造函数的表达式
            NewExpression ctorExpression = Expression.New(ctorInfo, paramExpressions);

            //var ctorCastExpression = Expression.Convert(ctorExpression, typeof(object));
            Expression<Func<object[], object>> lambda = Expression.Lambda<Func<object[], object>>(ctorExpression, paramsExpression);

            // 编译 lambda 表达式，得到委托 Func<object[],object>
            Func<object[], object> ctor = lambda.Compile();
            if (!cache.ContainsKey(typeToken)) {
                lock (cachelock) {
                    if (!cache.ContainsKey(typeToken)) {
                        cache.Add(typeToken, new Dictionary<int, object>());
                    }
                }
            }
            if (!cache[typeToken].ContainsKey(ctorToken)) {
                lock (itemlock) {
                    if (!cache[typeToken].ContainsKey(ctorToken)) {
                        cache[typeToken].Add(ctorToken, ctor);
                    }
                }
            }
            return ctor;
        }

        /// <summary>
        /// 创建一个表示方法调用的委托（第一个参数表示要调用方法的实例，第二个参数表示要传递到方法的参数列表，第三个参数表示返回值为 object）
        /// </summary>
        /// <param name="methodInfo"> 通过反射获取到的方法信息 </param>
        /// <returns> 用于调用方法的委托 </returns>
        private static Func<object, object[], object> CreateMethod(MethodInfo methodInfo) {
            int typeToken = methodInfo.ReflectedType.MetadataToken;
            int methodToken = methodInfo.MetadataToken;

            if (cache.ContainsKey(typeToken) && cache[typeToken].ContainsKey(methodToken)) {
                return cache[typeToken][methodToken] as Func<object, object[], object>;
            }

            ParameterExpression instExpression = Expression.Parameter(typeof(object), "instance");
            ParameterExpression paramsExpression = Expression.Parameter(typeof(object[]), "parameters");
            var paramExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++) {
                BinaryExpression valueObj = Expression.ArrayIndex(paramsExpression, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                paramExpressions.Add(valueCast);
            }
            UnaryExpression instCast = methodInfo.IsStatic ? null : Expression.Convert(instExpression, methodInfo.ReflectedType);
            MethodCallExpression methodCall = Expression.Call(instCast, methodInfo, paramExpressions);
            Func<object, object[], object> method;
            if (methodCall.Type == typeof(void)) {
                // 创建无返回值的方法调用委托
                Expression<Action<object, object[]>> lambda = Expression.Lambda<Action<object, object[]>>(methodCall, instExpression, paramsExpression);

                Action<object, object[]> execute = lambda.Compile();
                method = (instance, parameters) => {
                    execute(instance, parameters);
                    return null;
                };
            }
            else {
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(castMethodCall, instExpression, paramsExpression);
                method = lambda.Compile();
            }

            if (!cache.ContainsKey(typeToken)) {
                lock (cachelock) {
                    if (!cache.ContainsKey(typeToken)) {
                        cache.Add(typeToken, new Dictionary<int, object>());
                    }
                }
            }
            if (!cache[typeToken].ContainsKey(methodToken)) {
                lock (itemlock) {
                    if (!cache[typeToken].ContainsKey(methodToken)) {
                        cache[typeToken].Add(methodToken, method);
                    }
                }
            }
            return method;
        }

        /// <summary>
        /// 根据提供的自定义特性信息创建特性对象
        /// </summary>
        /// <param name="data"> 自定义特性信息 </param>
        /// <returns> 根据特性信息创建的对象 </returns>
        private static object CreateAttributeInstance(CustomAttributeData data) {
            Func<object[], object> ctor = CreateConstructor(data.Constructor);
            object[] args = data.ConstructorArguments.Select(arg => arg.Value).ToArray();
            object attribute = ctor.Invoke(args);
            if (data.NamedArguments != null)
                foreach (CustomAttributeNamedArgument arg in data.NamedArguments) {
                    attribute.Set(arg.MemberInfo.Name, arg.TypedValue.Value);
                }
            return attribute;
        }

        /// <summary>
        /// 创建一个调用属性的 get 访问器的委托
        /// </summary>
        /// <param name="propInfo"> 通过反射得到的属性信息 </param>
        /// <returns> 用于调用 get 访问器的委托 </returns>
        private static Func<object, object> CreateGetter(PropertyInfo propInfo) {
            if (!propInfo.CanRead) {
                throw new Exception("指定属性不可读（缺少 get 访问器）");
            }

            int typeToken = propInfo.ReflectedType.MetadataToken;
            int getterToken = propInfo.GetGetMethod().MetadataToken;

            if (cache.ContainsKey(typeToken) && cache[typeToken].ContainsKey(getterToken)) {
                return cache[typeToken][getterToken] as Func<object, object>;
            }

            ParameterExpression instExpression = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instCastExpression = propInfo.GetGetMethod(true).IsStatic ? null : Expression.Convert(instExpression, propInfo.ReflectedType);
            MemberExpression accessExpression = Expression.Property(instCastExpression, propInfo);
            UnaryExpression valueCastExpression = Expression.Convert(accessExpression, typeof(object));
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(valueCastExpression, instExpression);
            Func<object, object> getter = lambda.Compile();

            if (!cache.ContainsKey(typeToken)) {
                lock (cachelock) {
                    if (!cache.ContainsKey(typeToken)) {
                        cache.Add(typeToken, new Dictionary<int, object>());
                    }
                }
            }
            if (!cache[typeToken].ContainsKey(getterToken)) {
                lock (itemlock) {
                    if (!cache[typeToken].ContainsKey(getterToken)) {
                        cache[typeToken].Add(getterToken, getter);
                    }
                }
            }
            return getter;
        }

        /// <summary>
        /// 创建一个获取字段值的方法
        /// </summary>
        /// <param name="fieldInfo"> 通过反射得到的字段信息 </param>
        /// <returns> 获取字段值的方法 </returns>
        private static Func<object, object> CreateGetter(FieldInfo fieldInfo) {
            int typeToken = fieldInfo.ReflectedType.MetadataToken;
            int fieldToken = fieldInfo.MetadataToken;

            if (cache.ContainsKey(typeToken) && cache[typeToken].ContainsKey(fieldToken)) {
                return cache[typeToken][fieldToken] as Func<object, object>;
            }

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instance, fieldInfo.ReflectedType);
            MemberExpression fieldAccess = Expression.Field(instanceCast, fieldInfo);
            UnaryExpression castFieldValue = Expression.Convert(fieldAccess, typeof(object));
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instance);

            Func<object, object> getter = lambda.Compile();

            if (!cache.ContainsKey(typeToken)) {
                lock (cachelock) {
                    if (!cache.ContainsKey(typeToken)) {
                        cache.Add(typeToken, new Dictionary<int, object>());
                    }
                }
            }
            if (!cache[typeToken].ContainsKey(fieldToken)) {
                lock (itemlock) {
                    if (!cache[typeToken].ContainsKey(fieldToken)) {
                        cache[typeToken].Add(fieldToken, getter);
                    }
                }
            }
            return getter;
        }

        /// <summary>
        /// 创建一个用于调用属性的 set 访问器的委托
        /// </summary>
        /// <param name="propInfo"> 通过反射得到的属性信息 </param>
        /// <returns> 用于调用 set 访问器的委托 </returns>
        private static Func<object, object[], object> CreateSetter(PropertyInfo propInfo) {
            if (!propInfo.CanWrite) {
                throw new Exception("指定属性不可写（缺少 set 访问器）");
            }
            return CreateMethod(propInfo.GetSetMethod(true));
        }

        /// <summary>
        /// 创建一个用于设置字段值的方法
        /// </summary>
        /// <param name="fieldInfo"> 通过反射得到的字段信息 </param>
        /// <returns> 用于设置字段值的方法 </returns>
        private static Func<object, object, object> CreateSetter(FieldInfo fieldInfo) {
            //只需要一条赋值语句：(object)(((T)instance).field=value);
            if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral) {
                throw new Exception("指定字段不可更改（该字段设置为只能在构造函数中设置或设置为在编译时写入并且不能更改）");
            }

            int typeToken = fieldInfo.ReflectedType.MetadataToken;
            int fieldToken = -1 * fieldInfo.MetadataToken;
            if (cache.ContainsKey(typeToken) && cache[typeToken].ContainsKey(fieldToken)) {
                return cache[typeToken][fieldToken] as Func<object, object, object>;
            }

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            UnaryExpression instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instance, fieldInfo.ReflectedType);
            MemberExpression fieldAccess = Expression.Field(instanceCast, fieldInfo);

            ParameterExpression param = Expression.Parameter(typeof(object), "value");
            UnaryExpression paramCast = Expression.Convert(param, fieldInfo.FieldType);
            BinaryExpression ass = Expression.Assign(fieldAccess, paramCast);
            UnaryExpression assCast = Expression.Convert(ass, typeof(object));
            Expression<Func<object, object, object>> lambda = Expression.Lambda<Func<object, object, object>>(assCast, instance, param);
            Func<object, object, object> setter = lambda.Compile();

            if (!cache.ContainsKey(typeToken)) {
                lock (cachelock) {
                    if (!cache.ContainsKey(typeToken)) {
                        cache.Add(typeToken, new Dictionary<int, object>());
                    }
                }
            }
            if (!cache[typeToken].ContainsKey(fieldToken)) {
                lock (itemlock) {
                    if (!cache[typeToken].ContainsKey(fieldToken)) {
                        cache[typeToken].Add(fieldToken, setter);
                    }
                }
            }
            return setter;
        }

        #region 扩展的方法

        /// <summary>
        /// 获取与指定参数类型匹配的构造函数调用委托
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="argTypes"> 参数类型列表 </param>
        /// <returns> 返回匹配的构造函数 </returns>
        public static Func<object[], object> GetConstructorInvoker(this Type type, params Type[] argTypes) {
            ConstructorInfo ctorInfo = type.GetConstructor(argTypes);
            return ctorInfo == null ? null : CreateConstructor(ctorInfo);
        }

        /// <summary>
        /// 初始化由 Type 对象所表示的类型的新实例。 例如：typeof(string).New('s',5)，将得到一个由 5 个字符 ‘s’ 组成的字符串 "sssss"
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="args"> 初始化新实例时传入的参数。 </param>
        /// <returns> 返回初始化的新实例 </returns>
        public static object New(this Type type, params object[] args) {
            ConstructorInfo ctorInfo = type.GetConstructor(args.Select(arg => arg.GetType()).ToArray());
            if (ctorInfo != null) {
                return CreateConstructor(ctorInfo).Invoke(args);
            }
            throw new ArgumentException("参数不匹配");
        }

        /// <summary>
        /// 判断类型上是否定义了指定的特性
        /// </summary>
        /// <typeparam name="T"> 特性类型 </typeparam>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <returns> 如果当前类型上定义了指定的特性，返回 true，否则返回 false </returns>
        public static bool HasAttribute<T>(this Type type) {
            return HasAttribute(type, typeof(T));
        }

        /// <summary>
        /// 判断类型上是否定义了指定的特性
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="attributeType"> 特性类型 </param>
        /// <returns> 如果当前类型上定义了指定的特性，返回 true，否则返回 false </returns>
        public static bool HasAttribute(this Type type, Type attributeType) {
            IList<CustomAttributeData> datas = type.GetCustomAttributesData();
            return datas.Any(data => data.Constructor.ReflectedType == attributeType);
        }

        /// <summary>
        /// 判断类型上是否定义了指定的特性
        /// </summary>
        /// <param name="member"> 扩展的 MemberInfo 对象 </param>
        /// <param name="attributeType"> 特性类型 </param>
        /// <returns> 如果当前类型上定义了指定的特性，返回 true，否则返回 false </returns>
        public static bool HasAttribute(this MemberInfo member, Type attributeType) {
            IList<CustomAttributeData> datas = member.GetCustomAttributesData();
            return datas.Any(data => data.Constructor.ReflectedType == attributeType);
        }

        /// <summary>
        /// 判断类型上是否定义了指定的特性
        /// </summary>
        /// <param name="member"> 扩展的 MemberInfo 对象 </param>
        /// <returns> 如果当前类型上定义了指定的特性，返回 true，否则返回 false </returns>
        public static bool HasAttribute<T>(this MemberInfo member) {
            return member.HasAttribute(typeof(T));
        }

        /// <summary>
        /// 获取类型上指定类型自定义特性
        /// </summary>
        /// <typeparam name="T"> 自定义特性的类型 </typeparam>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有类型为 T 的自定义特性对象 </returns>
        //public static T[] GetAttributes<T>(this Type type) where T : Attribute {
        //    return (from data in type.GetCustomAttributesData()
        //            where data.Constructor.ReflectedType == typeof(T)
        //            select CreateAttributeInstance(data) as T).ToArray();
        //}

        /// <summary>
        /// 获取类型上的所有自定义特性
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有自定义特性对象 </returns>
        public static Attribute[] GetAttributes(this Type type) {
            return type.GetCustomAttributesData().Select(data => CreateAttributeInstance(data) as Attribute).ToArray();
        }

        /// <summary>
        /// 获取方法指定重载的调用委托
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="name"> 方法名称 </param>
        /// <param name="argTypes"> 参数类型列表 </param>
        /// <returns> 返回方法与参数类型列表匹配的重载的调用委托 </returns>
        public static Func<object, object[], object> GetMethodInvoker(this Type type, string name, params Type[] argTypes) {
            MethodInfo methodInfo = type.GetMethod(name, argTypes);
            return methodInfo == null ? null : CreateMethod(methodInfo);
        }

        /// <summary>
        /// 判断当前类型是否实现指定接口
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="interfaceType"> 判断是否实现的接口类型 </param>
        /// <returns> 如果当前类型实现指定接口，返回 true，否则返回 false </returns>
        public static bool Implements(this Type type, Type interfaceType) {
            if (interfaceType == null) {
                throw new ArgumentNullException("interfaceType");
            }
            if (interfaceType.IsInterface) {
                return type.GetInterface(interfaceType.Name) != null;
            }
            return false;
        }

        /// <summary>
        /// 获取指定属性的 Get 访问器
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="name"> 属性名称 </param>
        /// <returns> 返回指定属性的 Get 访问器 </returns>
        public static Func<object, object> GetGetter(this Type type, string name) {
            PropertyInfo prop = type.GetProperty(name);
            return prop == null ? null : (prop.CanRead ? CreateGetter(prop) : null);
        }

        /// <summary>
        /// 获取指定属性的 Set 访问器
        /// </summary>
        /// <param name="type"> 扩展的 Type 对象 </param>
        /// <param name="name"> 属性名称 </param>
        /// <returns> 返回指定属性的 Set 访问器 </returns>
        public static Func<object, object[], object> GetSetter(this Type type, string name) {
            PropertyInfo prop = type.GetProperty(name);
            return prop == null ? null : (prop.CanWrite ? CreateSetter(prop) : null);
        }

        /// <summary>
        /// 通过名称调用指定的方法
        /// </summary>
        /// <param name="obj"> 扩展的 object 实例 </param>
        /// <param name="name"> 要调用的方法的名称 </param>
        /// <param name="args"> 调用方法时传入的参数 </param>
        /// <returns> 返回函数调用结果 </returns>
        /// <remarks>
        /// 最终会通过方法名称，传入的参数数量及类型来确定调用的方法或其重载
        /// </remarks>
        public static object Call(this object obj, string name, params object[] args) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException("name");
            }

            MethodInfo methodInfo = obj.GetType().GetMethod(name, args.Select(arg => arg.GetType()).ToArray());
            if (methodInfo != null) {
                return CreateMethod(methodInfo).Invoke(obj, args);
            }
            throw new ArgumentException(string.Format("缺少名为 \"{0}\" 的方法，或对应的重载", name));
        }

        /// <summary>
        /// 通过名称设置对象上指定属性或字段的值
        /// </summary>
        /// <param name="obj"> 扩展的 object 对象 </param>
        /// <param name="name"> 属性名称 </param>
        /// <param name="value"> 设置的值 </param>
        public static void Set(this object obj, string name, object value) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException("name");
            }
            Type type = obj.GetType();
            PropertyInfo propInfo = type.GetProperty(name);
            if (propInfo != null) {
                CreateSetter(propInfo).Invoke(obj, new[] { value });
            }
            else {
                FieldInfo fieldInfo = type.GetField(name);
                if (fieldInfo != null) {
                    CreateSetter(fieldInfo).Invoke(obj, value);
                }
                else {
                    throw new ArgumentException(string.Format("不存在名为 \"{0}\" 的属性或字段", name));
                }
            }
        }
        /// <summary>
        /// 通过名称获取对象上指定属性或字段的值。
        /// </summary>
        /// <param name="obj"> 要获取属性值的对象 </param>
        /// <param name="name"> 属性名称 </param>
        /// <returns> 获取到的属性值 </returns>
        public static object Get(this object obj, string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException("name");
            }
            Type type = obj.GetType();

            PropertyInfo propInfo = type.GetProperty(name);
            if (propInfo != null) {
                return CreateGetter(propInfo).Invoke(obj);
            }
            FieldInfo fieldInfo = type.GetField(name);
            if (fieldInfo != null) {
                return CreateGetter(fieldInfo).Invoke(obj);
            }
            throw new Exception(string.Format("不存在名为 \"{0}\" 的属性或字段", name));
        }

        /// <summary>
        /// 获取属性上指定类型自定义特性
        /// </summary>
        /// <typeparam name="T"> 自定义特性的类型 </typeparam>
        /// <param name="prop"> 扩展的 PropertyInfo 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有类型为 T 的自定义特性对象 </returns>
        public static T[] GetAttributes<T>(this PropertyInfo prop) where T : Attribute {
            IList<CustomAttributeData> datas = prop.GetCustomAttributesData();
            return (from data in datas
                    where data.Constructor.ReflectedType == typeof(T)
                    select CreateAttributeInstance(data) as T).ToArray();
        }
        /// <summary>
        /// 获取方法上指定类型自定义特性
        /// </summary>
        /// <typeparam name="T"> 自定义特性的类型 </typeparam>
        /// <param name="method"> 扩展的 MethodInfo 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有类型为 T 的自定义特性对象 </returns>
        public static T[] GetAttributes<T>(this MethodInfo method) where T : Attribute {
            IList<CustomAttributeData> datas = method.GetCustomAttributesData();
            return (from data in datas
                    where data.Constructor.ReflectedType == typeof(T)
                    select CreateAttributeInstance(data) as T).ToArray();
        }
        /// <summary>
        /// 获取方法上指定类型自定义特性
        /// </summary>
        /// <typeparam name="T"> 自定义特性的类型 </typeparam>
        /// <param name="field"> 扩展的 MethodInfo 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有类型为 T 的自定义特性对象 </returns>
        public static T[] GetAttributes<T>(this FieldInfo field) where T : Attribute {
            IList<CustomAttributeData> datas = field.GetCustomAttributesData();
            return (from data in datas
                    where data.Constructor.ReflectedType == typeof(T)
                    select CreateAttributeInstance(data) as T).ToArray();
        }
        /// <summary>
        /// 获取方法上指定类型自定义特性
        /// </summary>
        /// <typeparam name="T"> 自定义特性的类型 </typeparam>
        /// <param name="member"> 扩展的 MethodInfo 对象 </param>
        /// <returns> 返回一个数组，其中包含在当前类型上定义的，所有类型为 T 的自定义特性对象 </returns>
        //public static T[] GetAttributes<T>(this MemberInfo member) where T : Attribute {
        //    IList<CustomAttributeData> datas = member.GetCustomAttributesData();
        //    return (from data in datas
        //            where data.Constructor.ReflectedType == typeof(T)
        //            select CreateAttributeInstance(data) as T).ToArray();
        //}

        #endregion

        public static object GetValue(this MemberInfo member, object instance)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetValue(instance, null);
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(instance);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static void SetValue(this MemberInfo member, object instance, object value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    var pi = (PropertyInfo)member;
                    pi.SetValue(instance, value, null);
                    break;
                case MemberTypes.Field:
                    var fi = (FieldInfo)member;
                    fi.SetValue(instance, value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}