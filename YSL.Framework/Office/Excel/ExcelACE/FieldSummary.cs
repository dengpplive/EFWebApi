using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace ExcelACE {
    /// <summary>
    /// 对一个字段进行简要描述。
    /// </summary>
    public class FieldSummary {
        private const int DefaltVarCharLength = 8000;
        private const int DefaultNumberLength = 20;
        private const int DefaultNumberPrecision = 3;
        /// <summary>
        /// 初始化 FieldSummary 类型的新实例。
        /// </summary>
        public FieldSummary() { }

        /// <summary>
        /// 初始化 FieldSummary 类型的新实例。
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <param name="length">长度</param>
        /// <param name="precision">精度</param>
        /// <param name="nullable">是否可为空</param>
        public FieldSummary(string name, OleDbType type, int length = 0, int precision = 0, bool nullable = true) {
            if (string.IsNullOrWhiteSpace("name")) { throw new ArgumentException("指定的字段名称无效。", "name"); }
            Name = name;
            Type = type;
            Nullable = nullable;
            switch (Type) {
                case OleDbType.Char:
                case OleDbType.WChar:
                case OleDbType.VarChar:
                case OleDbType.VarWChar:
                    Length = length <= 0 ? DefaltVarCharLength : length;
                    break;
                case OleDbType.Decimal:
                case OleDbType.Numeric:
                    Length = length <= 0 ? DefaultNumberLength : length;
                    Precision = precision <= 0 ? DefaultNumberPrecision : precision;
                    break;
            }
        }
        /// <summary>
        /// 初始化 FieldSummary 类型的新实例。
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <param name="length">长度</param>
        /// <param name="precision">精度</param>
        /// <param name="nullable">是否可为空</param>
        public FieldSummary(string name, Type type, int length = 0, int precision = 0, bool nullable = true) : this(name, TranslateType(type), length, precision, nullable) { }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public OleDbType Type { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 精度
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 返回表示当前 <see cref="T:ExcelReader.FieldSummary"/> 的 <see cref="T:ExcelReader.FieldSummary"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:ExcelReader.FieldSummary"/>。
        /// </returns>
        public override string ToString() {
            string type;
            switch (Type) {
                case OleDbType.Char:
                case OleDbType.WChar:
                case OleDbType.VarChar:
                case OleDbType.VarWChar:
                    type = Length > 255 ? "text" : string.Format("{0}({1})", Type, Length);
                    break;
                case OleDbType.Decimal:
                case OleDbType.Numeric:
                    type = string.Format("{0}({1},{2})", Type, Length, Precision);
                    break;
                default:
                    type = Type.ToString();
                    break;
            }
            return string.Format("{0} {1} {2}", Name, type.ToLower(), Nullable ? string.Empty : "NOT NULL");
        }

        private static OleDbType TranslateType(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            if (type == typeof(bool)) { return OleDbType.Boolean; }
            if (type == typeof(sbyte)) { return OleDbType.TinyInt; }
            if (type == typeof(byte)) { return OleDbType.UnsignedTinyInt; }
            if (type == typeof(short)) { return OleDbType.SmallInt; }
            if (type == typeof(ushort)) { return OleDbType.UnsignedSmallInt; }
            if (type == typeof(int)) { return OleDbType.Integer; }
            if (type == typeof(uint)) { return OleDbType.UnsignedInt; }
            if (type == typeof(long)) { return OleDbType.BigInt; }
            if (type == typeof(ulong)) { return OleDbType.UnsignedBigInt; }
            if (type == typeof(float)) { return OleDbType.Single; }
            if (type == typeof(double)) { return OleDbType.Double; }
            if (type == typeof(decimal)) { return OleDbType.Numeric; }
            if (type == typeof(string)) { return OleDbType.VarChar; }
            if (type == typeof(Guid)) { return OleDbType.Guid; }
            if (type == typeof(DateTime)) { return OleDbType.Date; }
            if (type == typeof(TimeSpan)) { return OleDbType.DBTime; }
            if (type.IsClass) { return OleDbType.Variant; }
            if (type == typeof(byte[])) { return OleDbType.VarBinary; }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) { return TranslateType(type.GetGenericArguments()[0]); }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) { return OleDbType.VarWChar; }
            if (type.IsEnum) { return TranslateType(type.GetEnumUnderlyingType()); }

            throw new InvalidCastException(string.Format("无法将类型 \"{0}\" 转换为对应的 OleDbType", type.FullName));
        }
    }
}
