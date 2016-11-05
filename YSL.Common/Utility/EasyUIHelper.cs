using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Text;

namespace YSL.Common.Utility
{
    public class EasyUIHelper
    {
        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string StringToJson(String s)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 将DataTable装换成Json格式
        /// <summary>
        /// 将DataTable装换成Json格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DtToJson(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "[]";
            }
            else
            {
                string strEncode;
                StringBuilder retVal = new StringBuilder();
                retVal.Append("[");
                int RowCount = 0, ColumnCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    RowCount++;
                    retVal.Append("{");
                    foreach (DataColumn column in dt.Columns)
                    {
                        ColumnCount++;
                        strEncode = StringToJson(row[column].ToString());
                        retVal.AppendFormat("\"{0}\":\"{1}\"{2}", column.ColumnName, strEncode, ColumnCount == dt.Columns.Count ? "" : ",");
                    }
                    ColumnCount = 0;
                    retVal.Append("}");
                    retVal.AppendFormat("{0}", RowCount == dt.Rows.Count ? "" : ",");
                }
                retVal.Append("]");
                return retVal.ToString();
            }
        }
        #endregion

        #region 将DataSet装换成Json格式
        /// <summary>
        /// 将DataSet装换成Json格式
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DsToJson(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return "[]";
            }
            else
            {
                StringBuilder json = new StringBuilder();
                foreach (DataTable dt in ds.Tables)
                {
                    json.Append(",{\"");
                    json.Append(dt.TableName);
                    json.Append("\":");
                    json.Append(DtToJson(dt));
                    json.Append("}");
                }
                return json.ToString().Substring(1);
            }
        }
        #endregion

        #region 返回EasyUI Datagrid的基础数据
        /// <summary>
        /// 返回EasyUI Datagrid的基础数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatagridToJson(DataSet ds)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                str.Append("" + dt.Rows.Count.ToString() + ",");
                str.Append("\"rows\":");
                str.Append(DtToJson(dt));
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion
        #region 返回EasyUI Datagrid的基础数据
        /// <summary>
        /// 返回EasyUI Datagrid的基础数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatagridToJson(DataTable dt)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (dt != null && dt.Rows.Count > 0)
            {
                str.Append("" + dt.Rows.Count.ToString() + ",");
                str.Append("\"rows\":");
                str.Append(DtToJson(dt));
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion
        #region 返回EasyUI Datagrid的异步分页数据
        /// <summary>
        /// 返回EasyUI Datagrid的异步分页数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatagridPagerToJson(DataSet ds)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                str.Append("" + ds.Tables[1].Rows[0][0].ToString() + ",");
                str.Append("\"rows\":");
                str.Append(DtToJson(ds.Tables[0]));
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion

        #region 返回EasyUI Datagrid的有页脚基础数据
        /// <summary>
        /// 返回EasyUI Datagrid的有页脚基础数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatagridFooterToJson(DataSet ds)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                str.Append("" + dt.Rows.Count.ToString() + ",");
                str.Append("\"rows\":");
                str.Append(DtToJson(dt));
                if (ds.Tables[1].Rows.Count > 0)
                {
                    str.Append(",");
                    str.Append("\"footer\":");
                    str.Append(DtToJson(ds.Tables[1]));
                }
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("],");
                str.Append("\"footer\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion

        #region 返回EasyUI Datagrid的有页脚异步分页数据
        /// <summary>
        /// 返回EasyUI Datagrid的有页脚异步分页数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DatagridPagerFooterToJson(DataSet ds)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                str.Append("" + ds.Tables[1].Rows[0][0].ToString() + ",");
                str.Append("\"rows\":");
                str.Append(DtToJson(ds.Tables[0]));
                if (ds.Tables[2].Rows.Count > 0)
                {
                    str.Append(",");
                    str.Append("\"footer\":");
                    str.Append(DtToJson(ds.Tables[2]));
                }
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("],");
                str.Append("\"footer\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion

        #region 返回EasyUI Treegrid的基础数据(返回数据源必须有id,name,pid字段)
        /// <summary>
        /// 返回EasyUI Treegrid的基础数据(返回数据源必须有id,name,pid字段)(第一级数据父ID为0) 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string TreegridToJson(DataSet ds)
        {
            return GetTreegridJson(ds, true, 0);
        }
        /// <summary>
        /// 返回EasyUI Treegrid的基础数据(返回数据源必须有id,name,pid字段,expand是否展开所有数据)(第一级数据父ID为0) 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="expand"></param>
        /// <returns></returns>
        public static string TreegridToJson(DataSet ds, bool expand)
        {
            return GetTreegridJson(ds, expand, 0);
        }
        /// <summary>
        /// 返回EasyUI Treegrid的基础数据(返回数据源必须有id,name,pid,level字段)(第一级数据父ID为0) 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="expand"></param>
        /// <returns></returns>
        public static string TreegridToJson(DataSet ds, int level)
        {
            return GetTreegridJson(ds, false, level);
        }
        private static string GetTreegridJson(DataSet ds, bool expand, int level)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataView dv = new DataView();
                DataTable dt = ds.Tables[0];
                dv.Table = dt;
                str.Append("" + dt.Rows.Count.ToString() + ",");
                str.Append("\"rows\":");
                //生成Treegrid格式数据
                str.Append("[");
                int count = dt.Rows.Count;
                string strEncode;
                for (int i = 0; i < count; i++)
                {
                    str.Append("{");
                    str.Append("\"id\":");
                    str.Append("" + dt.Rows[i]["id"].ToString() + ",");
                    str.Append("\"name\":");
                    str.Append("\"" + StringToJson(dt.Rows[i]["name"].ToString()) + "\",");
                    str.Append("\"_parentId\":");
                    str.Append("" + dt.Rows[i]["pid"].ToString() + ",");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Columns[j].ColumnName != "id" && dt.Columns[j].ColumnName != "name" && dt.Columns[j].ColumnName != "pid")
                        {
                            str.Append("\"" + dt.Columns[j].ColumnName + "\":");
                            strEncode = StringToJson(dt.Rows[i][j].ToString());
                            str.Append("\"" + strEncode + "\",");
                        }
                    }
                    str.Append("\"state\":");
                    if (expand)
                    {
                        str.Append("\"open\"");
                    }
                    else
                    {
                        dv.RowFilter = string.Format("pid = {0}", dt.Rows[i]["id"].ToString());
                        int curLevel = 1;
                        if (level > 0)
                        {
                            curLevel = int.Parse(dt.Rows[i]["level"].ToString());
                        }
                        if (dv.Count > 0 && curLevel >= level)
                        {
                            str.Append("\"closed\"");
                        }
                        else
                        {
                            str.Append("\"open\"");
                        }
                    }
                    if (i == count - 1)
                    {
                        str.Append("}");
                    }
                    else
                    {
                        str.Append("},");
                    }
                }
                str.Append("]");
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion

        #region 返回EasyUI Treegrid的异步数据(返回数据源必须有id,name,pid字段)
        /// <summary>
        /// 返回EasyUI Treegrid的异步数据(返回数据源必须有id,name,pid字段)(第一级数据父ID为0) 
        /// </summary>
        /// <param name="ds">table[0] table[1] 数据源DataSet ds = bll.GetList(pid,rootid);</param>
        /// <returns></returns>
        public static string TreegridToAsyJson(DataSet ds)
        {
            return GetTreegridAsyJson(ds, 0);
        }
        /// <summary>
        /// 返回EasyUI Treegrid的异步数据(返回数据源必须有id,name,pid,level字段)(第一级数据父ID为0) 
        /// </summary>
        /// <param name="ds">table[0] table[1]</param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string TreegridToAsyJson(DataSet ds, int level)
        {
            return GetTreegridAsyJson(ds, level);
        }

        private static string GetTreegridAsyJson(DataSet ds, int level)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("{");
            str.Append("\"total\":");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                DataTable dtAll = ds.Tables[1];
                DataView dv = new DataView();
                dv.Table = dtAll;
                str.Append("" + dt.Rows.Count.ToString() + ",");
                str.Append("\"rows\":");
                //生成Treegrid格式数据
                str.Append("[");
                int count = dt.Rows.Count;
                string strEncode;
                for (int i = 0; i < count; i++)
                {
                    str.Append("{");
                    str.Append("\"id\":");
                    str.Append("" + dt.Rows[i]["id"].ToString() + ",");
                    str.Append("\"name\":");
                    str.Append("\"" + StringToJson(dt.Rows[i]["name"].ToString()) + "\",");
                    str.Append("\"_parentId\":");
                    str.Append("" + dt.Rows[i]["pid"].ToString() + ",");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Columns[j].ColumnName != "id" && dt.Columns[j].ColumnName != "name" && dt.Columns[j].ColumnName != "pid")
                        {
                            str.Append("\"" + dt.Columns[j].ColumnName + "\":");
                            strEncode = StringToJson(dt.Rows[i][j].ToString());
                            str.Append("\"" + strEncode + "\",");
                        }
                    }
                    str.Append("\"state\":");
                    dv.RowFilter = string.Format("pid = {0}", dt.Rows[i]["id"].ToString());
                    int curLevel = 1;
                    if (level > 0)
                    {
                        curLevel = int.Parse(dt.Rows[i]["level"].ToString());
                    }
                    if (dv.Count > 0 && curLevel >= level)
                    {
                        str.Append("\"closed\"");
                    }
                    else
                    {
                        str.Append("\"open\"");
                    }

                    if (i == count - 1)
                    {
                        str.Append("}");
                    }
                    else
                    {
                        str.Append("},");
                    }
                }
                str.Append("]");
                str.Append("}");
            }
            else
            {
                str.Append("0,");
                str.Append("\"rows\":[");
                str.Append("]}");
            }
            return str.ToString();
        }
        #endregion

        #region 返回EasyUI Tree的基础数据(返回数据源必须有id,name,pid字段)
        /// <summary>
        /// 返回Tree基础数据,不展开
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public static string TreeToJson(DataSet ds, string parentid)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //清空数据
                strTree.Clear();
                allData = false;
                allExpand = false;
                GetTreeJson(ds.Tables[0], parentid);
                return strTree.ToString();
            }
            else
            {
                return "[]";
            }
        }

        /// <summary>
        /// 返回Tree数据，默认type:00 基础数据不展开 01:全展开 10:所有数据不展开 11:所有数据展开
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TreeToJson(DataSet ds, string parentid, string type)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                switch (type)
                {
                    case "10":
                        allData = true;
                        allExpand = false;
                        break;
                    case "01":
                        allData = false;
                        allExpand = true;
                        break;
                    case "11":
                        allData = true;
                        allExpand = true;
                        break;
                    default:
                        allData = false;
                        allExpand = false;
                        break;
                }
                //清空数据
                strTree.Clear();
                GetTreeJson(ds.Tables[0], parentid);
                return strTree.ToString();
            }
            else
            {
                return "[]";
            }
        }

        #region EasyUI Tree的递归数据
        //输出数据
        private static System.Text.StringBuilder strTree = new System.Text.StringBuilder();
        //是否读取数据源除id,name外数据
        private static bool allData = false;
        //是否全部展开
        private static bool allExpand = false;
        private static void GetTreeJson(DataTable dt, string parentid)
        {
            DataView dv = new DataView();
            DataView dv1 = new DataView();
            dv.Table = dt;
            dv1.Table = dt;
            dv.RowFilter = string.Format("pid = {0}", parentid);
            if (dv.Count > 0)
            {
                strTree.Append("[");
                DataRowView drv = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drv = dv[i];
                    strTree.Append("{");
                    strTree.Append("\"id\":");
                    strTree.Append("" + drv["id"].ToString() + ",");
                    strTree.Append("\"text\":");
                    strTree.Append("\"" + StringToJson(drv["name"].ToString()) + "\",");
                    //添加除id,name其他数据
                    if (allData)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Columns[j].ColumnName != "id" && dt.Columns[j].ColumnName != "name")
                            {
                                strTree.Append("\"" + dt.Columns[j].ColumnName + "\":");
                                strTree.Append("\"" + StringToJson(drv[j].ToString()) + "\",");
                            }
                        }
                    }
                    strTree.Append("\"state\":");
                    dv1.RowFilter = string.Format("pid = {0}", drv["id"].ToString());
                    if (dv1.Count > 0)
                    {
                        if (allExpand)
                        {
                            strTree.Append("\"open\"");
                        }
                        else
                        {
                            strTree.Append("\"closed\"");
                        }
                        strTree.Append(",\"children\":");
                        GetTreeJson(dt, drv["id"].ToString());
                    }
                    else
                    {
                        strTree.Append("\"open\"");
                    }
                    if (i == dv.Count - 1)
                    {
                        strTree.Append("}");
                    }
                    else
                    {
                        strTree.Append("},");
                    }
                }
                strTree.Append("]");
            }
        }
        #endregion
        #endregion

        #region 返回EasyUI Tree的异步数据(返回数据源必须有id,name,pid字段)
        /// <summary>
        /// 返回EasyUI Tree的异步数据(返回数据源必须有id,name,pid字段)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public static string TreeToAsyJson(DataSet ds, string parentid)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return GetTreeAsyJson(ds, parentid, false);
            }
            else
            {
                return "[]";
            }
        }

        /// <summary>
        /// 返回EasyUI Tree的异步数据(返回数据源必须有id,name,pid字段)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="parentid"></param>
        /// <param name="allAsyData"></param>
        /// <returns></returns>
        public static string TreeToAsyJson(DataSet ds, string parentid, bool allAsyData)
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return GetTreeAsyJson(ds, parentid, allAsyData);
            }
            else
            {
                return "[]";
            }
        }
        private static string GetTreeAsyJson(DataSet ds, string parentid, bool allAsyData)
        {
            System.Text.StringBuilder strAsyTree = new System.Text.StringBuilder();
            DataView dv = new DataView();
            DataView dv1 = new DataView();
            DataTable dt = ds.Tables[0];
            DataTable dtAll = ds.Tables[1];
            dv.Table = dt;
            dv1.Table = dtAll;
            dv.RowFilter = string.Format("pid = {0}", parentid);
            if (dv.Count > 0)
            {
                strAsyTree.Append("[");
                DataRowView drv = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drv = dv[i];
                    strAsyTree.Append("{");
                    strAsyTree.Append("\"id\":");
                    strAsyTree.Append("" + drv["id"].ToString() + ",");
                    strAsyTree.Append("\"text\":");
                    strAsyTree.Append("\"" + StringToJson(drv["name"].ToString()) + "\",");
                    //添加除id,name其他数据
                    if (allAsyData)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Columns[j].ColumnName != "id" && dt.Columns[j].ColumnName != "name")
                            {
                                strAsyTree.Append("\"" + dt.Columns[j].ColumnName + "\":");
                                strAsyTree.Append("\"" + StringToJson(drv[j].ToString()) + "\",");
                            }
                        }
                    }
                    strAsyTree.Append("\"state\":");
                    dv1.RowFilter = string.Format("pid = {0}", drv["id"].ToString());
                    if (dv1.Count > 0)
                    {
                        strAsyTree.Append("\"closed\"");
                    }
                    else
                    {
                        strAsyTree.Append("\"open\"");
                    }
                    if (i == dv.Count - 1)
                    {
                        strAsyTree.Append("}");
                    }
                    else
                    {
                        strAsyTree.Append("},");
                    }
                }
                strAsyTree.Append("]");
            }
            return strAsyTree.ToString();
        }
        #endregion

        #region 返回EasyUI Combobox的基础数据(返回数据源指明id,text字段)
        /// <summary>
        /// 返回EasyUI Combobox的基础数据(返回数据源指明id,text字段)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ComboboxToJson(DataTable dt)
        {
            return DtToJson(dt);
        }
        #endregion

        #region 返回EasyUI Combobox的分组数据(返回数据源指明value,text,group字段)
        /// <summary>
        /// 返回EasyUI Combobox的分组数据(返回数据源指明value,text,group字段)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ComboboxGroupToJson(DataTable dt)
        {
            return DtToJson(dt);
        }
        #endregion

        #region 返回指定日期到今天日期的年月Json数据
        /// <summary>
        /// 返回指定日期到今天日期的年月Json数据
        /// </summary>
        /// <param name="year">2010</param>
        /// <returns></returns>
        public static string YearMonthToJson(int year)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("[");
            int nowyear = DateTime.Now.Year;
            string ym;
            string ymvalue;
            if (year <= nowyear)
            {
                for (int i = nowyear; i >= year; i--)
                {
                    if (i == nowyear)
                    {
                        int month = DateTime.Now.Month;
                        for (int j = month; j > 0; j--)
                        {
                            str.Append("{");
                            ym = i.ToString() + "-" + j.ToString().PadLeft(2, '0');
                            ymvalue = i.ToString() + "/" + j.ToString().PadLeft(2, '0') + "/01";
                            str.Append("\"value\":");
                            str.Append("\"" + ymvalue + "\",");
                            str.Append("\"text\":");
                            str.Append("\"" + ym + "\"");
                            if (j == 1 && i == year)
                            {
                                str.Append("}");
                            }
                            else
                            {
                                str.Append("},");
                            }
                        }
                    }
                    else
                    {
                        for (int j = 12; j > 0; j--)
                        {
                            str.Append("{");
                            ym = i.ToString() + "-" + j.ToString().PadLeft(2, '0');
                            ymvalue = i.ToString() + "/" + j.ToString().PadLeft(2, '0') + "/01";
                            str.Append("\"value\":");
                            str.Append("\"" + ymvalue + "\",");
                            str.Append("\"text\":");
                            str.Append("\"" + ym + "\"");
                            if (j == 1 && i == year)
                            {
                                str.Append("}");
                            }
                            else
                            {
                                str.Append("},");
                            }
                        }
                    }
                }
            }
            str.Append("]");
            return str.ToString();
        }
        #endregion
    }
}