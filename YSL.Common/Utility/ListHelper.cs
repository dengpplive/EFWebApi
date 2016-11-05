using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Reflection;
using YSL.Common.Extender;
using System.Linq;
namespace YSL.Common.Utility
{
    /// <summary>
    /// list helper
    /// </summary>
    public class ListHelper
    {
        public class Sorter
        {
            public delegate int Compare<T>(T value1, T value2);

            public static void DimidiateSort<T>(List<T> myList, Compare<T> myCompareMethod)
            {
                DimidiateSort<T>(myList, 0, myList.Count - 1, myCompareMethod);
            }
            public static void DimidiateSort<T>(List<T> myList, int left, int right, Compare<T> myCompareMethod)
            {
                if (left < right)
                {
                    T s = myList[(right + left) / 2];
                    int i = left - 1;
                    int j = right + 1;
                    T temp = default(T);
                    while (true)
                    {
                        do
                        {
                            i++;
                        }
                        while (i < right && myCompareMethod(myList[i], s) == -1);
                        do
                        {
                            j--;
                        }
                        while (j > left && myCompareMethod(myList[j], s) == 1);
                        if (i >= j)
                            break;
                        temp = myList[i];
                        myList[i] = myList[j];
                        myList[j] = temp;
                    }
                    DimidiateSort(myList, left, i - 1, myCompareMethod);
                    DimidiateSort(myList, j + 1, right, myCompareMethod);
                }
            }
        }

        interface ISort<T> where T : IComparable<T>
        {
            void Sort(T[] items);
        }

        public class Sort<T>
        {
            public static void Swap(T[] items, int left, int right)
            {
                T temp = items[right];
                items[right] = items[left];
                items[left] = temp;
            }
        }

        public static LTo ConvertListType<LFrom, From, LTo, To>(LFrom l)
            where LTo : IList<To>, new()
            where LFrom : IList<From>, new()
        {
            LTo ret = new LTo();
            foreach (From f in l)
            {
                To t = (To)Convert.ChangeType(f, typeof(To));
                ret.Add(t);
            }
            return ret;
        }

        public static List<To> ConvertListType<From, To>(List<From> f)
        {
            return ConvertListType<List<From>, From, List<To>, To>(f);
        }
        /// <summary>
        /// List转换DataTable
        /// </summary>
        /// <param name="ResList"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(IList ResList)
        {
            DataTable ret = new DataTable();
            if (ResList == null || ResList.Count == 0) return ret;
            System.Reflection.PropertyInfo[] p = ResList[0].GetType().GetProperties();
            List<System.Reflection.PropertyInfo> trim = new List<System.Reflection.PropertyInfo>();
            foreach (System.Reflection.PropertyInfo pi in p)
            {
                if (pi.Name.ToUpper().Equals("CONNINFO") || pi.Name.ToUpper().Equals("ITEM"))
                {
                    continue;
                }
                trim.Add(pi);
                ret.Columns.Add(pi.Name, System.Type.GetType(pi.PropertyType.ToString()));
            }
            p = trim.ToArray();
            for (int i = 0; i < ResList.Count; i++)
            {
                IList TempList = new ArrayList();
                foreach (System.Reflection.PropertyInfo pi in p)
                {
                    object oo = pi.GetValue(ResList[i], null);
                    TempList.Add(oo);
                }

                object[] itm = new object[p.Length];
                for (int j = 0; j < TempList.Count; j++)
                {
                    itm.SetValue(TempList[j], j);
                }
                ret.LoadDataRow(itm, true);
            }
            return ret;
        }

