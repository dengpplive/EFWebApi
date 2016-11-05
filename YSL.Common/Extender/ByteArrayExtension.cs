namespace YSL.Common.Extender
{
    using System;
    using YSL.Common.Exceptions;

    /// <summary>
    /// Byte数组 扩展类
    /// </summary>
    public static class ByteArrayExtension {
        private static void checkLength(byte[] data) { if (data.Length == 0) { throw new ZeroLengthArrayException("data"); } }
        private static long toLong(byte[] data, int offset, int width) {
            if (width < 0 || width > 8) { throw new ArgumentOutOfRangeException("width", "width must between 1 and 8"); }
            if (offset < 0 || offset > data.Length - 1) { throw new IndexOutOfRangeException(); }
            if (offset + width > data.Length) { width = data.Length - offset; }
            long result = 0;
            for (int i = offset; i < width + offset; i++) {
                unchecked {
                    result += (data[i] << 8 * (width - 1 - i + offset));
                }
            }
            return result;
        }

        public static short ToShort(this byte[] data) {
            return ToShort(data, 0);
        }
        public static short ToShort(this byte[] data, int offset) {
            return ToShort(data, offset, 2);
        }
        public static short ToShort(this byte[] data, int offset, int count) {
            checkLength(data);
            return (short)toLong(data, offset, count);
        }

        public static ushort ToUShort(this byte[] data) {
            return ToUShort(data, 0);
        }
        public static ushort ToUShort(this byte[] data, int offset) {
            return ToUShort(data, offset, 2);
        }
        public static ushort ToUShort(this byte[] data, int offset, int count) {
            checkLength(data);
            return (ushort)toLong(data, offset, count);
        }

        public static int ToInt(this byte[] data) {
            return ToInt(data, 0);
        }
        public static int ToInt(this byte[] data, int offset) {
            return ToInt(data, offset, 4);
        }
        public static int ToInt(this byte[] data, int offset, int count) {
            checkLength(data);
            return (int)toLong(data, offset, count);
        }

        public static uint ToUInt(this byte[] data) {
            return ToUInt(data, 0);
        }
        public static uint ToUInt(this byte[] data, int offset) {
            return ToUInt(data, offset, 4);
        }
        public static uint ToUInt(this byte[] data, int offset, int count) {
            checkLength(data);
            return (uint)toLong(data, offset, count);
        }

        public static long ToLong(this byte[] data) {
            return ToLong(data, 0);
        }
        public static long ToLong(this byte[] data, int offset) {
            return ToLong(data, offset, 8);
        }
        public static long ToLong(this byte[] data, int offset, int count) {
            checkLength(data);
            return (long)toLong(data, offset, count);
        }

        public static ulong ToULong(this byte[] data) {
            return ToULong(data, 0);
        }
        public static ulong ToULong(this byte[] data, int offset) {
            return ToULong(data, offset, 8);
        }
        public static ulong ToULong(this byte[] data, int offset, int count) {
            checkLength(data);
            return (ulong)toLong(data, offset, count);
        }
    }
}