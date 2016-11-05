using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExcelACE {
    /// <summary>
    /// 提供一组方法，用于将对象序列，或 DataTable，DataSet 中的数据写入到 Excel 工作簿中。
    /// </summary>
    public static class ExcelWriter {
        //private const string WriteConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;data source={0};Extended Properties=Excel 8.0;";
        private const string WriteConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source={0};Extended Properties=Excel 8.0;";
        private const string InsertCommand = "INSERT INTO [{0}$] ({1}) VALUES ({2})";
        private const string CreateTableCommand = "CREATE TABLE {0} ({1})";

        /// <summary>
        /// 在指定的 Excel 工作簿中创建一个工作表。
        /// </summary>
        /// <param name="path">工作簿所在路径</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="fields">字段描述</param>
        /// <returns>成功创建或指定工作表已存在时返回 true。</returns>
        public static bool CreateSheet(string path, string sheetName, params FieldSummary[] fields) {
            if (string.IsNullOrWhiteSpace(sheetName)) { throw new ArgumentException("未指定工作表名称。"); }
            if (fields == null || fields.Length == 0) { throw new InvalidOperationException("缺少字段。"); }

            var fieldList = string.Join(",", fields.Select(f => f.ToString()));
            using (var conn = new OleDbConnection(string.Format(WriteConnectionString, path))) {
                conn.Open();
                var tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (tables != null && tables.Rows.Cast<DataRow>().Any(r => r["TABLE_NAME"].ToString() == sheetName)) { return true; }
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = string.Format(CreateTableCommand, sheetName, fieldList);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        /// <summary>
        /// 在指定的 Excel 工作簿中创建一个工作表。
        /// </summary>
        /// <param name="path">工作簿所在路径</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="fields">字段描述</param>
        /// <returns>成功创建或指定工作表已存在时返回 true。</returns>
        public static bool CreateSheet(string path, string sheetName, IEnumerable<FieldSummary> fields) {
            return CreateSheet(path, sheetName, fields.ToArray());
        }

        /// <summary>
        /// 将数据写入指定的 Excel 文件。
        /// </summary>
        /// <typeparam name="T">源数据类型</typeparam>
        /// <param name="source">要写入的数据</param>
        /// <param name="path">Excel 文件路径（如指定文件不存在，则创建新文件。）</param>
        /// <param name="sheetName">Excel 工作表名称（如指定工作表不存在，则创建新的工作表。）</param>
        public static void WriteExcel<T>(IEnumerable<T> source, string path, string sheetName) {
            if (source == null) { throw new ArgumentNullException("source"); }
            var props = typeof(T).GetProperties();
            var fields = props.Select(p => new FieldSummary(p.Name, p.PropertyType)).ToArray();

            var created = CreateSheet(path, sheetName, fields);
            if (created) {
                var fieldList = string.Join(",", fields.Select(f => f.Name));
                var valueList = string.Join(",", fields.Select(f => "@" + f.Name));
                var insert = string.Format(InsertCommand, sheetName, fieldList, valueList);
                using (var conn = new OleDbConnection(string.Format(WriteConnectionString, path))) {
                    conn.Open();
                    using (var cmd = conn.CreateCommand()) {
                        cmd.CommandText = insert;
                        foreach (var item in source) {
                            cmd.Parameters.Clear();
                            var args = fields.Select(f => new OleDbParameter("@" + f.Name, GetValue(item, props.First(p => p.Name == f.Name)) ?? DBNull.Value) { IsNullable = true }).ToArray();
                            cmd.Parameters.AddRange(args);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            else { throw new InvalidCastException(string.Format("创建工作表 \"{0}\" 失败。", sheetName)); }
        }

        /// <summary>
        /// 将 DataTable 中的数据写入指定的 Excel 文件。
        /// </summary>
        /// <param name="source">要写入的数据</param>
        /// <param name="path">Excel 文件路径（如指定文件不存在，则创建新文件。）</param>
        /// <param name="sheetName">Excel 工作表名称（如指定工作表不存在，则创建新的工作表。）</param>
        public static void WriteExcel(DataTable source, string path, string sheetName = "") {
            if (source == null) { throw new ArgumentNullException("source"); }
            sheetName = string.IsNullOrWhiteSpace(sheetName) ? source.TableName : sheetName;
            var fields = source.Columns.Cast<DataColumn>().Select(c => new FieldSummary(c.ColumnName, c.DataType)).ToArray();
            var created = CreateSheet(path, sheetName, fields);
            if (created) {
                var fieldList = string.Join(",", fields.Select(f => f.Name));
                var valueList = string.Join(",", fields.Select(f => "@" + f.Name));
                var insert = string.Format(InsertCommand, sheetName, fieldList, valueList);
                using (var conn = new OleDbConnection(string.Format(WriteConnectionString, path))) {
                    conn.Open();
                    using (var cmd = conn.CreateCommand()) {
                        cmd.CommandText = insert;
                        foreach (DataRow row in source.Rows) {
                            cmd.Parameters.Clear();
                            var args = fields.Select(f => new OleDbParameter("@" + f.Name, row[f.Name]) { IsNullable = true }).ToArray();
                            cmd.Parameters.AddRange(args);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            else { throw new InvalidCastException(string.Format("创建工作表 \"{0}\" 失败。", sheetName)); }
        }

        /// <summary>
        /// 将 DataSet 中的数据写入指定的 Excel 文件。
        /// </summary>
        /// <param name="source">要写入的数据</param>
        /// <param name="path">Excel 文件路径（如指定文件不存在，则创建新文件。）</param>
        public static void WriteExcel(DataSet source, string path) {
            if (source == null) { throw new ArgumentNullException("source"); }
            foreach (DataTable table in source.Tables) { WriteExcel(table, path, table.TableName); }
        }

        /// <summary>
        /// 将数据写入指定的 CSV 格式文件。
        /// </summary>
        /// <typeparam name="T">源数据类型</typeparam>
        /// <param name="source">要写入的数据</param>
        /// <param name="path">CSV 文件路径（如指定文件不存在，则创建新文件。）</param>
        /// <param name="encoding">采用的编码格式，默认为 utf-8</param>
        public static void WriteCSV<T>(IEnumerable<T> source, string path, Encoding encoding = null) {
            if (source == null) { throw new ArgumentNullException("source"); }
            var buffer = new StringBuilder(8192);
            var type = typeof(T);
            var props = type.GetProperties();
            buffer.Append(string.Join(",", props.Select(p => p.Name)));
            buffer.Append("\r\n");
            //buffer.Append(string.Join("\r\n", source.Select(item => string.Join(",", props.Select(p => { var val = GetValue(item, p); return val == DBNull.Value ? string.Empty : val.ToString(); })))));
            var len = props.Length;
            foreach (var item in source) {
                var values = new object[len];
                for (var i = 0; i < len; i++) {
                    var value = (GetValue(item, props[i]) ?? string.Empty).ToString();
                    values[i] = value.IndexOf(',') < 0 ? value : string.Format("\"{0}\"", value);
                }
                buffer.Append(string.Join(",", values));
                buffer.Append("\r\n");
            }

            using (var writer = new StreamWriter(path, false, encoding ?? Encoding.UTF8)) {
                writer.Write(buffer.ToString());
                writer.Flush();
            }
        }

        /// <summary>
        /// 将 DataTable 中的数据写入指定的 CSV 格式文件。
        /// </summary>
        /// <param name="source">要写入的数据</param>
        /// <param name="path">CSV 文件路径（如指定文件不存在，则创建新文件。）</param>
        /// <param name="encoding">采用的编码格式，默认为 utf-8</param>
        public static void WriteCSV(DataTable source, string path, Encoding encoding = null) {
            if (source == null)
                throw new ArgumentNullException("source");

            var buffer = new StringBuilder(8192);
            buffer.Append(string.Join(",", source.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
            buffer.Append("\r\n");
            foreach (DataRow row in source.Rows) {
                buffer.Append(string.Join(",", row.ItemArray.Select(item => {
                    var val = (item ?? string.Empty).ToString();
                    return val.IndexOf(',') < 0 ? val : string.Format("\"{0}\"", val);
                })));
                buffer.Append("\r\n");
            }

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new StreamWriter(path, false, encoding ?? Encoding.UTF8)) {
                writer.Write(buffer.ToString());
                writer.Flush();
            }
        }

        private static object GetValue(object obj, PropertyInfo prop) {
            var type = prop.PropertyType;
            var value = prop.GetValue(obj, null);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) { return string.Join("-", value as IEnumerable<object> ?? Enumerable.Empty<object>()); }
            return value;
        }
    }
}
