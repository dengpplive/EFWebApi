#define DataFileResourceMode
#define KeepDataContentInMemoryMode

namespace YSL.Framework.AddressLocator
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;

    public class CityLocator {
        static readonly string dataFileName = "QQWry.Dat";
#if KeepDataContentInMemoryMode
        static Stream dataFileStream = null;
#endif
        public static IPLocation GetIPLocation(IPAddress ipAddress) {
            return GetIPLocation(ipAddress.ToString());
        }
        public static IPLocation GetIPLocation(string ip) {
            Stream dataStream = getDataStream();
            BinaryReader binaryReader = new BinaryReader(dataStream);
            //读文件头,获取首末记录偏移量
            int firstOffset = binaryReader.ReadInt32();
            int lastOffset = binaryReader.ReadInt32();
            //IP值
            uint ipValue = ipStringToInt(ip);
            // 获取IP索引记录偏移值
            int rcOffset = getIndexOffset(dataStream, binaryReader, firstOffset, lastOffset, ipValue);
            dataStream.Seek(rcOffset, System.IO.SeekOrigin.Begin);

            string ipStart, ipEnd, country, city;
            if (rcOffset >= 0) {
                dataStream.Seek(rcOffset, System.IO.SeekOrigin.Begin);
                //读取开头IP值
                ipStart = uintToIpString(binaryReader.ReadUInt32());
                //转到记录体
                dataStream.Seek(readInt24(binaryReader), System.IO.SeekOrigin.Begin);
                //读取结尾IP值
                ipEnd = uintToIpString(binaryReader.ReadUInt32());
                country = getCity(dataStream, binaryReader);
                city = getCity(dataStream, binaryReader).Replace("CZ88.NET", "").Replace("对方和您在同一内部网", "");
            } else {
                ipStart = "0.0.0.0";
                ipEnd = "0.0.0.0";
                country = "未知国家";
                city = "未知地址";
            }
            closeDataStream(dataStream, binaryReader);
            return new IPLocation(ipStart, ipEnd, country, city);
        }
        static Stream getDataStream() {
            Stream result;
#if KeepDataContentInMemoryMode // 常驻内存方式
            if (dataFileStream == null) {
                result = dataFileStream = createStream();
            } else if (!dataFileStream.CanSeek) {
                dataFileStream.Close();
                result = dataFileStream = createStream();
            } else {
                result = dataFileStream;
            }
#else // 每次读取方式
            result = createStream();
#endif
            result.Position = 0;
            return result;
        }
        static void closeDataStream(Stream dataStream, BinaryReader reader) {
#if KeepDataContentInMemoryMode // 常驻内存方式，无须关闭流
#else // 每次读取方式，需将流关闭
            if (reader != null) {
                reader.Close();
            }
            if (dataStream != null) {
                dataStream.Close();
            }
#endif
        }
        static Stream createStream() {
#if DataFileResourceMode // 嵌入式资源方式
            string resourceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "." + dataFileName;
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
#else // 外部资源方式
            string dataFileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataFileName);
            if (!File.Exists(dataFileFullName)) {
                throw new Exception("文件不存在!");
            }
            return new FileStream(dataFileFullName, FileMode.Open, FileAccess.Read, FileShare.Read);
#endif
        }
        // 函数功能: 采用“二分法”搜索索引区, 定位IP索引记录位置
        static int getIndexOffset(Stream dataStream, BinaryReader binaryReader, int firstOffset, int lastOffset, uint ipValue) {
            int _firstOffset = firstOffset, _lastOffset = lastOffset;
            int _middleOffset;    //中间偏移量
            uint _middleValue;    //中间值
            uint _firstValue, _lastValue; //边界值
            uint llv;   //边界末末值
            dataStream.Seek(_firstOffset, System.IO.SeekOrigin.Begin);
            _firstValue = binaryReader.ReadUInt32();
            dataStream.Seek(_lastOffset, System.IO.SeekOrigin.Begin);
            _lastValue = binaryReader.ReadUInt32();
            //临时作它用,末记录体偏移量
            _middleOffset = readInt24(binaryReader);
            dataStream.Seek(_middleOffset, System.IO.SeekOrigin.Begin);
            llv = binaryReader.ReadUInt32();
            //边界检测处理
            if (ipValue < _firstValue)
                return -1;
            else if (ipValue > llv)
                return -1;
            //使用"二分法"确定记录偏移量
            do {
                _middleOffset = _firstOffset + (_lastOffset - _firstOffset) / 7 / 2 * 7;
                dataStream.Seek(_middleOffset, System.IO.SeekOrigin.Begin);
                _middleValue = binaryReader.ReadUInt32();
                if (ipValue >= _middleValue)
                    _firstOffset = _middleOffset;
                else
                    _lastOffset = _middleOffset;
                if (_lastOffset - _firstOffset == 7)
                    _middleOffset = _lastOffset = _firstOffset;
            } while (_firstOffset != _lastOffset);
            return _middleOffset;
        }
        static bool isNumeric(string value) {
            return !string.IsNullOrWhiteSpace(value)
                && System.Text.RegularExpressions.Regex.IsMatch(value, @"^-?\d+$");
        }
        static uint ipStringToInt(string ipString) {
            uint result = 0;
            string[] ipStringArray = ipString.Split('.');
            for (int index = 0; index < 4 && index < ipStringArray.Length; index++) {
                if (isNumeric(ipStringArray[index])) {
                    uint ipSegmentValue = (uint)Math.Abs(Convert.ToInt32(ipStringArray[index]));
                    if (ipSegmentValue > 255) ipSegmentValue = 255;
                    result += ipSegmentValue << (3 - index) * 8;
                }
            }
            return result;
        }
        static string uintToIpString(uint ipValue) {
            return (ipValue >> 24) + "." + ((ipValue & 0x00FF0000) >> 16) + "." + ((ipValue & 0x0000FF00) >> 8) + "." + (ipValue & 0x000000FF);
        }
        static string readString(BinaryReader binaryReader) {
            byte[] tempByteArray = new byte[128];
            int index = 0;
            do {
                tempByteArray[index] = binaryReader.ReadByte();
            } while (tempByteArray[index++] != '\0' && index < 128);
            return System.Text.Encoding.Default.GetString(tempByteArray).TrimEnd('\0');
        }
        static int readInt24(BinaryReader binaryReader) {
            if (binaryReader == null) return -1;
            int result = 0;
            result |= (int)binaryReader.ReadByte();
            result |= (int)binaryReader.ReadByte() << 8 & 0xFF00;
            result |= (int)binaryReader.ReadByte() << 16 & 0xFF0000;
            return result;
        }
        // 读取IP所在地字符串
        static string getCity(Stream dataStream, BinaryReader binaryReader) {
            int offset;
            switch (binaryReader.ReadByte()) {
                case 0x01:
                    // 重定向模式1: 城市信息随国家信息定向
                    offset = readInt24(binaryReader);
                    dataStream.Seek(offset, System.IO.SeekOrigin.Begin);
                    return getCity(dataStream, binaryReader);
                case 0x02:
                    // 重定向模式2: 城市信息没有随国家信息定向
                    offset = readInt24(binaryReader);
                    int tmpOffset = (int)dataStream.Position;
                    dataStream.Seek(offset, System.IO.SeekOrigin.Begin);
                    string tmpString = getCity(dataStream, binaryReader);
                    dataStream.Seek(tmpOffset, System.IO.SeekOrigin.Begin);
                    return tmpString;
                default:
                    // 无重定向: 最简单模式
                    dataStream.Seek(-1, System.IO.SeekOrigin.Current);
                    return readString(binaryReader).Trim();
            }
        }
    }

    public struct IPLocation {
        string _ipStart;
        string _ipEnd;
        string _country;
        string _city;
        public IPLocation(string ipStart, string ipEnd, string country, string city) {
            _ipStart = ipStart;
            _ipEnd = ipEnd;
            _country = country;
            _city = city;
        }
        public string IPStart { get { return _ipStart; } }
        public string IPEnd { get { return _ipEnd; } }
        public string Country { get { return _country; } }
        public string City { get { return _city; } }
        public override string ToString() {
            return _country + _city;
        }
    }
}