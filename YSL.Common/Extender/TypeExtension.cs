using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using YSL.Common.Utility;

namespace YSL.Common.Extender
{
    /// <summary>
    /// Type 扩展类
    /// </summary>
    public static class TypeExtension {
        public static string GetDescription(this Type type) {
            var descriptionAttribute = type.GetAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute == null ? string.Empty : descriptionAttribute.Description;
        }

        public static Type FindGenericType(Type definition, Type type) {
            while ((type != null) && (type != typeof(object))) {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition)) {
                    return type;
                }
                if (definition.IsInterface) {
                    foreach (var type2 in type.GetInterfaces()) {
                        var type3 = FindGenericType(definition, type2);
                        if (type3 != null) {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
        public static bool IsEnumerableType(this Type enumerableType) {
            return (FindGenericType(typeof(IEnumerable<>), enumerableType) != null);
        }
        public static Type GetElementType(this Type enumerableType) {
            var type = FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type != null) {
                return type.GetGenericArguments()[0];
            }
            return enumerableType;
        }
        /// <summary>
        /// 是否可空
        /// </summary>
        public static bool IsNullAssignable(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return !type.IsValueType || IsNullableType(type);
        }
        /// <summary>
        /// 是否是可空类型
        /// </summary>
        public static bool IsNullableType(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// 获取可空类型 的 实例类型
        /// </summary>
        public static Type GetNullAssignableType(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.GetGenericArguments()[0];
        }
        /// <summary>
        /// 获取与当前类型对应的原始类型
        /// 如果当前类型为引用类型，返回该类型本身，否则返回 Nullable&lt;T&gt; 的 T 类型
        /// </summary>
        public static Type GetNonNullableType(this Type type) {
            return IsNullableType(type) ? GetNullAssignableType(type) : type;
        }
        /// <summary>
        /// 获取成员类型
        /// </summary>
        public static Type GetMemberType(this MemberInfo mi) {
            if (mi == null)
                throw new ArgumentNullException("mi");

            switch (mi.MemberType) {
                case MemberTypes.Constructor:
                    return mi.DeclaringType;
                case MemberTypes.Method:
                    return ((MethodInfo)mi).ReturnType;
                case MemberTypes.Event:
                    return ((EventInfo)mi).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)mi).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)mi).PropertyType;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 是否是只读
        /// </summary>
        public static bool IsReadOnly(this MemberInfo mi) {
            if (mi == null)
                throw new ArgumentNullException("mi");

            switch (mi.MemberType) {
                case MemberTypes.Field:
                    return ((FieldInfo)mi).IsInitOnly;
                case MemberTypes.Property:
                    var pi = (PropertyInfo)mi;
                    return !pi.CanWrite || pi.GetSetMethod() != null;
                default:
                    return true;
            }
        }
        /// <summary>
        /// 获取当前类型的默认值
        /// </summary>
        public static object DefaultValue(this Type type) {
            return IsNullAssignable(type) ? null : Activator.CreateInstance(type);
        }
        /// <summary>
        /// 是否是整数类型
        /// </summary>
        public static bool IsInteger(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type)) {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 是否是数字类型
        /// </summary>
        public static bool IsNumeric(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type)) {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 是否是简单类型
        /// </summary>
        public static bool IsSimpleType(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsEnum || typeof(Guid) == type)
                return true;
            var typeCode = Type.GetTypeCode(type);
            return typeCode != TypeCode.Empty && typeCode != TypeCode.Object;
        }

        /// <summary>
        /// 创建当前类型的新实例。
        /// </summary>
        /// <param name="type">要创建实例的类型。</param>
        /// <param name="args">创建实例需要的参数。</param>
        /// <returns>返回创建的新实例。</returns>
        public static object CreateInstance(this Type type, params object[] args) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var proxy = TypeProxy.GetProxy(type);
            return proxy.CreateInstance(args);
        }

