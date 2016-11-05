//  文件名：		Exceptions.cs
//  描述：		    提供在使用反射过程中引发的各种异常类型。
//  类型列表：
// ---------------------------------------------------------------------------------------------------
//      InvalidMemberNameException：表示指定的成员名称无效时引发的异常。
//      MemberNameIsNullOrEmptyException：表示成员名称为 null 或者全为空白字符时引发的异常。
//      MemberNotExistsException：表示当不存在指定名称的成员时引发的异常。
//      InvalidMemberTypeException：表示当成员类型无效时引发的异常。
//      MemberCannotReadException：表示当读取不可读成员时引发的异常。
//      MemberCannotWriteException：表示当写入不可写成员时引发的异常。
//      MethodNotExistsException：表示当调用的方法不存在或没有匹配的重载存在时引发的异常。
//      ConstructorNotExistsException：表示当调用没有匹配的构造函数时引发的异常	
// ---------------------------------------------------------------------------------------------------
//  作者：			ChinaPay.Core
//  创建时间：		2013/02/27
// ---------------------------------------------------------------------------------------------------
// 更新列表：

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 表示使用反射时引发的异常的代码。
    /// </summary>
    public abstract class ReflectionErrorCodes {
        /// <summary>
        /// 成员名称无效。
        /// </summary>
        public static readonly int InvalidMemberName = -11000;
        /// <summary>
        /// 成员名称为 null， 或空字符串，或全是空白字符。
        /// </summary>
        public static readonly int MemberNameIsNullOrEmpty = -11001;
        /// <summary>
        /// 成员不存在。
        /// </summary>
        public static readonly int MemberNotExists = -12000;
        /// <summary>
        /// 构造函数不存在。
        /// </summary>
        public static readonly int ConstructorNotExists = -12001;
        /// <summary>
        /// 方法不存在。
        /// </summary>
        public static readonly int MethodNotExists = -12002;
        /// <summary>
        /// 属性不存在。
        /// </summary>
        public static readonly int PropertyNotExists = -12003;
        /// <summary>
        /// 字段不存在。
        /// </summary>
        public static readonly int FieldNotExists = -12004;
        /// <summary>
        /// 事件不存在。
        /// </summary>
        public static readonly int EventNotExists = -12005;
        /// <summary>
        /// 指定重载的索引器不存在。
        /// </summary>
        public static readonly int IndexerNotExists = -12006;
        /// <summary>
        /// 成员不可读。
        /// </summary>
        public static readonly int MemberCannotRead = -13000;
        /// <summary>
        /// 属性不可读。
        /// </summary>
        public static readonly int PropertyCannotRead = -13001;
        /// <summary>
        /// 字段不可读。
        /// </summary>
        public static readonly int FieldCannotRead = -13002;
        /// <summary>
        /// 成员不可写。
        /// </summary>
        public static readonly int MemberCannotWrite = -14000;
        /// <summary>
        /// 属性不可写。
        /// </summary>
        public static readonly int PropertyCannotWrite = -14001;
        /// <summary>
        /// 字段不可写。
        /// </summary>
        public static readonly int FieldCannotWrite = -14002;
        /// <summary>
        /// 无效的成员类型。
        /// </summary>
        public static readonly int InvalidMemberType = -18000;
        /// <summary>
        /// 访问索引器时未指定索引。
        /// </summary>
        public static readonly int MissingIndexParameter = -20000;

    }
    /// <summary>
    /// 反射过程中引发的异常的基类。
    /// </summary>
    [Serializable]
    public abstract class ReflectionException : System.Exception {
        /// <summary>
        /// 初始化 ReflectionException 类型的新实例。
        /// </summary>
        protected ReflectionException() { }

        /// <summary>
        /// 用自定义的错误描述，初始化 ReflectionException 类型的新实例。
        /// </summary>
        /// <param name="message">自定义的错误描述。</param>
        protected ReflectionException(string message) : base(message) { }

        /// <summary>
        /// 用自定义的错误描述和内部异常，初始化 ReflectionException 类型的新实例。
        /// </summary>
        /// <param name="message">自定义的错误描述。</param>
        /// <param name="inner">内部异常。</param>
        protected ReflectionException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 ReflectionException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected ReflectionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public abstract int Code { get; }
    }

    /// <summary>
    /// 当指定的成员名称无效时引发的异常。
    /// </summary>
    [Serializable]
    public class InvalidMemberNameException : ReflectionException {
        /// <summary>
        /// 初始化 InvalidMemberNameException 类型的新实例。
        /// </summary>
        public InvalidMemberNameException() : base("指定的成员名称无效。") { }

        /// <summary>
        /// 用自定义的错误描述，初始化 InvalidMemberNameException 类型的新实例。
        /// </summary>
        /// <param name="message">错误描述。</param>
        public InvalidMemberNameException(string message) : base(message) { }

        /// <summary>
        /// 用指定的错误描述和内部异常，初始化 InvalidMemberNameException 类型的新实例。
        /// </summary>
        /// <param name="message">错误信息。</param>
        /// <param name="inner">内部异常。</param>
        public InvalidMemberNameException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 InvalidMemberNameException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected InvalidMemberNameException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.InvalidMemberName; }
        }
        #endregion
    }

    /// <summary>
    /// 当不存在指定名称的成员时引发的异常。
    /// </summary>
    [Serializable]
    public class MemberNotExistsException : ReflectionException {
        /// <summary>
        /// 获取引发异常的成员名称。
        /// </summary>
        private readonly string memberName;

        /// <summary>
        /// 初始化 MemberNotExistsException 类型的新实例。
        /// </summary>
        public MemberNotExistsException() : base("不存在指定名称的成员。") { }

        /// <summary>
        /// 用引发异常的成员名称，初始化 MemberNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        public MemberNotExistsException(string memberName) : base(string.Format("不存在名称为 \"{0}\" 的成员。", memberName)) { this.memberName = memberName; }

        /// <summary>
        /// 用引发异常的成员名称和内部异常信息，初始化 MemberNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        /// <param name="inner">内部异常信息。</param>
        public MemberNotExistsException(string memberName, System.Exception inner) : base(string.Format("不存在名称为 \"{0}\" 的成员。", memberName), inner) { this.memberName = memberName; }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 MemberNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected MemberNotExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// 获取引发异常的成员名称。
        /// </summary>
        public string MemberName {
            get { return memberName; }
        }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.MemberNotExists; }
        }
        #endregion
    }

    /// <summary>
    /// 当成员名称为 null 或者全为空白字符时引发的异常。
    /// </summary>
    [Serializable]
    public class MemberNameIsNullOrEmptyException : ReflectionException {
        /// <summary>
        /// 初始化 MemberNameIsNullOrEmptyException 类型的新实例。
        /// </summary>
        public MemberNameIsNullOrEmptyException() : base("成员名称为 null，空字符串，或者全为空白字符。") { }

        /// <summary>
        /// 用自定义的异常描述，初始化 MemberNameIsNullOrEmptyException 类型的新实例。
        /// </summary>
        /// <param name="message">自定义的异常描述。</param>
        public MemberNameIsNullOrEmptyException(string message) : base(message) { }

        /// <summary>
        /// 用自定义的异常描述和内部异常信息，初始化 MemberNameIsNullOrEmptyException 类型的新实例。
        /// </summary>
        /// <param name="message">自定义的异常描述。</param>
        /// <param name="inner">内部异常信息。</param>
        public MemberNameIsNullOrEmptyException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 MemberNameIsNullOrEmptyException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected MemberNameIsNullOrEmptyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.MemberNameIsNullOrEmpty; }
        }
        #endregion
    }

    /// <summary>
    /// 当成员类型不匹配时引发的异常。
    /// </summary>
    [Serializable]
    public class InvalidMemberTypeException : ReflectionException {
        /// <summary>
        /// 获取引发异常的成员类型。
        /// </summary>
        private readonly MemberTypes memberType;

        /// <summary>
        /// 初始化 InvalidMemberTypeException 类型的新实例。
        /// </summary>
        public InvalidMemberTypeException() : base("指定的成员类型在当前操作中无效。") { }

        /// <summary>
        /// 用引发异常的成员类型，初始化 InvalidMemberTypeException 类型的新实例。
        /// </summary>
        /// <param name="memberType">引发异常的成员类型。</param>
        public InvalidMemberTypeException(MemberTypes memberType) : base(string.Format("成员类型 \"{0}\" 在当前操作中无效。", memberType)) { this.memberType = memberType; }

        /// <summary>
        /// 用引发异常的成员类型和内部异常信息，初始化 InvalidMemberTypeException 类型的新实例。
        /// </summary>
        /// <param name="memberType">引发异常的成员类型。</param>
        /// <param name="inner">内部异常信息。</param>
        public InvalidMemberTypeException(MemberTypes memberType, System.Exception inner) : base(string.Format("成员类型 \"{0}\" 在当前操作中无效。", memberType), inner) { this.memberType = memberType; }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 InvalidMemberTypeException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected InvalidMemberTypeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// 获取引发异常的成员类型。
        /// </summary>
        public MemberTypes MemberType {
            get { return memberType; }
        }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.InvalidMemberType; }
        }
        #endregion
    }

    /// <summary>
    /// 表示当读取不可读成员时引发的异常。
    /// </summary>
    [Serializable]
    public class MemberCannotReadException : ReflectionException {
        private readonly string memberName;

        /// <summary>
        /// 初始化 MemberCannotReadException 类型的新实例。
        /// </summary>
        public MemberCannotReadException() : base("指定的成员不可读。") { }

        /// <summary>
        /// 用引发异常的成员名称，初始化 MemberCannotReadException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        public MemberCannotReadException(string memberName) : base(string.Format("成员 \"{0}\" 不可读。", memberName)) { this.memberName = memberName; }

        /// <summary>
        /// 用引发异常的成员名称和内部异常信息，初始化 MemberCannotReadException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        /// <param name="inner">内部异常信息。</param>
        public MemberCannotReadException(string memberName, System.Exception inner) : base(string.Format("成员 \"{0}\" 不可读。", memberName), inner) { this.memberName = memberName; }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 MemberCannotReadException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected MemberCannotReadException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// 获取引发异常的成员的名称。
        /// </summary>
        public string MemberName {
            get { return memberName; }
        }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.MemberCannotRead; }
        }
        #endregion
    }

    /// <summary>
    /// 表示当写入不可写成员时引发的异常。
    /// </summary>
    [Serializable]
    public class MemberCannotWriteException : ReflectionException {
        private readonly string memberName;

        /// <summary>
        /// 初始化 MemberCannotWriteException 类型的新实例。
        /// </summary>
        public MemberCannotWriteException() : base("指定的成员不可写。") { }

        /// <summary>
        /// 用引发异常的成员名称，初始化 MemberCannotWriteException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        public MemberCannotWriteException(string memberName) : base(string.Format("成员 \"{0}\" 不可写。", memberName)) { this.memberName = memberName; }

        /// <summary>
        /// 用引发异常的成员名称和内部异常信息，初始化 MemberCannotWriteException 类型的新实例。
        /// </summary>
        /// <param name="memberName">引发异常的成员名称。</param>
        /// <param name="inner">内部异常信息。</param>
        public MemberCannotWriteException(string memberName, System.Exception inner) : base(string.Format("成员 \"{0}\" 不可写。", memberName), inner) { this.memberName = memberName; }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 MemberCannotWriteException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected MemberCannotWriteException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// 获取引发异常的成员的名称。
        /// </summary>
        public string MemberName {
            get { return memberName; }
        }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.MemberCannotWrite; }
        }
        #endregion
    }

    /// <summary>
    /// 表示当调用的方法不存在或没有匹配的重载存在时引发的异常。
    /// </summary>
    [Serializable]
    public class MethodNotExistsException : ReflectionException {
        /// <summary>
        /// 初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        public MethodNotExistsException() : base("不存在指定的方法，或没有与传入参数类型相匹配的重载。") { }

        /// <summary>
        /// 使用引发异常的方法名称，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="methodName">引发异常的方法的名称。</param>
        public MethodNotExistsException(string methodName) : base(string.Format("不存在方法 \"{0}\"，或没有与传入参数类型相匹配的重载。", methodName)) { }

        /// <summary>
        /// 使用引发异常的方法名称和参数类型数组，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="methodName">引发异常的方法的名称。</param>
        /// <param name="argTypes">参数类型数组。</param>
        public MethodNotExistsException(string methodName, Type[] argTypes) : base(string.Format("不存在方法 \"{0}\"，或没有与传入参数类型 ({1}) 相匹配的重载。", methodName, MessageBuilder.GetTypeMessage(argTypes))) { }

        /// <summary>
        /// 使用引发异常的方法名称和参数数组，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="methodName">引发异常的方法的名称。</param>
        /// <param name="args">参数数组。</param>
        public MethodNotExistsException(string methodName, object[] args) : base(string.Format("不存在方法 \"{0}\"，或没有与传入参数类型 ({1}) 相匹配的重载。", methodName, MessageBuilder.GetArgumentsTypeMessage(args))) { }

        /// <summary>
        /// 使用引发异常的方法名称、参数类型数组和内部异常信息，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="methodName">引发异常的方法的名称。</param>
        /// <param name="argTypes">参数类型数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public MethodNotExistsException(string methodName, Type[] argTypes, System.Exception inner) : base(string.Format("不存在方法 \"{0}\"，或没有与传入参数类型 ({1}) 相匹配的重载。", methodName, MessageBuilder.GetTypeMessage(argTypes)), inner) { }

        /// <summary>
        /// 使用引发异常的方法名称和参数数组，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="methodName">引发异常的方法的名称。</param>
        /// <param name="args">参数数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public MethodNotExistsException(string methodName, object[] args, System.Exception inner) : base(string.Format("不存在方法 \"{0}\"，或没有与传入参数类型 ({1}) 相匹配的重载。", methodName, MessageBuilder.GetArgumentsTypeMessage(args)), inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 MethodNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected MethodNotExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.MethodNotExists; }
        }
        #endregion
    }

    /// <summary>
    /// 表示当调用的构造函数为非公开的，或者没有与参数类型相匹配的重载时引发的异常。
    /// </summary>
    [Serializable]
    public class ConstructorNotExistsException : ReflectionException {
        /// <summary>
        /// 初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        public ConstructorNotExistsException() { }

        /// <summary>
        /// 使用参数类型数组，初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="argTypes">参数类型数组。</param>
        public ConstructorNotExistsException(Type[] argTypes) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的构造函数重载，或该构造函数未公开。", MessageBuilder.GetTypeMessage(argTypes))) { }

        /// <summary>
        /// 使用参数数组，初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="args">参数数组。</param>
        public ConstructorNotExistsException(object[] args) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的构造函数重载，或该构造函数未公开。", MessageBuilder.GetArgumentsTypeMessage(args))) { }

        /// <summary>
        /// 使用参数数组和内部异常信息，初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="argTypes">参数类型数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public ConstructorNotExistsException(Type[] argTypes, System.Exception inner) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的构造函数重载，或该构造函数未公开。", MessageBuilder.GetTypeMessage(argTypes)), inner) { }

        /// <summary>
        /// 使用参数数组和内部异常信息，初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="args">参数类型数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public ConstructorNotExistsException(object[] args, System.Exception inner) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的构造函数重载，或该构造函数未公开。", MessageBuilder.GetArgumentsTypeMessage(args)), inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 ConstructorNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected ConstructorNotExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.ConstructorNotExists; }
        }
        #endregion
    }

    /// <summary>
    /// 表示当访问的索引器为非公开的，或者没有与参数类型相匹配的重载时引发的异常。
    /// </summary>
    public class IndexerNotExistsException : ReflectionException {
        /// <summary>
        /// 初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        public IndexerNotExistsException() { }

        /// <summary>
        /// 使用参数类型数组，初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="argTypes">参数类型数组。</param>
        public IndexerNotExistsException(Type[] argTypes) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的索引器重载，或该索引器未公开。", MessageBuilder.GetTypeMessage(argTypes))) { }

        /// <summary>
        /// 使用参数数组，初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="args">参数数组。</param>
        public IndexerNotExistsException(object[] args) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的索引器重载，或该索引器未公开。", MessageBuilder.GetArgumentsTypeMessage(args))) { }

        /// <summary>
        /// 使用参数数组和内部异常信息，初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="argTypes">参数类型数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public IndexerNotExistsException(Type[] argTypes, System.Exception inner) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的索引器重载，或该索引器未公开。", MessageBuilder.GetTypeMessage(argTypes)), inner) { }

        /// <summary>
        /// 使用参数数组和内部异常信息，初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="args">参数类型数组。</param>
        /// <param name="inner">内部异常信息。</param>
        public IndexerNotExistsException(object[] args, System.Exception inner) : base(string.Format("没有与传入参数类型 ({0}) 相匹配的索引器重载，或该索引器未公开。", MessageBuilder.GetArgumentsTypeMessage(args)), inner) { }

        /// <summary>
        /// 用自定义的序列化信息和流上下文，初始化 IndexerNotExistsException 类型的新实例。
        /// </summary>
        /// <param name="info">序列化信息。</param>
        /// <param name="context">上下文。</param>
        protected IndexerNotExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        #region Overrides of ReflectionException
        /// <summary>
        /// 获取表示当前异常的编码。
        /// </summary>
        public override int Code {
            get { return ReflectionErrorCodes.IndexerNotExists; }
        }
        #endregion
    }

    /// <summary>
    /// 在尝试访问索引器时未指定索引引发的异常。
    /// </summary>
    [Serializable]
    public class MissingIndexParameterException : ReflectionException {
        /// <summary>
        /// 初始化 MissingIndexParameterException 类型的新实例。
        /// </summary>
        public MissingIndexParameterException() : base("未指定索引。") { }
        /// <summary>
        /// 用指定的描述信息，初始化 MissingIndexParameterException 类型的新实例。
        /// </summary>
        /// <param name="message">描述信息。</param>
        public MissingIndexParameterException(string message) : base(message) { }
        /// <summary>
        /// 用指定的描述信息和内部异常，初始化 MissingIndexParameterException 类型的新实例。
        /// </summary>
        /// <param name="message">描述信息。</param>
        /// <param name="inner">内部异常。</param>
        public MissingIndexParameterException(string message, System.Exception inner) : base(message, inner) { }
        public override int Code {
            get { return ReflectionErrorCodes.MissingIndexParameter; }
        }
        protected MissingIndexParameterException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// 用于生成异常信息的工具类。
    /// </summary>
    internal static class MessageBuilder {
        /// <summary>
        /// 根据提供的类型数组，拼接由类型全称组成，并由逗号分隔的字符串。
        /// </summary>
        /// <param name="types">用于拼接字符串的类型数组。</param>
        /// <returns>返回拼接的字符串。</returns>
        public static string GetTypeMessage(Type[] types) {
            var buffer = new StringBuilder();
            var sepCount = types.Length - 1;
            for (var i = 0; i < types.Length; i++) {
                var type = types[0];
                buffer.Append(type.FullName);
                if (i < sepCount) {
                    buffer.Append(',');
                }
            }
            return buffer.ToString();
        }

        /// <summary>
        /// 根据提供的参数数组，拼接由各参数类型全称组成，并由逗号分隔的字符串。
        /// </summary>
        /// <param name="args">用于拼接字符串的参数数组。</param>
        /// <returns>返回拼接的字符串。</returns>
        public static string GetArgumentsTypeMessage(object[] args) {
            var buffer = new StringBuilder();
            var sepCount = args.Length - 1;
            for (var i = 0; i < args.Length; i++) {
                var argType = args[i].GetType();
                buffer.Append(argType.FullName);
                if (i < sepCount) {
                    buffer.Append(',');
                }
            }
            return buffer.ToString();
        }
    }
}