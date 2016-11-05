using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using NPOI.HSSF.UserModel;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using NPOI.SS.UserModel;

namespace YSL.Common.Utility
{
    /// <summary>
    /// Excel导入导出帮助类
    /// </summary>
    public static class NPOIExcelHelper
    {
        private static Regex reg = new Regex("^-?\\d+(.\\d+)?$");

        #region Excel导出
        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>
        /// <param name="sourceDs">要导出数据的DataTable</param>    
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>    
        private static Stream ExportDataSetToExcel(DataSet sourceDs, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string[] sheetNames = sheetName.Split(',');
            for (int i = 0; i < sheetNames.Length; i++)
            {
                var sheet = workbook.CreateSheet(sheetNames[i]);//HSSFSheet
                var headerRow = sheet.CreateRow(0);//HSSFRow
                // handling header.            
                foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                // handling value.            
                int rowIndex = 1;
                foreach (DataRow row in sourceDs.Tables[i].Rows)
                {
                    var dataRow = sheet.CreateRow(rowIndex);//HSSFRow
                    foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    {


                        if (column.DataType != typeof(DBNull) && reg.IsMatch(row[column].ToString()))
                        {
                            dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToDouble(row[column]));
                        }
                        else
                        {
                            dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                        }
                    }
                    rowIndex++;
                }
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>   
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <returns>Excel工作表</returns>    
        public static void ExportDataSetToExcel(DataSet sourceDs, string fileName, string sheetName)
        {
            fileName = HttpUtility.UrlEncode(fileName); //对文件名编码
            fileName = fileName.Replace("+", "%20"); //解决空格被编码为"+"号的问题

            MemoryStream ms = ExportDataSetToExcel(sourceDs, sheetName) as MemoryStream;
            HttpContext.Current.Response.AppendHeader("content-disposition", "attachment;filename=" + (fileName == string.Empty ? "Excle" : fileName) + ".xls");
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
            ms.Close();
            ms = null;
        }
        /// <summary>
        /// 由DataTable导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param> 
        /// <returns>Excel工作表</returns>    
        private static Stream ExportDataTableToExcel(DataTable sourceTable, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            var sheet = workbook.CreateSheet(sheetName);//HSSFSheet
            var headerRow = sheet.CreateRow(0);//HSSFRow
            // handling header.      
            foreach (DataColumn column in sourceTable.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            // handling value.      
            int rowIndex = 1;
            foreach (DataRow row in sourceTable.Rows)
            {
                var dataRow = sheet.CreateRow(rowIndex);//HSSFRow
                foreach (DataColumn column in sourceTable.Columns)
                {
                    if (column.DataType != typeof(DBNull) && reg.IsMatch(row[column].ToString()))
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToDouble(row[column]));
                    }
                    else
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                }
                rowIndex++;
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            sheet = null;
            headerRow = null;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// 由DataTable导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param> 
        /// <returns>Excel工作表</returns>    
        private static Stream ExportDataTableToExcel(DataTable sourceTable)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            int size = 60000;
            ISheet sheet=null;
            IRow headerRow = null;
            bool dataSaved = false;
            var dbnull = typeof(DBNull);
            var @string = typeof(String);
            int indexRow = 1;
            for (int j = 0; j < sourceTable.Rows.Count; j++)
            {
                if (j % size == 0)
                {
                    dataSaved = false;
                    sheet = workbook.CreateSheet("sheet" + ((j / size) + 1));
                    headerRow = sheet.CreateRow(0);
                    // handling header.      
                    foreach (DataColumn column in sourceTable.Columns)
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    indexRow = 1;
                }
                var row = sourceTable.Rows[j];
                var dataRow = sheet.CreateRow(indexRow);//HSSFRow
                foreach (DataColumn column in sourceTable.Columns)
                {
                    if (column.DataType != dbnull && column.DataType != @string && reg.IsMatch(row[column].ToString()))
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(Convert.ToDouble(row[column].ToString()));
                    }
                    else
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                }
                if ((j + 1) % size == 0)
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    sheet = null;
                    headerRow = null;
                    dataSaved = true;
                }
                indexRow++;
            }
            if (!dataSaved)
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                sheet = null;
                headerRow = null;
            }
            workbook = null;
            return ms;
        }
        ///// <summary>
        ///// 由DataTable导出Excel
        ///// </summary>
        ///// <param name="sourceTable">要导出数据的DataTable</param>
        ///// <param name="fileName">指定Excel工作表名称</param>
        ///// <returns>Excel工作表</returns>
        //public static void ExportDataTableToExcel(DataTable sourceTable, string fileName, string sheetName)
        //{
        //    // fileName = HttpUtility.UrlEncode(fileName); //对文件名编码
        //    fileName = fileName.Replace("+", "%20"); //解决空格被编码为"+"号的问题

        //    MemoryStream ms = ExportDataTableToExcel(sourceTable, sheetName) as MemoryStream;
        //    HttpContext.Current.Response.AppendHeader("content-disposition", "attachment;filename=" + (fileName == string.Empty ? "Excle" : fileName) + ".xls");
        //    HttpContext.Current.Response.BinaryWrite(ms.ToArray());
        //    HttpContext.Current.Response.End();
        //    ms.Close();
        //    ms = null;
        //}
        /// <summary>
        /// 由DataTable导出Excel，自动生成sheet名
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <returns>Excel工作表</returns>
        public static void ExportDataTableToExcelAutoSheetName(DataTable sourceTable, string fileName)
        {
            // fileName = HttpUtility.UrlEncode(fileName); //对文件名编码
            fileName = fileName.Replace("+", "%20"); //解决空格被编码为"+"号的问题

            MemoryStream ms = ExportDataTableToExcel(sourceTable) as MemoryStream;
            HttpContext.Current.Response.AppendHeader("content-disposition", "attachment;filename=" + (fileName == string.Empty ? "Excle" : fileName) + ".xls");
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
            ms.Close();
            ms = null;
        }
        /// <summary>
        /// 由DataTable导出Csv
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <returns>Excel工作表</returns>
        public static void ExportDataTableToCsv(DataTable dataTable, string fileName)
        {
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".csv");
            fileName = fileName.Replace("+", "%20"); //解决空格被编码为"+"号的问题 
            StringBuilder str = new StringBuilder();
            string colHeaders = "", ls_item = "";
            int i = 0;
            //定义表对象和行对像，同时用DataSet对其值进行初始化 
            DataRow[] myRow = dataTable.Select("");
            //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符 
            for (i = 0; i < dataTable.Columns.Count; i++)
            {
                if (i == dataTable.Columns.Count - 1)
                {
                    colHeaders += dataTable.Columns[i].Caption.ToString() + "\r\n";
                }
                else
                {
                    colHeaders += dataTable.Columns[i].Caption.ToString() + ",";
                }
            }
            //向HTTP输出流中写入取得的数据信息 
            resp.Write(colHeaders);
            //逐行处理数据 
            foreach (DataRow row in myRow)
            {
                //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n 
                for (i = 0; i < dataTable.Columns.Count; i++)
                {

                    if (i == dataTable.Columns.Count - 1)
                    {
                        ls_item += row[i].ToString() + "\r\n";
                    }
                    else
                    {
                        ls_item += row[i].ToString() + ",";
                    }
                }
                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据 
                resp.Write(ls_item);
                ls_item = "";
            }
            resp.End();
        }
        /// <summary>
        /// 由DataTable导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="fileName">指定Excel工作表名称</param>
        /// <returns>Excel工作表</returns>
        public static void ExportDataTableToExcelModel(DataTable sourceTable, string modelpath, string modelName, string fileName, string sheetName)
        {
            int rowIndex = 2;//从第二行开始，因为前两行是模板里面的内容 
            FileStream file = new FileStream(modelpath + modelName + ".xls", FileMode.Open, FileAccess.Read);//读入excel模板
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            HSSFSheet sheet1 = (HSSFSheet)hssfworkbook.GetSheet("Sheet1");
            sheet1.GetRow(0).GetCell(0).SetCellValue("excelTitle");      //设置表头
            foreach (DataRow row in sourceTable.Rows)
            {   //双循环写入sourceTable中的数据
                rowIndex++;
                int colIndex = 0;
                var xlsrow = sheet1.CreateRow(rowIndex);//HSSFRow
                foreach (DataColumn col in sourceTable.Columns)
                {
                    if (col.DataType != typeof(DBNull) && reg.IsMatch(row[col.ColumnName].ToString()))
                    {
                        xlsrow.CreateCell(colIndex).SetCellValue(Convert.ToDouble(row[col.ColumnName]));
                    }
                    else
                    {
                        xlsrow.CreateCell(colIndex).SetCellValue(row[col.ColumnName].ToString());
                    }

                    colIndex++;
                }
            }
            sheet1.ForceFormulaRecalculation = true;
            FileStream fileS = new FileStream(modelpath + fileName + ".xls", FileMode.Create);//保存
            hssfworkbook.Write(fileS);
            fileS.Close();
            file.Close();
        }


        #endregion



        #region Excel导入
        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="sheetName">Excel工作表名称</param>
        /// <param name="headerRowIndex">Excel表头行索引</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportDataTableFromExcel(Stream excelFileStream, string sheetName, int headerRowIndex)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheet(sheetName);//HSSFSheet
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);//HSSFRow
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (headerRowIndex + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);//HSSFRow
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                    dataRow[j] = row.GetCell(j).ToString();
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>
        /// 由Excel导入DataTable
        /// </summary>    
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>    
        /// <param name="sheetName">Excel工作表名称</param>    
        /// <param name="headerRowIndex">Excel表头行索引</param>    
        /// <returns>DataTable</returns>
        public static DataTable ImportDataTableFromExcel(string excelFilePath, string sheetName, int headerRowIndex)
        {
            using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetName, headerRowIndex);
            }
        }
        /// <summary>    
        /// 由Excel导入DataTable    
        /// </summary>    
        /// <param name="excelFileStream">Excel文件流</param>    
        /// <param name="sheetName">Excel工作表索引</param>    
        /// <param name="headerRowIndex">Excel表头行索引</param>    
        /// <returns>DataTable</returns>    
        public static DataTable ImportDataTableFromExcel(Stream excelFileStream, int sheetIndex, int headerRowIndex)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            var sheet = workbook.GetSheetAt(sheetIndex);//HSSFSheet
            DataTable table = new DataTable();
            var headerRow = sheet.GetRow(headerRowIndex);//HSSFRow
            int cellCount = headerRow.LastCellNum;
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                {
                    // 如果遇到第一个空列，则不再继续向后读取
                    cellCount = i + 1;
                    break;
                }
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (headerRowIndex + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);//HSSFRow
                if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
                {
                    // 如果遇到第一个空行，则不再继续向后读取
                    break;
                }
                DataRow dataRow = table.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    dataRow[j] = row.GetCell(j);
                }
                table.Rows.Add(dataRow);
            }
            excelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>    
        /// 由Excel导入DataTable    
        /// </summary>    
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>    
        /// <param name="sheetName">Excel工作表索引</param>    
        /// <param name="headerRowIndex">Excel表头行索引</param>    
        /// <returns>DataTable</returns>  
        public static DataTable ImportDataTableFromExcel(string excelFilePath, int sheetIndex, int headerRowIndex)
        {
            using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
            {
                return ImportDataTableFromExcel(stream, sheetIndex, headerRowIndex);
            }
        }
        /// <summary>    
        /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable    
        /// </summary>    
        /// <param name="excelFileStream">Excel文件流</param>    
        /// <param name="headerRowIndex">Excel表头行索引</param>    
        /// <returns>DataSet</returns>
        public static DataSet ImportDataSetFromExcel(Stream excelFileStream, int headerRowIndex)
        {
            DataSet ds = new DataSet();
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            for (int a = 0, b = workbook.NumberOfSheets; a < b; a++)
            {
                var sheet = workbook.GetSheetAt(a);//HSSFSheet
                DataTable table = new DataTable();
                var headerRow = sheet.GetRow(headerRowIndex);//HSSFRow
                if (headerRow != null)
                {
                    int cellCount = headerRow.LastCellNum;
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null || headerRow.GetCell(i).StringCellValue.Trim() == "")
                        {
                            // 如果遇到第一个空列，则不再继续向后读取
                            cellCount = i + 1;
                            break;
                        }
                        DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                        table.Columns.Add(column);
                    }
                    // for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    for (int i = (headerRowIndex + 1); i <= sheet.LastRowNum; i++)
                    {
                        var row = sheet.GetRow(i);//HSSFRow
                        if (row == null || row.GetCell(0) == null || row.GetCell(0).ToString().Trim() == "")
                        {
                            // 如果遇到第一个空行，则不再继续向后读取
                            break;
                        }
                        DataRow dataRow = table.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                dataRow[j] = row.GetCell(j).ToString() == "dd - MMM - yyyy" ? row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd") : row.GetCell(j).ToString();
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    ds.Tables.Add(table);
                }
            }
            excelFileStream.Close();
            workbook = null;
            return ds;
        }
        /// <summary>    
        /// 由Excel导入DataSet，如果有多个工作表，则导入多个DataTable    
        /// </summary>    
        /// <param name="excelFilePath">Excel文件路径，为物理路径。</param>    
        /// <param name="headerRowIndex">Excel表头行索引</param>    
        /// <returns>DataSet</returns>
        public static DataSet ImportDataSetFromExcel(string excelFilePath, int headerRowIndex)
        {
            using (FileStream stream = System.IO.File.OpenRead(excelFilePath))
            {
                return ImportDataSetFromExcel(stream, headerRowIndex);
            }
        }
        /// <summary>    
        /// 将Excel的列索引转换为列名，列索引从0开始，列名从A开始。如第0列为A，第1列为B...    
        /// </summary>    
        /// <param name="index">列索引</param>    
        /// <returns>列名，如第0列为A，第1列为B...</returns>
        public static string ConvertColumnIndexToColumnName(int index)
        {
            index = index + 1;
            int system = 26;
            char[] digArray = new char[100];
            int i = 0;
            while (index > 0)
            {
                int mod = index % system;
                if (mod == 0) mod = system;
                digArray[i++] = (char)(mod - 1 + 'A');
                index = (index - 1) / 26;
            }
            StringBuilder sb = new StringBuilder(i);
            for (int j = i - 1; j >= 0; j--)
            {
                sb.Append(digArray[j]);
            }
            return sb.ToString();
        }
        /// <summary>    
        /// 转化日期    
        /// </summary>    
        /// <param name="date">日期</param>    
        /// <returns></returns>
        public static DateTime ConvertDate(string date)
        {
            DateTime dt = new DateTime();
            string[] time = date.Split('-');
            int year = Convert.ToInt32(time[2]);
            int month = Convert.ToInt32(time[0]);
            int day = Convert.ToInt32(time[1]);
            string years = Convert.ToString(year);
            string months = Convert.ToString(month);
            string days = Convert.ToString(day);
            if (months.Length == 4)
            {
                dt = Convert.ToDateTime(date);
            }
            else
            {
                string rq = "";
                if (years.Length == 1)
                {
                    years = "0" + years;
                }
                if (months.Length == 1)
                {
                    months = "0" + months;
                }
                if (days.Length == 1)
                {
                    days = "0" + days;
                }
                rq = "20" + years + "-" + months + "-" + days;
                dt = Convert.ToDateTime(rq);
            }
            return dt;
        }
        #endregion;
    }

}