        /// <summary>
        /// 创建当前类型的新实例序列。
        /// </summary>
        /// <param name="type">要创建实例序列的类型。</param>
        /// <param name="count">要创建的实例序列中包含的元素的数量。</param>
        /// <param name="args">创建实例需要的参数。</param>
        /// <returns>返回创建的新实例序列。</returns>
        public static IEnumerable<object> CreateInstances(this Type type, int count, params object[] args) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var proxy = TypeProxy.GetProxy(type);
            return proxy.CreateInstances(count, args);
        }

        /// <summary>
        /// 获取当前类型上指定的静态属性或字段的值。
        /// </summary>
        /// <param name="type">要获取的属性或字段的值的类型。</param>
        /// <param name="name">属性或字段的名称。</param>
        /// <param name="ignoreCase">指定是否忽略属性或字段名称的大小写。</param>
        /// <returns>返回该属性或字段的值。</returns>
        public static object GetValue(this Type type, string name, bool ignoreCase = false) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var proxy = TypeProxy.GetProxy(type);
            return proxy.GetValue(null, name, ignoreCase);
        }

        /// <summary>
        /// 设置当前类型上指定的静态属性或字段的值。
        /// </summary>
        /// <param name="type">要设置的属性或字段的值的类型。</param>
        /// <param name="name">属性或字段的名称。</param>
        /// <param name="value">要设置的值。</param>
        /// <param name="ignoreCase">指定是否忽略属性或字段名称的大小写。</param>
        /// <returns>返回该属性或字段的值。</returns>
        public static void SetValue(this Type type, string name, object value, bool ignoreCase = false) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var proxy = TypeProxy.GetProxy(type);
            proxy.SetValue(null, name, value);
        }

        /// <summary>
        /// 调用当前类型上指定的静态方法。
        /// </summary>
        /// <param name="type">要调用方法的类型。</param>
        /// <param name="name">方法的名称。</param>
        /// <param name="ignoreCase">指定是否忽略方法名称的大小写。</param>
        /// <param name="args">调用方法时传入的参数。</param>
        /// <returns>返回被调用方法的返回值。</returns>
        public static object Invoke(this Type type, string name, bool ignoreCase, params object[] args) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            var proxy = TypeProxy.GetProxy(type);
            return proxy.Invoke(null, name, ignoreCase, args);
        }

        /// <summary>
        /// 判断当前类型是否是一个序列类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果当前对象实现 IEnumerable 接口，返回 true；否则返回 false。</returns>
        public static bool IsEnumerable(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return IsEnumerableType(type);
        }

        /// <summary>
        /// 判断当前类型是否是一个 T 类型元素的序列类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果当前对象实现 IEnumerable&lt;T&gt; 接口，返回 true；否则返回 false。</returns>
        public static bool IsEnumerable<T>(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return IsEnumerableType<T>(type);
        }

        /// <summary>
        /// 判断当前类型是否是一个 T 类型元素的数组类型。
        /// </summary>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果当前对象是一个 T 类型元素的数组类型，返回 true；否则返回 false。</returns>
        public static bool IsArray<T>(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsArray && type.GetElementType() == typeof(T);
        }

        /// <summary>
        /// 判断一个类型是否是实现 IEnumerable&lt;T&gt; 接口的类型。
        /// </summary>
        /// <typeparam name="T">序列中元素的类型。</typeparam>
        /// <param name="type">要判断的类型。</param>
        /// <returns>如果类型 type 的实例可用于 IEnumerable&lt;T&gt; 的实例，返回 true；否则返回 false。</returns>
        private static bool IsEnumerableType<T>(Type type) {
            return typeof(IEnumerable<T>).IsAssignableFrom(type);
        }


        public static Attribute[] GetAttributes(this Type type, Type attributeType) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }
            if (type.IsDefined(attributeType, true)) {
                return type.GetCustomAttributes(attributeType, true) as Attribute[];
            }
            return null;
        }
        public static Attribute GetAttribute(this Type type, Type attributeType) {
            Attribute[] attributes = GetAttributes(type, attributeType);
            if (attributes != null && attributes.Length > 0) {
                return attributes[0];
            }
            return null;
        }

        public static Attribute[] GetAttributes(this MemberInfo mem, Type attributeType) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }
            if (mem.IsDefined(attributeType, true)) {
                return mem.GetCustomAttributes(attributeType, true) as Attribute[];
            }
            return null;
        }
        public static Attribute GetAttribute(this MemberInfo mem, Type attributeType) {
            Attribute[] attributes = GetAttributes(mem, attributeType);
            if (attributes != null && attributes.Length > 0) {
                return attributes[0];
            }
            return null;
        }

        public static T[] GetAttributes<T>(this Type type) where T : Attribute {
            var attributes = type.GetCustomAttributes(true);
            return GetAttributes<T>(attributes);
        }
        public static T GetAttribute<T>(this Type type) where T : Attribute {
            var attributes = type.GetAttributes<T>();
            return attributes.FirstOrDefault();
        }
        //public static T[] GetAttributes<T>(this FieldInfo field) where T : Attribute {
        //    var attributes = field.GetCustomAttributes(true);
        //    return GetAttributes<T>(attributes);
        //}
        //public static T GetAttribute<T>(this FieldInfo field) where T : Attribute {
        //    var attributes = field.GetAttributes<T>();
        //    return attributes.FirstOrDefault();
        //}
        //public static T[] GetAttributes<T>(this PropertyInfo property) where T : Attribute {
        //    var attributes = property.GetCustomAttributes(true);
        //    return GetAttributes<T>(attributes);
        //}
        //public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute {
        //    var attributes = property.GetAttributes<T>();
        //    return attributes.FirstOrDefault();
        //}
        //public static T[] GetAttributes<T>(this MethodInfo method) where T : Attribute {
        //    var attributes = method.GetCustomAttributes(true);
        //    return GetAttributes<T>(attributes);
        //}
        //public static T GetAttribute<T>(this MethodInfo method) where T : Attribute {
        //    var attributes = method.GetAttributes<T>();
        //    return attributes.FirstOrDefault();
        //}

        public static T[] GetAttributes<T>(this MemberInfo member) where T : Attribute {
            var attributes = member.GetCustomAttributes(true);
            return GetAttributes<T>(attributes);
        }
        public static T GetAttribute<T>(this MemberInfo member) where T : Attribute {
            var attributes = member.GetAttributes<T>();
            return attributes.FirstOrDefault();
        }

        private static T[] GetAttributes<T>(IEnumerable<object> attributes) where T : Attribute {
            return (from attr in attributes
                    let tattr = attr as T
                    where tattr != null
                    select tattr).ToArray();
        }
    }
}