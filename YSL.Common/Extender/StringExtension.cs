using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace YSL.Common.Extender
{
    /// <summary>
    /// String 扩展类
    /// </summary>
    public static class StringExtension {
        /// <summary>
        /// 覆盖内容
        /// </summary>
        /// <param name="value">待处理的字符串</param>
        /// <param name="index">开始位置</param>
        /// <param name="text">新内容</param>
        public static string Cover(this string value, int index, string text) {
            string result;
            if(value == null) {
                result = text;
            } else if(text == null) {
                result = value;
            } else if(index <= 0) {
                result = text;
                if(value.Length > text.Length) {
                    result += value.Substring(result.Length);
                }
            } else {
                if(value.Length <= index) {
                    result = value + text;
                } else {
                    result = value.Substring(0, index);
                    result += text;
                    if(value.Length - index > text.Length) {
                        result += value.Substring(result.Length);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 取字符串左边指定长度的值
        /// </summary>
        /// <param name="value">待处理的字符串</param>
        /// <param name="length">截取长度</param>
        public static string LeftString(this string value, int length) {
            if (string.IsNullOrEmpty(value) || value.Length <= length) {
                return value ?? string.Empty;
            }
            return value.Substring(0, length);
        }
        /// <summary>
        /// 取字符串右边指定长度的值
        /// </summary>
        /// <param name="value">待处理的字符串</param>
        /// <param name="length">截取长度</param>
        public static string RightString(this string value, int length) {
            if (string.IsNullOrEmpty(value) || value.Length <= length) {
                return value ?? string.Empty;
            }
            return value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// 转换成16位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static short ToShort(this string value) {
            return ToShort(value, 0);
        }
        /// <summary>
        /// 转换成16位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static short ToShort(this string value, short defaultValue) {
            var result = ToNullableShort(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空16位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static short? ToNullableShort(this string value) {
            short result;
            if (short.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成16位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static ushort ToUShort(this string value) {
            return ToUShort(value, 0);
        }
        /// <summary>
        /// 转换成16位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static ushort ToUShort(this string value, ushort defaultValue) {
            var result = ToNullableUShort(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空16位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static ushort? ToNullableUShort(this string value) {
            ushort result;
            if (ushort.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成32位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static int ToInt(this string value) {
            return ToInt(value, 0);
        }
        /// <summary>
        /// 转换成32位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static int ToInt(this string value, int defaultValue) {
            var result = ToNullableInt(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空32位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static int? ToNullableInt(this string value) {
            int result;
            if (int.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成32位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static uint ToUInt(this string value) {
            return ToUInt(value, 0);
        }
        /// <summary>
        /// 转换成32位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static uint ToUInt(this string value, uint defaultValue) {
            var result = ToNullableUInt(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空32位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static uint? ToNullableUInt(this string value) {
            uint result;
            if (uint.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成64位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static long ToLong(this string value) {
            return ToLong(value, 0);
        }
        /// <summary>
        /// 转换成64位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static long ToLong(this string value, long defaultValue) {
            var result = ToNullableLong(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空64位带符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static long? ToNullableLong(this string value) {
            long result;
            if (long.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成64位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static ulong ToULong(this string value) {
            return ToULong(value, 0);
        }
        /// <summary>
        /// 转换成64位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static ulong ToULong(this string value, ulong defaultValue) {
            var result = ToNullableULong(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空64位无符号整数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static ulong? ToNullableULong(this string value) {
            ulong result;
            if (ulong.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成十进制小数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static decimal ToDecimal(this string value) {
            return ToDecimal(value, 0);
        }
        /// <summary>
        /// 转换成十进制小数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static decimal ToDecimal(this string value, decimal defaultValue) {
            var result = ToNullableDecimal(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空十进制小数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static decimal? ToNullableDecimal(this string value) {
            decimal result;
            if (decimal.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成双精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static double ToDouble(this string value) {
            return ToDouble(value, 0);
        }
        /// <summary>
        /// 转换成双精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static double ToDouble(this string value, double defaultValue) {
            var result = ToNullableDouble(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空双精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static double? ToNullableDouble(this string value) {
            double result;
            if (double.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 转换成单精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <returns>转换失败时，返回0；成功时，返回正常值</returns>
        public static float ToFloat(this string value) {
            return ToFloat(value, 0);
        }
        /// <summary>
        /// 转换成单精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换失败时，返回默认值；成功时，返回正常值</returns>
        public static float ToFloat(this string value, float defaultValue) {
            var result = ToNullableFloat(value);
            return result ?? defaultValue;
        }
        /// <summary>
        /// 转换成可空单精度浮点数
        /// </summary>
        /// <param name="value">待转换的字符串</param>
        public static float? ToNullableFloat(this string value) {
            float result;
            if (float.TryParse(value, out result)) {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 判断字符串是否日期(带时间)格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <returns>如果字符串是日期(带时间)格式类型，返回 true，否则返回 false.</returns>
        public static bool IsDateTime(this string value) {
            value = Regex.Replace(Regex.Replace(Regex.Replace(value, @"[\.\/年月]", "-"), @"[时点分]", ":"), @"[日秒]", "");
            var m = Regex.Match(value, @"^(?<year>\d{2,4})\-(?<month>\d{1,2})\-(?<day>\d{1,2})(\s+(?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2})(\s+(?<millsecond>\d{1,3}))?)?$");
            if (m.Success) {
                var year = string.IsNullOrEmpty(m.Groups["year"].Value) ? 0 : int.Parse(m.Groups["year"].Value);
                var month = string.IsNullOrEmpty(m.Groups["month"].Value) ? 1 : int.Parse(m.Groups["month"].Value);
                var day = string.IsNullOrEmpty(m.Groups["day"].Value) ? 1 : int.Parse(m.Groups["day"].Value);
                var hour = string.IsNullOrEmpty(m.Groups["hour"].Value) ? 0 : int.Parse(m.Groups["hour"].Value);
                var minute = string.IsNullOrEmpty(m.Groups["minute"].Value) ? 0 : int.Parse(m.Groups["minute"].Value);
                var second = string.IsNullOrEmpty(m.Groups["second"].Value) ? 0 : int.Parse(m.Groups["second"].Value);
                var millSecond = string.IsNullOrEmpty(m.Groups["millsecond"].Value) ? 0 : int.Parse(m.Groups["millsecond"].Value);

                return month <= 12 && day <= DateTime.DaysInMonth(year, month) && hour <= 23 && minute <= 59 && second <= 59 && millSecond <= 999;
            }
            return false;
        }
        /// <summary>
        /// 判断一个字符串是否是布尔值（true 或 false）格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <returns>如果字符串为 true 或者 false（不区分大小写），返回 true；否则返回 false。</returns>
        public static bool IsBool(this string value) {
            return regexBool.IsMatch(value);
        }
        /// <summary>
        /// 判断一个字符串是否是 Guid 格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <returns>如果字符串为 Guid 格式（xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx），返回 true；否则返回 false。</returns>
        public static bool IsGuid(this string value) {
            return regexGuid.IsMatch(value);
        }
        /// <summary>
        /// 判断一个字符串是否是 整数 格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        public static bool IsInteger(this string value) {
            return regexInteger.IsMatch(value);
        }
        /// <summary>
        /// 判断一个字符串是否是 数字（整数和小数） 格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        public static bool IsNumber(this string value) {
            return regexNumber.IsMatch(value);
        }
        /// <summary>
        /// 判断一个字符串是否是 IP地址(IP4) 格式。
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        public static bool IsIP4Address(this string value) {
            return regexIP4Address.IsMatch(value);
        }
        /// <summary>
        /// 判断一个字符串是否是 被指定字符串 包住
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <param name="surroundString">外围字符串</param>
        public static bool IsSurroundWith(this string value, string surroundString) {
            if (string.IsNullOrEmpty(surroundString)) return true;
            return Regex.IsMatch(value, Regex.Escape(surroundString));
        }
        /// <summary>
        /// 判断一个字符串是否是 被指定字符 包住
        /// </summary>
        /// <param name="value">要判断的字符串。</param>
        /// <param name="surroundChar">外围字符</param>
        public static bool IsSurroundWith(this string value, char surroundChar) {
            return Regex.IsMatch(value, Regex.Escape(surroundChar.ToString()));
        }

        public static string TrimStart(this string source, char[] chars, int count = -1) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Length == 0 || count == 0) {
                return source;
            }
            if (chars == null || chars.Length == 0) {
                chars = new[] { ' ' };
            }
            count = count < 0 ? source.Length : count;
            for (var i = 0; i < count; i++) {
                if (chars.Contains(source[0])) {
                    source = source.Substring(1);
                }
            }
            return source;
        }
        public static string TrimStart(this string source, string[] strings, int count = -1) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Length == 0 || count == 0) {
                return source;
            }
            if (strings == null || strings.Length == 0) {
                strings = new[] { " " };
            }
            strings = strings.Select(Regex.Escape).ToArray();
            var m = Regex.Match(source, string.Format("^({0}{1})(?<cnt>.*)", strings.Join("|"), count < 0 ? "*" : string.Format("{{0,{0}}}", count)));
            return m.Success ? m.Groups["cnt"].Value : source;
        }
        public static string TrimEnd(this string source, char[] chars, int count = -1) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Length == 0 || count == 0) {
                return source;
            }
            if (chars == null || chars.Length == 0) {
                chars = new[] { ' ' };
            }
            count = count < 0 ? source.Length : count;
            for (var i = 0; i < count; i++) {
                if (source.Length == 0) {
                    return string.Empty;
                }
                var pos = source.Length - 1;
                if (chars.Contains(source[pos])) {
                    source = source.Substring(0, pos);
                }
            }
            return source;
        }
        public static string TrimEnd(this string source, string[] strings, int count = -1) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Length == 0 || count == 0) {
                return source;
            }
            if (strings == null || strings.Length == 0) {
                strings = new[] { " " };
            }
            strings = strings.Select(Regex.Escape).ToArray();
            var m = Regex.Match(source, string.Format("(?<cnt>.*)({0}{1})$", strings.Join("|"), count < 0 ? "*" : string.Format("{{0,{0}}}", count)));
            return m.Success ? m.Groups["cnt"].Value : source;
        }
        public static string Trim(this string source, char[] chars, int count = -1, bool match = false) {
            var strings = chars == null || chars.Length == 0 ? new[] { " " } : chars.Select(c => new string(new[] { c })).ToArray();
            return Trim(source, strings, count, match);
        }
        public static string Trim(this string source, string[] strings, int count = -1, bool match = false) {
            if (source == null)
                throw new ArgumentNullException("source");
            if (source.Length == 0 || count == 0) {
                return source;
            }
            if (strings == null || strings.Length == 0) {
                strings = new[] { " " };
            }
            strings = strings.Select(Regex.Escape).ToArray();
            var trim = string.Format("{0}{1}", strings.Join("|"), count < 0 ? "*" : (count == 1 ? "?" : string.Format("{{0,{0}}}", count)));
            var m = Regex.Match(source, string.Format("^({0})(?<cnt>.*){1}$", trim, match ? @"\1" : trim));
            return m.Success ? m.Groups["cnt"].Value : source;
        }

        private static readonly Regex regexBool = new Regex(@"^true|false$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexGuid = new Regex(@"^('|""?)[a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12}\1$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexInteger = new Regex(@"^-?\d+$", RegexOptions.Compiled);
        private static readonly Regex regexNumber = new Regex(@"^-?\d+(\.\d+)?$", RegexOptions.Compiled);
        private static readonly Regex regexIP4Address = new Regex(@"^((25[0-5]|2[0-4]\d|(1\d|[1-9])?\d)\.){3}(25[0-5]|2[0-4]\d|(1\d|[1-9])?\d)$", RegexOptions.Compiled);
    }
}