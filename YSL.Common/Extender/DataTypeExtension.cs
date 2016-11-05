namespace YSL.Common.Extender
{
    /// <summary>
    /// 数据类型的扩展类
    /// </summary>
    public static class DataTypeExtension {
        public static byte[] ToBytes(this short value) {
            int length = 2;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static byte[] ToBytes(this ushort value) {
            int length = 2;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static byte[] ToBytes(this int value) {
            int length = 4;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static byte[] ToBytes(this uint value) {
            int length = 4;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static byte[] ToBytes(this long value) {
            int length = 8;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static byte[] ToBytes(this ulong value) {
            int length = 8;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++) {
                result[i] = (byte)(value >> 8 * (length - 1 - i));
            }
            return result;
        }
        public static string TrimInvaidZero(this decimal value) {
            string originalValue = value.ToString();
            if(originalValue.IndexOf(".") > -1) {
                return value.ToString().TrimEnd('0').TrimEnd('.');
            } else {
                return value.ToString();
            }
        }
    }
}