        /// <summary>
        /// DataTable转成Table格式的HTML字符串
        /// </summary>
        /// <param name="dt">表数据</param>
        /// <param name="notShow">第几行不显示</param>
        /// <returns></returns>
        public static string ConvertDataTableToHTML(DataTable dt, int[] notShow = null)
        {
            StringBuilder htmlTable = new StringBuilder();
            if (dt != null)
            {
                htmlTable.Append("<style>#container{width:1000px;text-align:left;}");
                htmlTable.Append("#uc_box{width:99%;margin:5px;border:1px solid #036;text-align:left;}");
                htmlTable.Append("#uc_forms{margin: 12px;font-size:12px;color:#036;width:100%;}</style>");
                htmlTable.Append("<div id='container'><div id='uc_box'><fieldset><table id='uc_forms'>");

                #region Table Header
                htmlTable.Append("<tr style='background-color:Azure'><td></td>");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (notShow == null)
                    {
                        htmlTable.Append(string.Format("<td>{0}</td>", dt.Columns[i].ColumnName));
                    }
                    else if (notShow.Where(o => o.Equals(i)).Count() == 0)
                        htmlTable.Append(string.Format("<td>{0}</td>", dt.Columns[i].ColumnName));
                }
                htmlTable.Append("</tr>");
                #endregion

                #region Table Data
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        htmlTable.Append("<tr style='background-color:GhostWhite;height:15px;'>");
                        htmlTable.Append(string.Format("<td>{0}</td>", i + 1));
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (notShow == null)
                            {
                                htmlTable.Append(string.Format("<td>{0}</td>", dt.Rows[i][j].ToString()));
                            }
                            else if (notShow.Where(o => o.Equals(j)).Count() == 0)
                                htmlTable.Append(string.Format("<td>{0}</td>", dt.Rows[i][j].ToString()));
                        }
                        htmlTable.Append("<tr>");
                    }
                }
                else
                    htmlTable.Append(string.Format("<tr style='background-color:GhostWhite'><td colspan='{0}'>No Data Found</td><tr>", dt.Columns.Count + 1 - notShow.Count()));

                #endregion

                htmlTable.Append("</table></fieldset></div></div>");
            }
            return htmlTable.ToString();
        }
    }

    /// <summary>
    /// IList排序类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListSort<T>
    {
        private string[] _propertyName;
        private bool[] _sortBy;
        private IList<T> _list;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">排序的Ilist</param>
        /// <param name="propertyName">排序字段属性名</param>
        /// <param name="sortBy">true升序 false 降序 不指定则为true</param>
        public ListSort(IList<T> list, string[] propertyName, bool[] sortBy)
        {
            _list = list;
            _propertyName = propertyName;
            _sortBy = sortBy;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">排序的Ilist</param>
        /// <param name="propertyName">排序字段属性名</param>
        public ListSort(IList<T> list, string[] propertyName)
        {
            _list = list;
            _propertyName = propertyName;
            for (int i = 0; i < _propertyName.Length; i++)
            {
                _sortBy[i] = true;
            }
        }

        /// <summary>
        /// IList
        /// </summary>
        public IList<T> List
        {
            get { return _list; }
            set { _list = value; }
        }

        /// <summary>
        /// 排序字段属性名
        /// </summary>
        public string[] PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        /// <summary>
        /// true升序 false 降序
        /// </summary>
        public bool[] SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        }

        /// <summary>
        /// 排序，插入排序方法
        /// </summary>
        /// <returns></returns>
        public IList<T> Sort()
        {
            if (_list.Count == 0) return _list;
            for (int i = 1; i < _list.Count; i++)
            {
                T t = _list[i];
                int j = i;
                while ((j > 0) && Compare(_list[j - 1], t) < 0)
                {
                    _list[j] = _list[j - 1];
                    --j;
                }
                _list[j] = t;
            }
            return _list;
        }

        /// <summary>
        /// 比较大小 返回值 小于零则X小于Y，等于零则X等于Y，大于零则X大于Y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int Compare(T x, T y)
        {
            int i = 0;
            //检查属性名
            for (i = 0; i < _propertyName.Length; ++i)
            {
                if (string.IsNullOrEmpty(_propertyName[i])) throw new ArgumentNullException("没有指定对象的排序字段属性名!");
            }

            //取属性的属性
            PropertyInfo[] property = new PropertyInfo[_propertyName.Length];
            for (i = 0; i < _propertyName.Length; ++i)
            {
                property[i] = typeof(T).GetProperty(_propertyName[i]);
                if (property[i] == null) throw new ArgumentNullException("在对象中没有找到指定属性!");
            }

            int compare = 0;
            for (i = 0; i < _propertyName.Length; ++i)
            {
                compare = CompareOne(x, y, property[i], _sortBy[i]);
                if (compare != 0) return compare;
            }
            return compare;
        }

        private int CompareOne(T x, T y, PropertyInfo property, bool sortBy)
        {
            switch (property.PropertyType.ToString())
            {
                case "System.Int32":
                    int int1 = 0;
                    int int2 = 0;
                    if (property.GetValue(x, null) != null)
                    {
                        int1 = Convert.ToInt32(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        int2 = Convert.ToInt32(property.GetValue(y, null));
                    }
                    if (sortBy)
                    {
                        return int2.CompareTo(int1);
                    }
                    return int1.CompareTo(int2);
                case "System.Decimal":
                    decimal decimal1 = 0;
                    decimal decimal2 = 0;
                    if (property.GetValue(x, null) != null)
                    {
                        decimal1 = Convert.ToDecimal(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        decimal2 = Convert.ToDecimal(property.GetValue(y, null));
                    }
                    if (sortBy)
                    {
                        return decimal2.CompareTo(decimal1);
                    }
                    return decimal1.CompareTo(decimal2);
                case "System.Double":
                    double double1 = 0;
                    double double2 = 0;
                    if (property.GetValue(x, null) != null)
                    {
                        double1 = Convert.ToDouble(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        double2 = Convert.ToDouble(property.GetValue(y, null));
                    }
                    if (sortBy)
                    {
                        return double2.CompareTo(double1);
                    }
                    return double1.CompareTo(double2);
                case "System.String":
                    string string1 = string.Empty;
                    string string2 = string.Empty;
                    if (property.GetValue(x, null) != null)
                    {
                        string1 = property.GetValue(x, null).ToString();
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        string2 = property.GetValue(y, null).ToString();
                    }
                    if (sortBy)
                    {
                        return String.Compare(string2, string1, StringComparison.Ordinal);
                    }
                    return String.Compare(string1, string2, StringComparison.Ordinal);
                case "System.DateTime":
                    DateTime dateTime1 = DateTime.Now;
                    DateTime dateTime2 = DateTime.Now;
                    if (property.GetValue(x, null) != null)
                    {
                        dateTime1 = Convert.ToDateTime(property.GetValue(x, null));
                    }
                    if (property.GetValue(y, null) != null)
                    {
                        dateTime2 = Convert.ToDateTime(property.GetValue(y, null));
                    }
                    if (sortBy)
                    {
                        return dateTime2.CompareTo(dateTime1);
                    }
                    return dateTime1.CompareTo(dateTime2);
            }
            return 0;
        }
    }

    public class ConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable转换为list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertToList(DataTable dt)
        {
            // 定义集合    
            IList<T> ts = new List<T>();

            if (dt == null || dt.Rows.Count <= 0)
            {
                return ts;
            }

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(t, value, null);
                            }
                            else if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(t, value.ToString().ToInt(), null);
                            }
                            else if (pi.PropertyType == typeof(DateTime))
                            {
                                pi.SetValue(t, value.ToString().ToDateTime(System.DateTime.Now), null);
                            }
                            else if (pi.PropertyType == typeof(float) || pi.PropertyType == typeof(double))
                            {
                                pi.SetValue(t, value.ToString().ToDouble(0), null);
                            }
                            else if (pi.PropertyType == typeof(decimal))
                            {
                                pi.SetValue(t, value.ToString().ToDecimal(), null);
                            }
                            else if (pi.PropertyType == typeof(long))
                            {
                                pi.SetValue(t, (long)value, null);
                            }
                            else if (pi.PropertyType == typeof(SByte))
                            {
                                pi.SetValue(t, (SByte)value, null);
                            }
                        }

                    }
                }
                ts.Add(t);
            }
            return ts;
        }

    }
}
