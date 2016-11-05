using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YSL.Common.Exceptions;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 类型处理工具类
    /// </summary>
    public static partial class TypeHelper {
        private static readonly MethodInfo convert = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });

        /// <summary>
        /// 根据通过反射得到的 ConstructorInfo 创建一个调用该构造函数的匿名方法。
        /// </summary>
        /// <param name="ctor">通过反射获取到的 ConstructorInfo。</param>
        /// <returns>返回一个调用指定构造函数的匿名方法。</returns>
        /// <remarks>要调用的构造函数必须是公开的。</remarks>
        internal static Func<object[], object> CreateConstructorInvoker(ConstructorInfo ctor) {
            /* 要调用一个构造函数，通常使用的方式为：new TypeName(arg1,arg2,...)
             * 因此我们只需要创建一个 Lambda 表达式 args => new TypeName(arg[0],arg[1],...)，然后将该表达式编译为一个匿名方法，
             * 就可以调用该匿名方法实现对由 ctor 所表示的构造函数的调用。
             * --------------------------------------------------------------------------------------------------------------------------------
             * 最终我们得到的匿名方法签名：.ctor(object[] args)
             * 参数 args 表示调用构造函数时要使用的参数。
             */

            if(ctor == null) {
                throw new ArgumentNullException("ctor");
            }

            // args
            var args = Expression.Parameter(typeof(object[]), "args");

            // new TypeName(args[0],args[1],...)
            var parameters = ctor.GetParameters().Select((pi, i) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i)), pi.ParameterType));
            var call = Expression.New(ctor, parameters);

            // (args) => new TypeName()
            var exp = Expression.Lambda<Func<object[], object>>(call, args);

            return exp.Compile();
        }

        /// <summary>
        /// 根据通过反射得到的 MethodInfo 创建一个调用该方法的匿名方法。
        /// </summary>
        /// <param name="method">通过反射获取到的 MethodInfo</param>
        /// <returns>返回一个调用指定方法的匿名方法。</returns>
        /// <remarks>要调用的方法必须是公开的。</remarks>
        internal static Func<object, object[], object> CreateMethodInvoker(MethodInfo method) {
            /* 要调用一个方法，通常使用的方式为：instance.MethodName(arg1,arg2,...)
             * 因此我们只需要创建一个 Lambda 表达式 (instance,args)=>instance.MethodName(arg[0],arg[1],...)，然后将该表达式编译为一个匿名方法，
             * 就可以调用该匿名方法实现对由 method 所表示的方法的调用。
             * --------------------------------------------------------------------------------------------------------------------------------
             * 方法里有一种比较特殊的类型，返回值类型为 void 的方法。
             * 在处理这种情况时，我们创建一个 Action<T,...> 而不是 Func<T,...>。
             * 原因是 Func<T,...>的最后一个类型参数是用来表示返回值类型的，而我们无法在 void 和 object 之间做任何的转换操作。
             * 所以我们创建一个 Action<T,...> 用 action(instance,args) 的方式来调用 method 表示的方法，
             * 在外面用 Lambda 表达式：(instance,args) => { action(instance,args); return null; } 的形式包装，达到与有返回值的方法调用相同的方式。
             * --------------------------------------------------------------------------------------------------------------------------------
             * 最终我们得到的匿名方法签名：(object instance,object[] args)
             * 参数 instance 表示要在其上调用方法的实例；
             * 参数 args 表示调用方法时要使用的参数。
             */

            if(method == null) {
                throw new ArgumentNullException("method");
            }

            // instance
            var instance = Expression.Parameter(typeof(object), "instance");
            // args
            var args = Expression.Parameter(typeof(object[]), "args");

            // instance.methodName(args[0],args[1],...)
            var parameters = method.GetParameters()
                .Select((pi, i) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i)),
                pi.ParameterType.IsGenericTypeDefinition
                ? pi.ParameterType.MakeGenericType(pi.ParameterType.GetGenericArguments())
                : pi.ParameterType) as Expression).ToArray();

            var call = method.IsStatic
                ? Expression.Call(null, method, parameters)
                : Expression.Call(Expression.Convert(instance, method.ReflectedType), method, parameters);

            // (instance,args)=>instance.methodName(args[0],arg[1],...)
            Func<object, object[], object> invoker;
            if(call.Type == typeof(void)) {
                var action = Expression.Lambda<Action<object, object[]>>(call, instance, args).Compile();
                invoker = (inst, arguments) => {
                    action(inst, arguments);
                    return null;
                };
            }
            else {
                invoker = Expression.Lambda<Func<object, object[], object>>(Expression.Convert(call, typeof(object)), instance, args).Compile();
            }
            return invoker;
        }

        /// <summary>
        /// 根据通过反射获取到的 PropertyInfo 创建一个获取该成员的值的委托。
        /// </summary>
        /// <param name="property">通过反射获取到的 PropertyInfo </param>
        /// <returns>返回一个获取指定属性的值的委托。</returns>
        internal static Func<object, object[], object> CreatePropertyGetter(PropertyInfo property) {
            /* 要获取一个对象的属性的值，通常使用的方式为：instance.MemberName
             * （对于获取属性的值，另一种方式是通过反射获取到该属性的 get 访问器，调用 get 访问器可以取到该属性的值。）
             * 因此我们只需要创建一个 Lambda 表达式 instance=>instance.MemberName，然后将该表达式编译为一个委托，
             * 就可以调用该方法实现对该成员的取值操作。
             * --------------------------------------------------------------------------------------------------------------------------------
             * update：2013/2/25，ChinaPay.Core
             * 属性中有一类特殊的属性，索引器：Indexer。
             * 针对索引器，我们要创建的 Lambda 表达式为：instance,indexes=>instance.Item[indexes[0],...]
             * --------------------------------------------------------------------------------------------------------------------------------
             * 最终我们得到的委托签名：(object instance,object[] indexes)
             * 参数 instance 表示要获取属性的实例；
             * 参数 indexes 表示用于访问索引器时用到的索引。
             */

            if(property == null) {
                throw new ArgumentNullException("property");
            }

            if(!property.CanRead) {
                throw new MemberCannotReadException(property.Name);
            }

            // instance.memberName, or instance.Item[indexes[0],...]

            // instance
            var instance = Expression.Parameter(typeof(object), "instance");
            // indexes
            var indexes = Expression.Parameter(typeof(object[]), "indexes");

            var indexParams = property.GetIndexParameters();

            Expression prop;
            // instance.memberName
            if(indexParams.Length == 0) {
                prop = Expression.Property(Expression.Convert(instance, property.ReflectedType), property.Name);
            }
            // instance.memberName[index]
            else {
                var parameters = indexParams.Select((pi, i) => Expression.Convert(Expression.ArrayIndex(indexes, Expression.Constant(i)), pi.ParameterType));
                prop = Expression.MakeIndex(Expression.Convert(instance, property.ReflectedType), property, parameters);
            }

            // instance=>instance.memberName or instance=>instance.memberName[index]
            var exp = Expression.Lambda<Func<object, object[], object>>(Expression.Convert(prop, typeof(object)), instance, indexes);
            return exp.Compile();
        }

        /// <summary>
        /// 根据通过反射获取到的 MemberInfo 创建一个设置该成员的值的委托。
        /// </summary>
        /// <param name="property">通过反射获取到的 PropertyInfo </param>
        /// <returns>返回一个设置指定成员的值的委托。</returns>
        internal static Action<object, object, object[]> CreatePropertySetter(PropertyInfo property) {
            /* 要设置一个对象的属性的值，通常使用的方式为：instance.MemberName = value
             * （对于设置属性的值，另一种方式是获取到该属性的 set 访问器，调用 set 访问器可以设置该属性的值。）
             * 因此我们只需要创建一个 Lambda 表达式 (instance,value)=>instance.MemberName = value，然后将该表达式编译为一个委托，
             * 就可以调用该方法实现设置该成员的值操作。
             * --------------------------------------------------------------------------------------------------------------------------------
             * update：2013/2/25，ChinaPay.Core
             * 属性中有一类特殊的属性，索引器：Indexer。
             * 针对索引器，我们要创建的 Lambda 表达式为：(instance,value,indexes)=>instance.Item[indexes[0],...] = value
             * --------------------------------------------------------------------------------------------------------------------------------
             * --------------------------------------------------------------------------------------------------------------------------------
             * update：2013/11/4，ChinaPay.Core
             * 针对枚举类型的属性，需要将值的类型转换为对应枚举的 UnderlyingType 才能正确用强制转换将值转换为枚举。
             * --------------------------------------------------------------------------------------------------------------------------------
             * 最终我们得到的委托签名：(object instance,object value,object[] indexes)
             * 参数 instance 表示要设置属性值的实例；
             * 参数 value 表示要设置的属性的值；
             * 参数 indexes 表示用于访问索引器时用到的索引。
             */

            if(property == null) {
                throw new ArgumentNullException("property");
            }

            if(!property.CanWrite) {
                throw new MemberCannotWriteException(property.Name);
            }

            // (instance,value)=>instance.memberName = value,or (instance,value,indexes)=>instance.memberName[indexes[0],...] = value
            // instance
            var instance = Expression.Parameter(typeof(object), "instance");
            // value
            var value = Expression.Parameter(typeof(object), "value");
            // indexes
            var indexes = Expression.Parameter(typeof(object[]), "indexes");

            var indexParams = property.GetIndexParameters();

            Expression prop;
            // instance.memberName
            if(indexParams.Length == 0) {
                prop = Expression.Property(Expression.Convert(instance, property.ReflectedType), property.Name);
            }
            // instance.memberName[index,...]
            else {
                var parameters = indexParams.Select((pi, i) => Expression.Convert(Expression.ArrayIndex(indexes, Expression.Constant(i)), pi.ParameterType));
                prop = Expression.MakeIndex(Expression.Convert(instance, property.ReflectedType), property, parameters);
            }

            // instance.memberName = value, or instance.memberName[index1,...] = value

            // 2013-11-4，ChinaPay.Core，增加对枚举类型的处理（值的类型与枚举的 UnderlyingType 不一致的情况）。
            //var assign = Expression.Assign(prop, Expression.Convert(value, property.PropertyType));
            // 思路1、通过两次强制转换，先将 value 转换为枚举的 UnderlyingType，然后再转换为枚举类型。
            //var assign = Expression.Assign(prop, Expression.Convert(
            //    property.PropertyType.IsEnum ? Expression.Convert(value,property.PropertyType.GetEnumUnderlyingType()) as Expression : value, 
            //    property.PropertyType));
            // 以上方法不可行，原因是：value 中的值类型与要强制转换的类型必须一致，例如，value 为一个 byte 类型数据的装箱，那么就不能作为一个 int 来取消装箱（拆箱）。
            // 思路2、先按原始类型拆箱，再强制转换为枚举的 UnderlyingType，最后转换为枚举类型。
            // 放弃了，原因：如何在表达式中确定 value 的原始类型？
            // 思路3、首先通过调用一次 Convert.ChangeType 将 value 转换成 UnderlyingType，再通过强制转换得到对应的枚举类型的值。
            var assign = Expression.Assign(prop,
                Expression.Convert(property.PropertyType.IsEnum
                ? Convert(value, property.PropertyType.GetEnumUnderlyingType()) as Expression
                : value, property.PropertyType));

            // (arg,value)=>arg.memberName = value, or (instance,value,indexes)=>instance.memberName[indexes[0],...] = value
            var exp = Expression.Lambda<Action<object, object, object[]>>(assign, instance, value, indexes);

            return exp.Compile();
        }

        ///// <summary>
        ///// 创建一个索引器取值的委托。
        ///// </summary>
        ///// <param name="argumentTypes">索引参数类型。</param>
        ///// <returns>返回一个委托，用于访问索引器，通过指定的索引取值。</returns>
        //internal static Func<object, object[], object> CreateIndexerGetter(params Type[] argumentTypes) {
        //    // 对于访问索引器，通常我们会在代码里面这样写：obj[index1,index2,...];
        //    // 而实际上，索引器在内部实现为一个名为 Item 的特殊属性。所以，实际上外部访问 obj[index1,index2,...] 等效于内部 obj.item[index1,index2,...]
        //    // 因此我们借助 GetPropertySetter 创建一个针对 Item 属性的取值委托。
        //}

        ///// <summary>
        ///// 创建一个索引器赋值的委托。
        ///// </summary>
        ///// <param name="argumentTypes">索引参数类型。</param>
        ///// <returns>返回一个委托，用于访问索引器，通过指定的索引赋值。</returns>
        //internal static Action<object, object, object[]> CreateIndexerSetter(params Type[] argumentTypes) {

            
        //}

        /// <summary>
        /// 根据通过反射获取到的 MemberInfo 创建一个获取该成员的值的委托。
        /// </summary>
        /// <param name="field">通过反射获取到的 FieldInfo </param>
        /// <returns>返回一个获取指定字段的值的委托。</returns>
        internal static Func<object, object> CreateFieldGetter(FieldInfo field) {
            /* 要获取一个对象的字段的值，通常使用的方式为：instance.MemberName
             * 因此我们只需要创建一个 Lambda 表达式 instance=>instance.MemberName，然后将该表达式编译为一个委托，就可以调用该委托实现对该成员的取值操作。
             * 参数 instance 表示要获取属性或字段值的实例；
             */

            if(field == null) {
                throw new ArgumentNullException("field");
            }

            // instance.memberName

            // instance
            var instance = Expression.Parameter(typeof(object), "instance");

            // instance.memberName
            var exp = Expression.Field(Expression.Convert(instance, field.ReflectedType), field.Name);

            // instance=>instance.memberName or instance=>instance.memberName[index]
            return Expression.Lambda<Func<object, object>>(Expression.Convert(exp, typeof(object)), instance).Compile();
        }

        /// <summary>
        /// 根据通过反射获取到的 FieldInfo 创建一个设置该字段的值的委托。
        /// </summary>
        /// <param name="field">通过反射获取到的 FieldInfo </param>
        /// <returns>返回一个设置指定成员的值的委托。</returns>
        internal static Action<object, object> CreateFieldSetter(FieldInfo field) {
            /* 要设置一个对象的字段的值，通常使用的方式为：instance.MemberName = value
             * 因此我们只需要创建一个 Lambda 表达式 (instance,value)=>instance.MemberName = value，然后将该表达式编译为一个委托，就可以调用该委托实现设置该成员的值操作。
             * 最终我们得到的委托签名：(object instance,object value)
             * 参数 instance 表示要设置字段值的实例；
             * 参数 value 表示要设置的属性或字段的值；
             */

            if(field == null) {
                throw new ArgumentNullException("field");
            }

            // (instance,value)=>instance.memberName = value,or (instance,value,indexes)=>instance.memberName[indexes[0],...] = value
            // instance
            var instance = Expression.Parameter(typeof(object), "instance");
            // value
            var value = Expression.Parameter(typeof(object), "value");

            // instance.memberName = value, or instance.memberName[index1,...] = value
            // 2013-11-4，ChinaPay.Core，增加对枚举类型的处理（值的类型与枚举的 UnderlyingType 不一致的情况）。
            //var assign = Expression.Assign(Expression.Field(Expression.Convert(instance, field.ReflectedType), field.Name), Expression.Convert(value, field.FieldType));
            var assign = Expression.Assign(Expression.Field(Expression.Convert(instance, field.ReflectedType), field.Name),
                Expression.Convert(field.FieldType.IsEnum ? Convert(value, field.FieldType.GetEnumUnderlyingType()) as Expression : value, field.FieldType));
            // (arg,value)=>arg.memberName = value, or (instance,value,indexes)=>instance.memberName[indexes[0],...] = value
            var exp = Expression.Lambda<Action<object, object>>(assign, instance, value);

            return exp.Compile();
        }

        /// <summary>
        /// 根据提供的 MemberInfo 获取为该成员取值的委托。
        /// </summary>
        /// <param name="member">通过反射获取到的成员信息。</param>
        /// <returns>获取到的可为该成员取值的 Func&lt;object,object&gt; 类型的委托。</returns>
        internal static Func<object, object> GetPropertyOrFieldGetter(MemberInfo member) {
            if(!(member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)) {
                throw new InvalidMemberTypeException(member.MemberType);
            }
            Func<object, object> getter;
            if(member.MemberType == MemberTypes.Field) {
                getter = CreateFieldGetter(member as FieldInfo);
            }
            else {
                if(member.Name != "Item") {
                    var propertyGetter = CreatePropertyGetter(member as PropertyInfo);
                    getter = instance => propertyGetter(instance, null);
                }
                else {
                    throw new InvalidMemberTypeException();
                }
            }
            return getter;
        }

        /// <summary>
        /// 根据提供的 MemberInfo 获取为该成员赋值的委托。
        /// </summary>
        /// <param name="member">通过反射获取到的成员信息。</param>
        /// <returns>获取到的可为该成员赋值的 Action&lt;object,object&gt; 类型的委托。</returns>
        internal static Action<object, object> GetPropertyOrFieldSetter(MemberInfo member) {
            if(!(member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)) {
                throw new InvalidMemberTypeException(member.MemberType);
            }
            Action<object, object> setter;
            if(member.MemberType == MemberTypes.Field) {
                setter = CreateFieldSetter(member as FieldInfo);
            }
            else {
                if(member.Name != "Item") {
                    var propertySetter = CreatePropertySetter(member as PropertyInfo);
                    setter = (instance, value) => propertySetter(instance, value, null);
                }
                else {
                    throw new InvalidMemberTypeException();
                }
            }
            return setter;
        }

        /// <summary>
        /// 创建一个对 System.Convert.ChangeType 的调用表达式。
        /// </summary>
        /// <param name="value">要转换类型的值。</param>
        /// <param name="type">转换的目标类型。</param>
        /// <returns>返回一个 MethodCallExpression 表示对一次 System.Convert.ChangeType 的调用。</returns>
        private static MethodCallExpression Convert(Expression value, Type type) {
            return Expression.Call(convert, value, Expression.Constant(type));
        }

        /// <summary>
        /// 判断当前属性是否是静态属性。
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsStatic(this PropertyInfo prop) {
            if (prop == null) {
                throw new ArgumentNullException("prop");
            }
            if (prop.CanRead) {
                return prop.GetGetMethod().IsStatic;
            }
            if (prop.CanWrite) {
                return prop.GetSetMethod().IsStatic;
            }
            throw new InvalidOperationException("属性必须可读或者可写。");
        }
    }
}
