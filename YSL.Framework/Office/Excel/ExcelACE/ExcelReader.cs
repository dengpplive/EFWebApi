using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace ExcelACE {
    /// <summary>
    /// 提供一组方法，用于将 Excel 工作簿中的内容读取到对象序列，或 DataTable，DataSet 中。
    /// </summary>
    public static class ExcelReader {
        //private const string ReadConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=\"{0}\";Extended Properties=\"{1};HDR={2};{3};\";";
        private const string ReadConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=\"{0}\";Extended Properties=\"{1};HDR={2};{3};\";";
        private const string SelectCommand = "SELECT * FROM [{0}]";

        /// <summary>
        /// 读取 Excel 工作簿中指定工作表的内容。
        /// </summary>
        /// <typeparam name="T">结果序列中元素的类型</typeparam>
        /// <param name="selector">投影函数，用于将读取到的数据记录投影到指定类型的对象实例</param>
        /// <param name="path">Excel 文件路径</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="start">读取的开始位置，例：C5 表示从 C 列第 5 行开始</param>
        /// <param name="end">读取的结束位置，例：X20 表示读取到 X 列第 20 行</param>
        /// <param name="readHead">是否将读取到的第一行内容作为列头</param>
        /// <param name="ignoreType">是否忽略数据在 Excel 中定义的类型，而全部作为字符串读取</param>
        /// <returns>返回读取到的结果</returns>
        public static IEnumerable<T> Read<T>(Func<IDataRecord, T> selector, string path, string sheetName = "", string start = "", string end = "", bool readHead = true, bool ignoreType = true) {
            if (selector == null) { throw new ArgumentNullException("selector"); }
            if (string.IsNullOrWhiteSpace(path)) { throw new InvalidOperationException("无效的文件路径。"); }
            if (string.IsNullOrWhiteSpace(start) ^ string.IsNullOrWhiteSpace(end)) { throw new InvalidOperationException("参数 \"start\" 和 \"end\" 必须同时设置。"); }

            var ext = Path.GetExtension(path).Trim('.').ToLower();
            var hdr = readHead ? "Yes" : "No";
            var mex = ignoreType ? "IMEX=1" : "";
            string connStr, select;
            switch (ext) {
                case "csv":
                    connStr = string.Format(ReadConnectionString, Path.IsPathRooted(path) ? Path.GetDirectoryName(path) : AppDomain.CurrentDomain.BaseDirectory, "Text", hdr, mex);
                    select = string.Format(SelectCommand, Path.GetFileName(path));
                    break;
                case "xls":
                case "xlsx":
                    if (string.IsNullOrWhiteSpace(sheetName)) {
                        throw new ArgumentException("无效的工作表名称。");
                    }
                    var range = string.IsNullOrWhiteSpace(start) ? string.Empty : string.Format("{0}:{1}", start, end);
                    connStr = string.Format(ReadConnectionString, path, "Excel 8.0", hdr, mex);
                    select = string.Format(SelectCommand, string.Format("{0}${1}", sheetName, range));
                    break;
                default:
                    throw new InvalidOperationException(string.Format("尚不支持读取 \"{0}\" 格式的数据文件。", ext));
            }

            using (var conn = new OleDbConnection(connStr)) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = select;
                    using (var reader = cmd.ExecuteReader()) {
                        if (reader == null)
                            yield break;
                        while (reader.Read()) {
                            var record = selector(reader);
                            if (typeof(T).IsClass && !Equals(record, null))
                                yield return selector(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将 Excel 工作簿中指定工作表的内容填充到 DataTable 中
        /// </summary>
        /// <param name="path">Excel 文件路径</param>
        /// <param name="sheet">工作表名称</param>
        /// <param name="start">读取的开始位置，例：C5 表示从 C 列第 5 行开始</param>
        /// <param name="end">读取的结束位置，例：X20 表示读取到 X 列第 20 行</param>
        /// <param name="readHead">是否将读取到的第一行内容作为列头</param>
        /// <param name="ignoreType">是否忽略数据在 Excel 中定义的类型，而全部作为字符串读取</param>
        /// <returns>返回填充数据后的 DataTable</returns>
        public static DataTable FillDataTable(string path, string sheet="", string start = "", string end = "", bool readHead = true, bool ignoreType = true) {
            if (string.IsNullOrWhiteSpace(path)) { throw new InvalidOperationException("无效的文件路径。"); }
            if (string.IsNullOrWhiteSpace(start) ^ string.IsNullOrWhiteSpace(end)) { throw new InvalidOperationException("参数 \"start\" 和 \"end\" 必须同时设置。"); }

            var ext = Path.GetExtension(path).Trim('.').ToLower();
            var hdr = readHead ? "Yes" : "No";
            var mex = ignoreType ? "IMEX=1" : "";
            string connStr, select;
            switch (ext) {
                case "csv":
                    connStr = string.Format(ReadConnectionString, Path.IsPathRooted(path) ? Path.GetDirectoryName(path) : AppDomain.CurrentDomain.BaseDirectory, "Text", hdr, mex);
                    select = string.Format(SelectCommand, Path.GetFileName(path));
                    break;
                case "xls":
                case "xlsx":
                    if (string.IsNullOrWhiteSpace(sheet)) {
                        throw new ArgumentException("无效的工作表名称。");
                    }
                    var range = string.IsNullOrWhiteSpace(start) ? string.Empty : string.Format("{0}:{1}", start, end);
                    connStr = string.Format(ReadConnectionString, path, "Excel 8.0", hdr, mex);
                    select = string.Format(SelectCommand, string.Format("{0}${1}", sheet, range));
                    break;
                default:
                    throw new InvalidOperationException(string.Format("尚不支持读取 \"{0}\" 格式的数据文件。", ext));
            }

            using (var conn = new OleDbConnection(connStr)) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = select;
                    var adapter = new OleDbDataAdapter(cmd);
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        /// <summary>
        /// 将 Excel 工作簿中的所有内容填充到 DataSet 中
        /// </summary>
        /// <param name="path">Excel 文件路径</param>
        /// <param name="readHead">是否将读取到的第一行内容作为列头</param>
        /// <param name="ignoreType">是否忽略数据在 Excel 中定义的类型，而全部作为字符串读取</param>
        /// <returns>返回填充数据后的 DataSet</returns>
        public static DataSet FillDataSet(string path, bool readHead = true, bool ignoreType = true) {
            if (string.IsNullOrWhiteSpace(path)) { throw new InvalidOperationException("无效的文件路径。"); }
            using (var conn = new OleDbConnection(string.Format(ReadConnectionString, path, readHead ? "Yes" : "No", ignoreType ? "IMEX=1" : string.Empty))) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    var dataset = new DataSet();
                    var tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    if (tables != null && tables.Rows.Count > 0) {
                        var selectCmd = string.Join(";", tables.Rows.Cast<DataRow>().Select(row => string.Format(SelectCommand, row["TABLE_NAME"], "")));
                        cmd.CommandText = selectCmd;
                        var adapter = new OleDbDataAdapter(cmd);
                        adapter.Fill(dataset);
                    }
                    return dataset;
                }
            }
        }


        static void Test() {
            var s = ExcelReader.Read<object>(r => null, "3702dd85-1a64-4d7e-8b57-cf540dc42203(1).CSV").ToArray();
        }
    }
}
