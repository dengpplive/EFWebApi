using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YSL.Common.Exceptions;
using YSL.Common.Extender;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 在针对成员进行处理时，指定处理哪些类型的成员。
    /// </summary>
    [Flags]
    public enum MemberTypeBinding {
        /// <summary>
        /// 字段。
        /// </summary>
        Field = 0x1,
        /// <summary>
        /// 属性。
        /// </summary>
        Property = 0x2,
        /// <summary>
        /// 方法。
        /// </summary>
        Method = 0x4,
        /// <summary>
        /// 事件。
        /// </summary>
        Event = 0x8,
        /// <summary>
        /// 数据成员（字段和属性）。
        /// </summary>
        DataMember = Field | Property,
        /// <summary>
        /// 全部。
        /// </summary>
        All = Field | Property | Method | Event
    }

    [Flags]
    public enum MemberBinding {
        /// <summary>
        /// 实例成员
        /// </summary>
        Instance,
        /// <summary>
        /// 静态成员
        /// </summary>
        Static,

        All = Instance | Static
    }
    /// <summary>
    /// 类型代理类，
    /// </summary>
    public class TypeProxy {
        #region Static Members
        /// <summary>
        /// 获取指定类型的类型代理。
        /// </summary>
        /// <param name="type">要获取代理的类型。</param>
        /// <returns>返回类型 type 的代理类。</returns>
        public static TypeProxy GetProxy(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            return proxies[type, t => new TypeProxy(t)];
        }

        /// <summary>
        /// 获取指定类型的类型代理。
        /// </summary>
        /// <typeparam name="T">要获取代理的类型。</typeparam>
        /// <returns>返回类型 T 的代理类。</returns>
        public static TypeProxy GetProxy<T>() {
            return GetProxy(typeof(T));
        }

        private static readonly KeyValueCache<Type, TypeProxy> proxies = new KeyValueCache<Type, TypeProxy>();
        #endregion

        /// <summary>
        /// 获取当前 TypeProxy 所代理的类型。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取类型上指定的特性。
        /// </summary>
        /// <param name="attributeType">特性类型。如果为 null，表示不指定类型，获取所有定义在类型上的特性。</param>
        /// <returns>返回获取到的特性实例的列表。</returns>
        public object GetAttributes(Type attributeType = null) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定成员上指定的特性。
        /// </summary>
        /// <param name="name">成员名称。以 ".ctor" 表示构造函数。</param>
        /// <param name="attributeType"></param>
        /// <returns>返回获取到的特性实例的列表。</returns>
        public object GetMemberAttributes(string name, Type attributeType) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建当前所代理的类型的新实例。
        /// </summary>
        /// <param name="args">创建新实例需要的参数。</param>
        /// <returns>返回新创建的实例。</returns>
        public object CreateInstance(params object[] args) {
            var ctor = ctors[GetConstructor(args), GetConstructorInvoker];
            return ctor(args);
        }

        /// <summary>
        /// 创建当前所代理的类型的新实例序列。
        /// </summary>
        /// <param name="count">要创建的序列中包含的元素数量。</param>
        /// <param name="args">创建实例需要的参数。</param>
        /// <returns>返回新创建的实例序列。</returns>
        public IEnumerable<object> CreateInstances(int count, params object[] args) {
            var ctor = ctors[GetConstructor(args), GetConstructorInvoker];
            for (var i = 0; i < count; i++) {
                yield return ctor(args);
            }
        }

        /// <summary>
        /// 根据名称获取指定属性或字段的值。
        /// </summary>
        /// <param name="instance">要获取属性或字段值的实例。</param>
        /// <param name="name">属性或字段的名称。</param>
        /// <param name="ignoreCase">指定是否忽略属性或字段名称的大小写。</param>
        /// <returns>返回该属性或字段的值。</returns>
        public object GetValue(object instance, string name, bool ignoreCase = false) {
            var comparer = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var m = datas.FirstOrDefault(mi => mi.Name.Equals(name, comparer));
            if (m == null) {
                throw new MemberNotExistsException(name);
            }
            var getter = getters[m, TypeHelper.GetPropertyOrFieldGetter];
            return getter(instance);
        }

        /// <summary>
        /// 获取索引属性的值。
        /// </summary>
        /// <param name="instance">要获取索引属性值的实例。</param>
        /// <param name="indexes">索引列表。</param>
        /// <returns>返回索引属性对应索引处的值。</returns>
        public object GetValue(object instance, params object[] indexes) {
            if (indexes == null || indexes.Length == 0) {
                throw new MissingIndexParameterException();
            }
            var pi = GetIndexer(indexes);
            if (pi != null) {
                var getter = igetters[pi, GetIndexerGetter];
                return getter(instance, indexes);
            }
            throw new IndexerNotExistsException(indexes);
        }

        /// <summary>
        /// 根据名称设置指定属性或字段的值。
        /// </summary>
        /// <param name="instance">要设置属性或字段值的实例。</param>
        /// <param name="name">属性或字段的名称。</param>
        /// <param name="ignoreCase">指定是否忽略属性或字段名称的大小写。</param>
        /// <param name="value">要设置的值。</param>
        public void SetValue(object instance, string name, object value, bool ignoreCase = false) {
            if (value == null) { return; }

            var comparer = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var m = datas.FirstOrDefault(mi => mi.Name.Equals(name, comparer));
            if (m == null) {
                throw new MemberNotExistsException(name);
            }
            var setter = setters[m, TypeHelper.GetPropertyOrFieldSetter];
            var desType = GetMemberType(m.Name);

            var srcType = value.GetType();
            if (!desType.IsAssignableFrom(srcType)) {
                if (desType == typeof(string)) {
                    var toString = srcType.GetMethod("ToString", Type.EmptyTypes);
                    value = toString.Invoke(value, null);
                }
                else if (desType.IsEnumerable() && srcType.IsEnumerable()) {
                    var etype = TypeExtension.GetElementType(desType);
                    var cast = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(etype);
                    value = cast.Invoke(null, new[] { value });
                    if (desType.IsArray) {
                        var toArray = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(etype);
                        value = toArray.Invoke(null, new[] { value });
                    }
                    if (desType.IsGenericType && (desType.GetGenericTypeDefinition() == typeof(IList<>) || desType.GetGenericTypeDefinition() == typeof(List<>))) {
                        var toList = typeof(Enumerable).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(etype);
                        value = toList.Invoke(null, new[] { value });
                    }
                }
                else if (srcType == typeof(string)) {
                    var strValue = value.ToString();
                    if (desType == typeof(Guid)) {
                        value = string.IsNullOrWhiteSpace(strValue) ? Guid.Empty : Guid.Parse(strValue);
                    }
                    else {
                        var parse = desType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { srcType }, null);
                        if (parse != null) {
                            value = parse.Invoke(null, new[] { value });
                        }
                        else {
                            throw new InvalidCastException(string.Format("无法将 \"{0}\" 类型的值转换为 \"{1}\" 类型。", srcType, desType));
                        }
                    }
                }
            }
            setter(instance, value);
        }
        /// <summary>
        /// 根据名称设置指定索引属性的值。
        /// </summary>
        /// <param name="instance">要设置索引属性值的实例。</param>
        /// <param name="value">要设置的值。</param>
        /// <param name="indexes">索引列表。</param>
        public void SetValue(object instance, object value, params object[] indexes) {
            var pi = GetIndexer(indexes);
            if (pi != null) {
                var setter = isetters[pi, GetIndexerSetter];
                setter(instance, value, indexes);
            }
        }

        /// <summary>
        /// 根据名称调用指定的方法。
        /// </summary>
        /// <param name="instance">要调用方法的实例。</param>
        /// <param name="name">方法的名称。</param>
        /// <param name="ignoreCase">指定是否忽略方法名称的大小写。</param>
        /// <param name="args">调用方法时要传入的参数。</param>
        /// <returns>返回被调用方法的返回值。</returns>
        public object Invoke(object instance, string name, bool ignoreCase = false, params object[] args) {
            var comparer = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var pts = args.Select(p => p.GetType()).ToArray();
            var m = methods.First(mi => mi.Name.Equals(name, comparer) && mi.GetParameters().Select(pi => pi.ParameterType).ToArray().Equals(pts));
            var invoker = invokers[m, GetMethodInvoker];
            return invoker(instance, args);
        }

        /// <summary>
        /// 根据名称调用指定的方法。
        /// </summary>
        /// <param name="instance">要调用方法的实例。</param>
        /// <param name="name">方法的名称。</param>
        /// <param name="args">调用方法时要传入的参数。</param>
        /// <returns>返回被调用方法的返回值。</returns>
        public object Invoke(object instance, string name, params object[] args) {
            return Invoke(instance, name, false, args);
        }

        /// <summary>
        /// 判断当前类型中，是否存在指定名称的成员。
        /// </summary>
        /// <param name="name">要搜索的成员名称。</param>
        /// <param name="binding">指定搜索那些类型的成员。</param>
        /// <param name="ignoreCase">指定是否忽略大小写。</param>
        /// <returns>如果在指定的成员类型中搜索到该成员名称，返回 true；否则返回 false。</returns>
        public bool Contains(string name, MemberTypeBinding binding = MemberTypeBinding.All, bool ignoreCase = false) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var comparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return (binding & MemberTypeBinding.Property) == MemberTypeBinding.Property && props.Any(pi => pi.Name.Equals(name, comparison))
                || (binding & MemberTypeBinding.Field) == MemberTypeBinding.Field && fields.Any(fi => fi.Name.Equals(name, comparison))
                || (binding & MemberTypeBinding.Method) == MemberTypeBinding.Method && methods.Any(mi => mi.Name.Equals(name, comparison))
                || (binding & MemberTypeBinding.Event) == MemberTypeBinding.Event && events.Any(ei => ei.Name.Equals(name, comparison));
        }

        /// <summary>
        /// 获取指定类型的成员名称。
        /// </summary>
        /// <param name="typeBinding">成员类型。</param>
        /// <param name="includeStatic">指定返回的名称中，是否包含静态成员。</param>
        /// <param name="ignoreCase">是否忽略成员名称大小写。</param>
        /// <returns>返回指定类型的所有成员的名称。</returns>
        public IEnumerable<string> GetMemberNames(MemberTypeBinding typeBinding = MemberTypeBinding.All, bool includeStatic = true, bool ignoreCase = false) {
            var result = Enumerable.Empty<string>();
            var comparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            if ((typeBinding & MemberTypeBinding.Property) == MemberTypeBinding.Property) {
                result = result.Concat(props.Where(pi => includeStatic || !pi.IsStatic()).Select(pi => pi.Name));
            }
            if ((typeBinding & MemberTypeBinding.Field) == MemberTypeBinding.Field) {
                result = result.Concat(fields.Where(fi => includeStatic || !fi.IsStatic).Select(fi => fi.Name));
            }
            if ((typeBinding & MemberTypeBinding.Method) == MemberTypeBinding.Method) {
                result = result.Concat(methods.Where(mi => includeStatic || !mi.IsStatic).Select(mi => mi.Name));
            }
            if ((typeBinding & MemberTypeBinding.Event) == MemberTypeBinding.Event) {
                result = result.Concat(events.Select(ei => ei.Name));
            }
            return Enumerable.Distinct(result, comparer).ToArray();
        }

        /// <summary>
        /// 获取指定类型的成员名称。
        /// </summary>
        /// <param name="typeBinding">成员类型。</param>
        /// <param name="memberBinding"></param>
        /// <param name="ignoreCase">是否忽略成员名称大小写。</param>
        /// <returns>返回指定类型的所有成员的名称。</returns>
        //public IEnumerable<string> GetMemberNames(MemberTypeBinding typeBinding = MemberTypeBinding.All, MemberBinding memberBinding = MemberBinding.All, bool ignoreCase = false) {
        //throw new NotImplementedException();
        //var result = Enumerable.Empty<string>();
        //var comparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        //if ((typeBinding & MemberTypeBinding.Property) == MemberTypeBinding.Property) {
        //    result = result.Concat(props.Where(pi => inclueStatic || !pi.IsStatic()).Select(pi => pi.Name));
        //}
        //if ((typeBinding & MemberTypeBinding.Field) == MemberTypeBinding.Field) {
        //    result = result.Concat(fields.Where(fi => inclueStatic || !fi.IsStatic).Select(fi => fi.Name));
        //}
        //if ((typeBinding & MemberTypeBinding.Method) == MemberTypeBinding.Method) {
        //    result = result.Concat(methods.Where(mi => inclueStatic || !mi.IsStatic).Select(mi => mi.Name));
        //}
        //if ((typeBinding & MemberTypeBinding.Event) == MemberTypeBinding.Event) {
        //    result = result.Concat(events.Select(ei => ei.Name));
        //}
        //return result.Distinct(comparer).ToArray();
        //}


        /// <summary>
        /// 获取成员的数据类型。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <returns>返回该成员的数据类型。</returns>
        public Type GetMemberType(string name) {
            var mi = Type.GetMember(name).FirstOrDefault();
            if (mi == null) {
                throw new MemberNotExistsException();
            }
            return GetMemberType(mi);
        }

        /// <summary>
        /// 获取成员在类型中定义的类型，如：属性，字段，方法，事件。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <returns>返回成员在类型中定义的类型。</returns>
        public MemberTypes GetDeclarationMemberType(string name) {
            var m = Type.GetMember(name).FirstOrDefault();
            if (m != null) {
                return m.MemberType;
            }
            throw new MemberNotExistsException(name);
        }

        /// <summary>
        /// 根据提供的 type，初始化 TypeProxy 类型的新实例。
        /// </summary>
        /// <param name="type">要代理的类型。</param>
        private TypeProxy(Type type) {
            Type = type;
            props = type.GetProperties().Where(pi => pi.GetIndexParameters().Length == 0);
            indexers = type.GetProperties().Where(pi => pi.GetIndexParameters().Length > 0);
            fields = type.GetFields();
            methods = type.GetMethods();
            events = type.GetEvents();
            datas = props.Cast<MemberInfo>().Concat(fields);
        }

        private readonly IEnumerable<PropertyInfo> props;
        private readonly IEnumerable<PropertyInfo> indexers;
        private readonly IEnumerable<FieldInfo> fields;
        private readonly IEnumerable<MethodInfo> methods;
        private readonly IEnumerable<EventInfo> events;
        private readonly IEnumerable<MemberInfo> datas;
        private readonly KeyValueCache<MemberInfo, Func<object, object>> getters = new KeyValueCache<MemberInfo, Func<object, object>>();
        private readonly KeyValueCache<MemberInfo, Action<object, object>> setters = new KeyValueCache<MemberInfo, Action<object, object>>();
        private readonly KeyValueCache<MethodInfo, Func<object, object[], object>> invokers = new KeyValueCache<MethodInfo, Func<object, object[], object>>();
        private readonly KeyValueCache<ConstructorInfo, Func<object[], object>> ctors = new KeyValueCache<ConstructorInfo, Func<object[], object>>();
        //private readonly KeyValueCache<EventInfo, Delegate> events = new KeyValueCache<EventInfo, Delegate>();
        private readonly KeyValueCache<PropertyInfo, Func<object, object[], object>> igetters = new KeyValueCache<PropertyInfo, Func<object, object[], object>>();
        private readonly KeyValueCache<PropertyInfo, Action<object, object, object[]>> isetters = new KeyValueCache<PropertyInfo, Action<object, object, object[]>>();

        /// <summary>
        /// 获取调用指定构造函数的委托。
        /// </summary>
        /// <param name="ctor">构造函数信息。</param>
        /// <returns>返回一个 Func&lt;object[],object&gt; 类型的委托。</returns>
        /// <exception cref="ArgumentNullException">在构造函数信息 ctor 为 null 时引发的异常。</exception>
        private Func<object[], object> GetConstructorInvoker(ConstructorInfo ctor) {
            if (ctor == null) {
                throw new ArgumentNullException("ctor");
            }
            return TypeHelper.CreateConstructorInvoker(ctor);
        }

        /// <summary>
        /// 获取调用指定方法的委托。
        /// </summary>
        /// <param name="method">方法信息。</param>
        /// <returns>返回一个 Func&lt;object,object[],object&gt; 类型的委托。</returns>
        private Func<object, object[], object> GetMethodInvoker(MethodInfo method) {
            if (method != null) {
                return TypeHelper.CreateMethodInvoker(method);
            }
            throw new MethodNotExistsException();
        }

        /// <summary>
        /// 获取访问指定索引器的委托。
        /// </summary>
        /// <param name="prop"></param>
        /// <returns>返回一个 Func&lt;object,object[],object&gt; 类型的委托。</returns>
        private Func<object, object[], object> GetIndexerGetter(PropertyInfo prop) {
            if (prop != null) {
                if (prop.GetIndexParameters().Length > 0) {
                    return TypeHelper.CreatePropertyGetter(prop);
                }
                throw new InvalidOperationException("\"" + prop.Name + "\" 不是索引器。");
            }
            throw new IndexerNotExistsException();
        }

        /// <summary>
        /// 获取访问指定索引器的委托。
        /// </summary>
        /// <param name="prop">索引列表。</param>
        /// <returns>返回一个 Action&lt;object, object, object[]&gt; 类型的委托。</returns>
        private Action<object, object, object[]> GetIndexerSetter(PropertyInfo prop) {
            if (prop != null) {
                return TypeHelper.CreatePropertySetter(prop);
            }
            throw new IndexerNotExistsException();
        }

        /// <summary>
        /// 根据指定的索引列表，获取对应的索引器。
        /// </summary>
        /// <param name="indexes">索引列表。</param>
        /// <returns>返回与传入索引列表匹配的重载版本的索引器信息，如果韦昭到匹配的索引器，返回 null。</returns>
        private PropertyInfo GetIndexer(params object[] indexes) {
            return Type.GetProperty("Item", indexes.Select(idx => idx.GetType()).ToArray());
        }

        /// <summary>
        /// 根据传入的参数，获取指定重载版本的构造函数信息。
        /// </summary>
        /// <param name="args">调用构造函数时传入的参数。</param>
        /// <returns>返回与传入参数相匹配的重载版本的构造函数信息，如果未找到匹配的构造函数，返回 null。</returns>
        private ConstructorInfo GetConstructor(IEnumerable<object> args) {
            var types = args == null ? Type.EmptyTypes : args.Select(arg => arg.GetType()).ToArray();
            return Type.GetConstructor(types);
        }

        /// <summary>
        /// 获取成员的数据类型。
        /// </summary>
        /// <param name="mi">成员信息。</param>
        /// <returns>返回成员的类型信息。</returns>
        private Type GetMemberType(MemberInfo mi) {
            switch (mi.MemberType) {
                case MemberTypes.Field:
                    return ((FieldInfo)mi).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)mi).PropertyType;
                case MemberTypes.Method:
                    return ((MethodInfo)mi).ReturnType;
                case MemberTypes.TypeInfo:
                    return (Type)mi;
                case MemberTypes.Event:
                    return ((EventInfo)mi).EventHandlerType;
                default:
                    throw new InvalidMemberTypeException();
            }
        }
    }
}
