using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Assert;
using YSL.Common.Resources;
using YSL.Common.Extender;
using System.Threading;
using System.Reflection.Emit;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 动态对象，支持obj["PropName"]或者obj.PropName存取值
    /// </summary>
    [Serializable]
    public class Dynamic : DynamicObject
    {
        Dictionary<string, object> dic = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Dynamic()
        {
            dic = new Dictionary<string, object>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">数据</param>
        public Dynamic(Dictionary<string, object> dic)
        {
            this.dic = dic;
        }

        /// <summary>
        /// 属性个数
        /// </summary>
        public int Count
        {
            get
            {
                return dic.Count;
            }
        }

        /// <summary>
        /// 访问索引
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns>属性值</returns>
        public object this[string name]
        {
            get
            {
                AssertUtil.IsTrue(dic.ContainsKey(name), string.Format(Constant.NameNotExist, name));
                return dic[name];
            }
            set
            {
                dic[name] = value;
            }
        }

        /// <summary>
        /// 获取所有属性名
        /// </summary>
        /// <returns>所有属性名</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return dic.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            return dic.TryGetValue(name, out result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dic[binder.Name] = value;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result)
        {
            string name = indexes[0] as string;
            return dic.TryGetValue(name, out result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            string name = indexes[0] as string;
            dic[name] = value;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(Dictionary<string, object>))
            {
                result = this.dic;
                return true;
            }
            return base.TryConvert(binder, out result);
        }

        #region 重载
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static implicit operator Dynamic(Dictionary<string, object> dic)
        {
            if (dic == null)
                return null;
            else
                return new Dynamic(dic);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static implicit operator Dictionary<string, object>(Dynamic d)
        {
            if (d == null)
                return null;
            else
                return d.dic;
        }
        #endregion
    }


    /// <summary>
    /// 提供一个基类，用于表示简单数据对象。简单数据对象可以按名称访问成员，并可以枚举所有数据成员。
    /// </summary>
    public abstract class SimpleDataObject : INamedAccessable, IEnumerable<SimpleDataObject.DataMember>
    {
        /// <summary>
        /// 索引器，为简单数据对象提供按名称访问成员的能力。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <param name="ignoreCase">是否忽略名称大小写，默认 false 为匹配大小写。</param>
        public object this[string name, bool ignoreCase = false]
        {
            get { return this.GetValue(name, ignoreCase); }
            set { this.SetValue(name, value, ignoreCase); }
        }

        #region Implementation of IEnumerable
        public IEnumerator<DataMember> GetEnumerator()
        {
            return new DataMemberCollection(this).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public sealed class DataMember
        {
            public string Name { get; internal set; }
            public MemberTypes Type { get; internal set; }
            public object Value { get; internal set; }
        }
        public sealed class DataMemberCollection : IEnumerable<DataMember>
        {
            private readonly IEnumerable<DataMember> members;
            internal DataMemberCollection(SimpleDataObject owner)
            {
                if (owner == null)
                    throw new ArgumentNullException("owner");
                var proxy = TypeProxy.GetProxy(owner.GetType());
                members = proxy.GetMemberNames(MemberTypeBinding.DataMember).Select(name => new DataMember { Name = name, Type = proxy.GetDeclarationMemberType(name), Value = proxy.GetValue(owner, name) });
            }

            #region Implementation of IEnumerable
            public IEnumerator<DataMember> GetEnumerator()
            {
                return members.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }
    }

    /// <summary>
    /// 扩展 INamedAccessable 接口，提供按名称访问成员的功能。
    /// </summary>
    public static class NamedAccessable
    {
        /// <summary>
        /// 获取当前对象上指定名称的成员的值。
        /// </summary>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">成员名称。</param>
        /// <param name="ignoreCase">指定是否忽略成员名称的大小写。</param>
        /// <returns>返回该成员的值。</returns>
        public static object GetValue(this INamedAccessable instance, string name, bool ignoreCase = false)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            var proxy = TypeProxy.GetProxy(instance.GetType());
            return proxy.GetValue(instance, name, ignoreCase);
        }

        /// <summary>
        /// 获取当前对象上指定名称的成员的值。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">成员名称。</param>
        /// <param name="ignoreCase">指定是否忽略成员名称的大小写。</param>
        /// <returns>返回该成员的值。</returns>
        public static T GetValue<T>(this INamedAccessable instance, string name, bool ignoreCase = false)
        {
            return (T)Convert.ChangeType(GetValue(instance, name, ignoreCase), typeof(T));
        }

        /// <summary>
        /// 设置当前对象上指定名称的成员的值。
        /// </summary>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">成员名称。</param>
        /// <param name="value">要设置的值。</param>
        /// <param name="ignoreCase">指定是否忽略成员名称的大小写。</param>
        public static void SetValue(this INamedAccessable instance, string name, object value, bool ignoreCase = false)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            var proxy = TypeProxy.GetProxy(instance.GetType());
            proxy.SetValue(instance, name, value, ignoreCase);
        }

        /// <summary>
        /// 调用当前对象上指定名称的方法。
        /// </summary>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">成员名称。</param>
        /// <param name="ignoreCase">指定是否忽略成员名称的大小写。</param>
        /// <param name="args">调用方法时传入的参数列表。</param>
        /// <returns>返回该方法调用的返回结果。</returns>
        public static object Invoke(this INamedAccessable instance, string name, bool ignoreCase = false, params object[] args)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            var proxy = TypeProxy.GetProxy(instance.GetType());
            return proxy.Invoke(instance, name, ignoreCase, args);
        }

        /// <summary>
        /// 判断对象是否具有指定名称的成员。
        /// </summary>
        /// <param name="instance">被搜索的对象。</param>
        /// <param name="name">要搜索的成员名称。</param>
        /// <param name="binding">指定搜索对象中那些类型的成员。</param>
        /// <param name="ignoreCase">指定是否忽略成员名称的大小写。</param>
        /// <returns>如果对象中指定类型的成员中存在指定名称的成员，返回 true；否则返回 false。</returns>
        public static object Contains(this INamedAccessable instance, string name, MemberTypeBinding binding = MemberTypeBinding.All, bool ignoreCase = false)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var proxy = TypeProxy.GetProxy(instance.GetType());
            return proxy.Contains(name, binding, ignoreCase);
        }
    }

    /// <summary>
    /// 定义一组方法，提供通过成员名称访问成员的功能。
    /// </summary>
    public interface INamedAccessable { }

    /// <summary>
    /// 辅助工具类，为 Type 类型提供扩展功能。
    /// </summary>
    public static partial class TypeHelper
    {
        /// <summary>
        /// 根据指定的属性名称和类型，创建一个匿名类型。
        /// </summary>
        /// <param name="propNames">属性名称。</param>
        /// <param name="propTypes">属性类型。</param>
        /// <param name="baseType">基类类型。</param>
        /// <param name="interfaceTypes">实现的接口类型。</param>
        /// <param name="propertyReadonly">指定新创建的类型中的属性是否只读。</param>
        /// <returns>返回新创建的类型。</returns>
        public static Type CreateDynamicType(string[] propNames, Type[] propTypes, Type baseType = null, Type[] interfaceTypes = null, bool propertyReadonly = false)
        {
            var info = new DynamicTypeInfo { BaseType = baseType };
            for (var i = 0; i < propNames.Length; i++)
            {
                var propInfo = new PropInfo(propNames[i], propTypes[i]);
                if (!info.Properties.Contains(propInfo))
                    info.Properties.Add(propInfo);
            }
            if (interfaceTypes != null)
            {
                info.Interfaces.AddRange(interfaceTypes);
            }
            return dynamicTypes[info, key => CreateDynamicTypeByTypeInfo(key, propertyReadonly)];
        }

        private static int seed;

        private static readonly KeyValueCache<DynamicTypeInfo, Type> dynamicTypes = new KeyValueCache<DynamicTypeInfo, Type>(new DynamicTypeInfoEqualConparer());

        /// <summary>
        /// 为动态创建的类型创建内部引用的名称。
        /// </summary>
        /// <returns>返回一个字符串用于唯一标识一个动态创建的类型。</returns>
        private static string GetDynamicTypeName()
        {
            //return string.Format("<ChinaPay.Core__dynamictype>[id:{{{0}}}]", Guid.NewGuid());
            return string.Format("<ChinaPay.Core>dynamictypes.simpledataobject.{0}", Interlocked.Increment(ref seed));
        }

        /// <summary>
        /// 根据指定的类型信息，动态创建一个类型。
        /// </summary>
        /// <param name="info">类型信息。</param>
        /// <param name="propertyReadonly">指定一个值，用于确定新创建的类型的属性是否只读。</param>
        /// <returns>返回新创建的类型。</returns>
        private static Type CreateDynamicTypeByTypeInfo(DynamicTypeInfo info, bool propertyReadonly = false)
        {
            var interfaces = info.Interfaces.Count > 0 ? new Type[info.Interfaces.Count] : Type.EmptyTypes;
            if (info.Interfaces.Count > 0)
            {
                info.Interfaces.CopyTo(interfaces);
            }
            var type = DynamicAssemblyHolder.Module.DefineType(GetDynamicTypeName(), TypeAttributes.Public, info.BaseType, interfaces);
            var fnCreateProperty = propertyReadonly ? new Action<TypeBuilder, PropInfo>(CreateReadonlyProperty) : CreateProperty;
            foreach (var prop in info.Properties)
            {
                fnCreateProperty(type, prop);
            }
            return type.CreateType();
        }

        /// <summary>
        /// 根据给定的属性信息，为类型创建一个属性。
        /// </summary>
        /// <param name="type">要创建属性的类型。</param>
        /// <param name="pi">要创建的属性。</param>
        private static void CreateProperty(TypeBuilder type, PropInfo pi)
        {
            var prop = type.DefineProperty(pi.Name, PropertyAttributes.HasDefault, pi.Type, null);
            var field = type.DefineField("m_" + pi.Name, pi.Type, FieldAttributes.Private);
            const MethodAttributes attr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // getter
            var getter = type.DefineMethod("get_" + pi.Name, attr, pi.Type, Type.EmptyTypes);
            var getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, field);
            getIL.Emit(OpCodes.Ret);
            prop.SetGetMethod(getter);

            // setter
            var setter = type.DefineMethod("set_" + pi.Name, attr, null, new[] { pi.Type });
            var setIL = setter.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, field);
            setIL.Emit(OpCodes.Ret);
            prop.SetSetMethod(setter);
        }

        /// <summary>
        /// 根据给定的属性信息，为类型创建一个只读属性（没有 set 访问器）。
        /// </summary>
        /// <param name="type">要创建属性的类型。</param>
        /// <param name="pi">要创建的属性。</param>
        private static void CreateReadonlyProperty(TypeBuilder type, PropInfo pi)
        {
            var prop = type.DefineProperty(pi.Name, PropertyAttributes.HasDefault, pi.Type, null);
            var field = type.DefineField("m_" + pi.Name, pi.Type, FieldAttributes.Private);
            const MethodAttributes attr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // getter
            var getter = type.DefineMethod("get_" + pi.Name, attr, pi.Type, Type.EmptyTypes);
            var getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, field);
            getIL.Emit(OpCodes.Ret);
            prop.SetGetMethod(getter);
        }

        #region 套嵌类型

        /// <summary>
        /// 定义一个类型，用于在内部维持一个动态程序集。
        /// </summary>
        private static class DynamicAssemblyHolder
        {
            private static readonly ModuleBuilder module;
            private static readonly AssemblyBuilder assembly;

            static DynamicAssemblyHolder()
            {
                var curDomain = AppDomain.CurrentDomain;

                var name = new AssemblyName
                {
                    Name = "ChinaPay.Core.Core.DynamicAssembly",
                    Version = new Version("1.0.0.0")
                };

                assembly = curDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

                module = assembly.GetDynamicModule("ChinaPay.Core.Core.DynamicAssembly.DynamicModule") ??
                         assembly.DefineDynamicModule("ChinaPay.Core.Core.DynamicAssembly.DynamicModule");
            }

            internal static ModuleBuilder Module
            {
                get { return module; }
            }
        }

        /// <summary>
        /// 动态类型信息。
        /// </summary>
        private sealed class DynamicTypeInfo
        {
            private readonly HashSet<PropInfo> props = new HashSet<PropInfo>(new PropInfoEqualComparer());
            private readonly HashSet<Type> interfaces = new HashSet<Type>();

            public HashSet<PropInfo> Properties { get { return props; } }
            public HashSet<Type> Interfaces
            {
                get { return interfaces; }
            }
            public Type BaseType { get; set; }
        }

        /// <summary>
        /// 属性信息。
        /// </summary>
        private sealed class PropInfo
        {
            public PropInfo(string name, Type type)
            {
                Name = name;
                Type = type;
            }
            public string Name { get; private set; }
            public Type Type { get; private set; }
        }

        /// <summary>
        /// 比较器，用于确定两个属性信息是否相等。
        /// </summary>
        private sealed class PropInfoEqualComparer : IEqualityComparer<PropInfo>
        {
            /// <summary>
            /// 判断指定的两个属性信息是否相等。
            /// </summary>
            /// <param name="x">属性信息。</param>
            /// <param name="y">属性信息。</param>
            /// <returns>如果两个属性信息具有相同的名称和属性，返回 true，否则返回 false。（都为 null 的两个属性信息被视为不相等。）</returns>
            public bool Equals(PropInfo x, PropInfo y)
            {
                return x != null && y != null && (x.Name == y.Name && x.Type == y.Type);
            }

            public int GetHashCode(PropInfo obj)
            {
                return obj.GetType().GetHashCode();
            }
        }

        /// <summary>
        /// 比较器，用于确定两个动态类型信息是否相等。
        /// </summary>
        private sealed class DynamicTypeInfoEqualConparer : IEqualityComparer<DynamicTypeInfo>
        {
            /// <summary>
            /// 判断两个类型信息是否相等。
            /// </summary>
            /// <param name="x">类型信息。</param>
            /// <param name="y">类型信息。</param>
            /// <returns>如果两个类型信息具有完全相同的属性（相同的属性数量，以及相同的属性名称和属性类型），返回 true；否则返回 false。</returns>
            public bool Equals(DynamicTypeInfo x, DynamicTypeInfo y)
            {
                return x != null && y != null && x.Properties.SetEquals(y.Properties) && x.BaseType == y.BaseType && x.Interfaces.SetEquals(y.Interfaces);
            }

            public int GetHashCode(DynamicTypeInfo obj)
            {
                return 0;
            }
        }
        #endregion
    }
}
