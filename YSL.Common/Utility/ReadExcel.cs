using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace YSL.Common.Utility
{
   /// <summary>
   /// 读写Excel工具类
   /// </summary>
    public class ReadExcel : IDisposable
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        public const string UID = "Excel.Application";
        object oExcel;
        object WorkBooks, WorkBook, WorkSheets, WorkSheet, Range, Interior;
        object objPt;
        public IntPtr pt { get; set; }

        /// <summary>
        /// 类的构造函数
        /// </summary>
        public ReadExcel()
        {
            oExcel = Activator.CreateInstance(Type.GetTypeFromProgID(UID));
            objPt = oExcel.GetType().InvokeMember("Hwnd", BindingFlags.GetProperty, null, oExcel, null);
            pt = new IntPtr((Int32)objPt);
        }

        /// <summary>
        /// excel的可视性
        /// </summary>
        public bool Visible
        {
            set
            {
                if (false == value)
                {
                    oExcel.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, oExcel, new object[] { false });
                }
                else
                {
                    oExcel.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, oExcel, new object[] { true });
                }
            }
        }

        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="name">Excel文件名</param>
        public void OpenDocument(string name)
        {
            WorkBooks = oExcel.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, oExcel, null);
            WorkBook = WorkBooks.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, WorkBooks, new object[] { name, true });
            WorkSheets = WorkBook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, WorkBook, null);
            WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { 1 });
            //Range = WorkSheet.GetType().InvokeMember("Range",BindingFl-ags.GetProperty,null,WorkSheet,new object[1] { "A1" });
        }

        /// <summary>
        /// 创建一个新文件 
        /// </summary>
        public void NewDocument()
        {
            WorkBooks = oExcel.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, oExcel, null);
            WorkBook = WorkBooks.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, WorkBooks, null);
            WorkSheets = WorkBook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, WorkBook, null);
            WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { 1 });
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[1] { "A1" });
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void CloseDocument()
        {
            if (WorkBook != null)
            {
                WorkBook.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, WorkBook, new object[] { true });
            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="name">Excel文件名</param>
        public void SaveDocument(string name)
        {
            if (File.Exists(name))
            {
                WorkBook.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null, WorkBook, null);
            }
            else
            {
                WorkBook.GetType().InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, WorkBook, new object[] { name });
            }
        }

        /// <summary>
        /// 设置单元格的背景颜色
        /// </summary>
        /// <param name="range"></param>
        /// <param name="r"></param>
        public void SetColor(string range, int r)
        {
            //Range.Interior.ColorIndex
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            Interior = Range.GetType().InvokeMember("Interior", BindingFlags.GetProperty, null, Range, null);
            Range.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, Interior, new object[] { r });
        }

        /// <summary>
        /// 页面方向
        /// </summary>
        public enum XlPageOrientation
        {
            xlPortrait = 1, //纵向
            xlLandscape = 2 //横向
        }

        /// <summary>
        /// 设置页面方向
        /// </summary>
        /// <param name="Orientation"></param>
        public void SetOrientation(XlPageOrientation Orientation)
        {
            object PageSetup = WorkSheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, WorkSheet, null);
            PageSetup.GetType().InvokeMember("Orientation", BindingFlags.SetProperty, null, PageSetup, new object[] { 2 });
        }

        /// <summary>
        /// 设置页边距的大小表
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Top"></param>
        /// <param name="Bottom"></param>
        public void SetMargin(double Left, double Right, double Top, double Bottom)
        {
            object PageSetup = WorkSheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, WorkSheet, null);
            PageSetup.GetType().InvokeMember("LeftMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Left }); //Range.PageSetup.LeftMargin
            PageSetup.GetType().InvokeMember("RightMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Right });//Range.PageSetup.RightMargin
            PageSetup.GetType().InvokeMember("TopMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Top });  //Range.PageSetup.TopMargin
            PageSetup.GetType().InvokeMember("BottomMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Bottom });//Range.PageSetup.BottomMargin
        }

        /// <summary>
        /// 页尺寸
        /// </summary>
        public enum xlPaperSize
        {
            xlPaperA4 = 9,
            xlPaperA4Small = 10,
            xlPaperA5 = 11,
            xlPaperLetter = 1,
            xlPaperLetterSmall = 2,
            xlPaper10x14 = 16,
            xlPaper11x17 = 17,
            xlPaperA3 = 9,
            xlPaperB4 = 12,
            xlPaperB5 = 13,
            xlPaperExecutive = 7,
            xlPaperFolio = 14,
            xlPaperLedger = 4,
            xlPaperLegal = 5,
            xlPaperNote = 18,
            xlPaperQuarto = 15,
            xlPaperStatement = 6,
            xlPaperTabloid = 3
        }

        /// <summary>
        /// 设置工作表的大小
        /// </summary>
        /// <param name="Size"></param>
        public void SetPaperSize(xlPaperSize Size)
        {
            //Range.PageSetup.PaperSize(纸张尺寸)
            object PageSetup = WorkSheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, WorkSheet, null);
            PageSetup.GetType().InvokeMember("PaperSize", BindingFlags.SetProperty, null, PageSetup, new object[] { Size });
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="Percent"></param>
        public void SetZoom(int Percent)
        {
            //Range.PageSetup.Zoom(打印的范围)
            object PageSetup = WorkSheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, WorkSheet, null);
            PageSetup.GetType().InvokeMember("Zoom", BindingFlags.SetProperty, null, PageSetup, new object[] { Percent });
        }

        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="n"></param>
        /// <param name="Name"></param>
        public void ReNamePage(int n, string Name)
        {
            //Range.Interior.ColorIndex
            object Page = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { n });
            Page.GetType().InvokeMember("Name", BindingFlags.SetProperty, null, Page, new object[] { Name });
        }

        /// <summary>
        /// 添加一个工作表
        /// </summary>
        /// <param name="Name">页面标题</param>
        public void AddNewSheet(string Name)
        {
            //Worksheet.Add.Item
            WorkSheet = WorkSheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, null, WorkSheets, null);
            object Page = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { 1 });
            Page.GetType().InvokeMember("Name", BindingFlags.SetProperty, null, Page, new object[] { Name });
        }

        /// <summary>
        /// 设置单元格的字体
        /// </summary>
        /// <param name="range"></param>
        /// <param name="font"></param>
        public void SetFont(string range, Font font)
        {
            //Range.Font.Name
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object Font = Range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, Range, null);
            Range.GetType().InvokeMember("Name", BindingFlags.SetProperty, null, Font, new object[] { font.Name });
            Range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, Font, new object[] { font.Size });
        }

        /// <summary>
        /// 写在单元格的值
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        public void SetValue(string range, string value)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            Range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, Range, new object[] { value });
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="MergeCells"></param>
        public void SetMerge(bool MergeCells)//string range, bool MergeCells)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { "A1", "G1" });
            Range.GetType().InvokeMember("MergeCells", BindingFlags.SetProperty, null, Range, new object[] { MergeCells });
        }

        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Width"></param>
        public void SetColumnWidth(string range, double Width)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Width };
            Range.GetType().InvokeMember("ColumnWidth", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 设置文字方向
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Orientation"></param>
        public void SetTextOrientation(string range, int Orientation)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Orientation };
            Range.GetType().InvokeMember("Orientation", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 单元格中的文本垂直
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Alignment"></param>
        public void SetVerticalAlignment(string range, int Alignment)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Alignment };
            Range.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 单元格中的文本对齐水平
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Alignment"></param>
        public void SetHorisontalAlignment(string range, int Alignment)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Alignment };
            Range.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 格式化单元格中的指定文本
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Start"></param>
        /// <param name="Length"></param>
        /// <param name="Color"></param>
        /// <param name="FontStyle"></param>
        /// <param name="FontSize"></param>
        public void SelectText(string range, int Start, int Length, int Color, string FontStyle, int FontSize)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Start, Length };
            object Characters = Range.GetType().InvokeMember("Characters", BindingFlags.GetProperty, null, Range, args);
            object Font = Characters.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, Characters, null);
            Font.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, Font, new object[] { Color });
            Font.GetType().InvokeMember("FontStyle", BindingFlags.SetProperty, null, Font, new object[] { FontStyle });
            Font.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, Font, new object[] { FontSize });
        }

        /// <summary>
        /// 文字换行
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Value"></param>
        public void SetWrapText(string range, bool Value)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Value };
            Range.GetType().InvokeMember("WrapText", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Height"></param>
        public void SetRowHeight(string range, double Height)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { Height };
            Range.GetType().InvokeMember("RowHeight", BindingFlags.SetProperty, null, Range, args);
        }

        /// <summary>
        /// 设置边界
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Style"></param>
        public void SetBorderStyle(string range, int Style)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            object[] args = new object[] { 1 };
            object[] args1 = new object[] { 1 };
            object Borders = Range.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, Range, null);
            Borders = Range.GetType().InvokeMember("LineStyle", BindingFlags.SetProperty, null, Borders, args);
        }

        /// <summary>
        /// 将源区域的内容剪贴到目标单元格为起始点的相同大小的区域中
        /// </summary>
        /// <param name="sourceRange">源区域</param>
        /// <param name="targetCell">目标单元格</param>
        public void CutRange(string sourceRange, string targetCell)
        {
            //源区域的Range对象
            object source = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { sourceRange });

            //目标单元格的Range对象
            object target = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { targetCell });

            //执行剪贴(类似的VBA语句：Range("A1:A10").Cut Range("C10"))
            source.GetType().InvokeMember("Cut", BindingFlags.InvokeMethod, null, source, new object[] { target });
        }

        //Ctrl+ * 可以获得当前sheet的最大的行与列
        /// 以A1:C3为基础选择,Selection.CurrentRegion.Select
        /// <summary>
        /// 取得指定工作表的内容的最大行数
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns></returns>
        public int? getMaxRows(string sheetName)
        {
            int? rtn = null;
            try
            {
                WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });

                object range1 = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[1] { "A1:C3" });
                object range = range1.GetType().InvokeMember("CurrentRegion", BindingFlags.GetProperty, null, range1, null);

                //object range = WorkSheet.GetType().InvokeMember("UsedRange", BindingFlags.GetProperty, null, WorkSheet, null);
                object rows = range.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, range, null);
                rtn = (int)rows.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, rows, null);
                return rtn;
            }
            catch (Exception)
            {
                return rtn;
            }
        }

        /// <summary>
        /// 取第一个工作表的内容的最大行数
        /// </summary>
        /// <returns></returns>
        public int? getMaxRows()
        {
            int? rtn = null;
            try
            {
                object range1 = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[1] { "A1:C3" });
                object range = range1.GetType().InvokeMember("CurrentRegion", BindingFlags.GetProperty, null, range1, null);
                //object range = WorkSheet.GetType().InvokeMember("UsedRange", BindingFlags.GetProperty, null, WorkSheet, null);
                object rows = range.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, range, null);
                rtn = (int)rows.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, rows, null);
                return rtn;
            }
            catch (Exception)
            {
                return rtn;
            }
        }

        /// <summary>
        /// 取得指定工作表的内容的最大列数
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns></returns>
        public int? getMaxColumns(string sheetName)
        {
            int? rtn = null;
            try
            {
                WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });

                object range1 = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[1] { "A1:C3" });
                object range = range1.GetType().InvokeMember("CurrentRegion", BindingFlags.GetProperty, null, range1, null);

                //WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });
                //object range = WorkSheet.GetType().InvokeMember("UsedRange", BindingFlags.GetProperty, null, WorkSheet, null);
                object Columns = range.GetType().InvokeMember("Columns", BindingFlags.GetProperty, null, range, null);
                rtn = (int)Columns.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, Columns, null);
                return rtn;
            }
            catch (Exception)
            {
                return rtn;
            }
        }

        /// <summary>
        /// 取第一个工作表的内容的最大列数
        /// </summary>
        /// <returns></returns>
        public int? getMaxColumns()
        {
            int? rtn = null;
            try
            {
                object range1 = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[1] { "A1:C3" });
                object range = range1.GetType().InvokeMember("CurrentRegion", BindingFlags.GetProperty, null, range1, null);
                //object range = WorkSheet.GetType().InvokeMember("UsedRange", BindingFlags.GetProperty, null, WorkSheet, null);
                object Columns = range.GetType().InvokeMember("Columns", BindingFlags.GetProperty, null, range, null);
                rtn = (int)Columns.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, Columns, null);
                return rtn;
            }
            catch (Exception)
            {
                return rtn;
            }
        }

        /// <summary>
        /// 数字转换为Excel字母数字的列
        /// </summary>
        /// <param name="columnNum">数字</param>
        /// <returns>返回字符串</returns>
        public string ConvertColumnNum2String(int? columnNum)
        {
            if (columnNum > 26)
            {
                return string.Format("{0}{1}", (char)(((columnNum - 1) / 26) + 64), (char)(((columnNum - 1) % 26) + 65));
            }
            else
            {
                return ((char)(columnNum + 64)).ToString();
            }
        }
        /// <summary>
        /// Excel字母形式的列转换为数字
        /// </summary>
        /// <param name="letters">字符</param>
        /// <returns>返回的int值</returns>
        public int ConvertLetters2ColumnName(string letters)
        {
            int num = 0;
            if (letters.Length == 1)
            {
                num = Convert.ToInt32(letters[0]) - 64;
            }
            else if (letters.Length == 2)
            {
                num = (Convert.ToInt32(letters[0]) - 64) * 26 + Convert.ToInt32(letters[1]) - 64;
            }
            return num;
        }

        /// <summary>
        /// 取得指定工作表的内容的区域范围
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns></returns>
        public string GetSheetRange(string sheetName)
        {
            int? rows = getMaxRows(sheetName);
            int? cols = getMaxColumns(sheetName);
            object[] Parameters = new Object[2];
            Parameters[0] = "A1";
            Parameters[1] = ConvertColumnNum2String(cols) + rows.ToString();
            return Parameters[0] + ":" + Parameters[1];
        }

        /// <summary>
        /// 取得第一个工作表的内容的区域范围
        /// </summary>
        /// <returns></returns>
        public string GetSheetRange()
        {
            int? rows = getMaxRows();
            int? cols = getMaxColumns();
            object[] Parameters = new Object[2];
            Parameters[0] = "A1";
            Parameters[1] = ConvertColumnNum2String(cols) + rows.ToString();
            return Parameters[0] + ":" + Parameters[1];
        }

        /// <summary>
        /// 把指定工作表的有效区域的数据转换成一个二维数组
        /// </summary>
        /// <param name="sheetName">工作表名</param>
        /// <returns></returns>
        public Object[,] getSheetMaxRangeValues(string sheetName)
        {
            string range = GetSheetRange(sheetName);
            return getValues(sheetName, range);
        }

        /// <summary>
        /// 把第一个工作表的有效区域的数据转换成一个二维数组
        /// </summary>
        /// <returns></returns>
        public Object[,] getSheetMaxRangeValues()
        {
            string range = GetSheetRange();
            return getValues(range);
        }

        /// <summary>
        /// 取当前工作表中指定单元格的值
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public string GetValue(string range)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
            var rtn = Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null);
            return rtn == null ? "" : rtn.ToString();
        }



        /// <summary>
        /// 读取第一个默认sheet数据到二维数组
        /// </summary>
        /// <param name="range">读取的区域，如"D1:F15"</param>
        /// <returns>一个二维数组(例如：Object[,] val = excel.getValues("D1:D18");)</returns>
        public Object[,] getValues(string range)
        {
            Object[,] rtnValue;
            try
            {
                Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });

                if (range == "A1:A1")
                {
                    //定义一个只包含一个[1,1]元素的二维数组,数组下标从1开始
                    Array array = Array.CreateInstance(typeof(Object), new int[] { 1, 1 }, new int[] { 1, 1 });

                    //把range中的唯一元素赋值给数组
                    array.SetValue(Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null), 1, 1);
                    rtnValue = (Object[,])array; //把Array类型的二维数组转换成Object类型后赋值rtnValue
                }
                else
                {
                    rtnValue = (Object[,])Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null);
                }

                return rtnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得Excel指定WorkSheet中指定单元格的值
        /// </summary>
        /// <param name="sheetName">WorkSheet的名字</param>
        /// <param name="range">当前工作表中的某个单元格的值</param>
        /// <returns></returns>
        public string GetValue(string sheetName, string range)
        {
            try
            {
                WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });
                Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });
                var rtn = Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null);
                return rtn == null ? "" : rtn.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 读取数据到二维数组
        /// </summary>
        /// <param name="sheetName">WorkSheet名</param>
        /// <param name="range">读取的区域，如"D1:F15"</param>
        /// <returns>一个二维数组</returns>
        /// 例如：Object[,] val = excel.getValues("Sheet1", "D1:D18");
        public Object[,] getValues(string sheetName, string range)
        {
            Object[,] rtnValue;

            try
            {
                WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });
                Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { range });

                if (range == "A1:A1")
                {
                    //定义一个只包含一个[1,1]元素的二维数组,数组下标从1开始
                    Array array = Array.CreateInstance(typeof(Object), new int[] { 1, 1 }, new int[] { 1, 1 });

                    //把range中的唯一元素赋值给数组
                    array.SetValue(Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null), 1, 1);
                    rtnValue = (Object[,])array; //把Array类型的二维数组转换成Object类型后赋值rtnValue
                }
                else
                {
                    rtnValue = (Object[,])Range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, Range, null);
                }

                return rtnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 得到Excel文件中指定列的最大行数
        /// </summary>
        /// <param name="columnName">列标号(如A、B、C等)</param>
        /// <returns>返回columnName列的最大行数</returns>
        public int GetMaxRowNumber(string columnName)
        {
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { columnName + "65535" });
            Range = Range.GetType().InvokeMember("End", BindingFlags.GetProperty, null, Range, new object[] { -4162 });
            int RowMax = Convert.ToInt32(Range.GetType().InvokeMember("Row", BindingFlags.GetProperty, null, Range, null));
            return RowMax;
        }

        /// <summary>
        /// 得到Excel文件中指定列的最大行数
        /// </summary>
        /// <param name="sheetName">WorkSheet的名字</param>
        /// <param name="columnName">列标号(如A、B、C等)</param>
        /// <returns>返回columnName列的最大行数</returns>
        public int GetMaxRowNumber(string sheetName, string columnName)
        {
            WorkSheet = WorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, WorkSheets, new object[] { sheetName });
            Range = WorkSheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, WorkSheet, new object[] { columnName + "65535" });
            Range = Range.GetType().InvokeMember("End", BindingFlags.GetProperty, null, Range, new object[] { -4162 });
            int RowMax = Convert.ToInt32(Range.GetType().InvokeMember("Row", BindingFlags.GetProperty, null, Range, null));
            return RowMax;
        }

        //释放EXCEL
        public void Dispose()
        {
            NAR(oExcel);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="o"></param>
        private void NAR(object o)
        {
            try
            {
                WorkBooks = null;
                WorkBook = null;
                WorkSheets = null;
                WorkSheet = null;
                Range = null;
                Interior = null;
                oExcel = null;
                o = null;
                GC.Collect();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }
            catch { }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 释放资料
        /// </summary>
        /// <param name="pt"></param>
        public static void release(IntPtr pt)
        {
            if (pt != null)
            {
                int pid = 0;
                GetWindowThreadProcessId(pt, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                p.Kill();
            }
        }
    }
}
