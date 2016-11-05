using System;
using System.Data;

namespace YSL.Common.Extender
{
    /// <summary>
    /// IDataReader 扩展
    /// </summary>
    public static class DataReaderExtension
    {
        public static DateTime? GetNullableDateTime(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetDateTime(index);
        }
        public static Guid? GetNullableGuid(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetGuid(index);
        }
        public static char? GetNullableChar(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetChar(index);
        }
        public static bool? GetNullableBoolean(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetBoolean(index);
        }
        public static T? GetNullableEnum<T>(this IDataReader reader, int index) where T : struct
        {
            if (reader.IsDBNull(index))
                return null;
            return (T)Enum.Parse(typeof(T), reader[index].ToString());
        }
        public static byte? GetNullableByte(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetByte(index);
        }
        public static short? GetNullableInt16(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetInt16(index);
        }
        public static int? GetNullableInt32(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetInt32(index);
        }
        public static long? GetNullableInt64(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetInt64(index);
        }
        public static float? GetNullableFloat(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetFloat(index);
        }
        public static double? GetNullableDouble(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetDouble(index);
        }
        public static decimal? GetNullableDecimal(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            return reader.GetDecimal(index);
        }
        public static string GetString(this IDataReader reader, int index, string defaultValue)
        {
            if (reader.IsDBNull(index))
                return defaultValue;
            return reader.GetString(index);
        }
        public static T GetEnum<T>(this IDataReader reader, int index) where T : struct
        {
            return (T)Enum.Parse(typeof(T), reader[index].ToString());
        }
    }
}