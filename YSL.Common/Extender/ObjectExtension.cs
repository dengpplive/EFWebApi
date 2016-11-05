namespace YSL.Common.Extender
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Object 扩展类
    /// </summary>
    public static class ObjectExtension {
        /// <summary>
        /// 深拷贝
        /// </summary>
        public static T Copy<T>(this T value) {
            if (value != null) {
                if (value.IsSerializable()) {
                    using (Stream stream = new MemoryStream()) {
                        IFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                        formatter.Serialize(stream, value);
                        stream.Seek(0, SeekOrigin.Begin);
                        return (T)formatter.Deserialize(stream);
                    }
                }
                throw new SerializationException("类型\"" + value.GetType().ToString() + "\"未标记为可序列化");
            }
            return default(T);
        }
        /// <summary>
        /// 是否可序列化
        /// </summary>
        public static bool IsSerializable<T>(this T value) {
            return value.GetType().IsSerializable;
        }
    }
}