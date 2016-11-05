using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// html文档解析
    /// </summary>
    public static class HtmlParser
    {
        public const char TagStart = '<';
        public const char TagEnd = '>';
        public const char TagClose = '/';
        public const char QuoteSingle = '\'';
        public const char QuoteDouble = '"';
        public const char PropertySplitor = ' ';

        /// <summary>
        /// 获取html的元素
        /// </summary>
        /// <param name="html">要解析的 html 文本。</param>
        /// <param name="tag">要获取内容的元素的标签。</param>
        /// <param name="attr">要获取内容的元素的属性。</param>
        /// <param name="attrValue">要获取内容的元素的属性的值。</param>
        /// <returns></returns>
        public static string[] GetTags(string html, string tag, string attr = "", string attrValue = "")
        {
            List<string> list = new List<string>();
            StringBuilder pattern = new StringBuilder();
            pattern.Append("<");
            if (!string.IsNullOrEmpty(tag))
            {
                pattern.AppendFormat("(?<tag>{0})[^>]*", tag);
            }
            else
            {
                pattern.Append(@"(?<tag>\w+)[^>]*?");
            }
            if (!string.IsNullOrEmpty(attr))
            {
                pattern.Append(attr);
            }
            if (!string.IsNullOrEmpty(attrValue))
            {
                pattern.AppendFormat(@"\s*=\s*(""|')?{0}(""|')?\b?", attrValue);
            }
            pattern.Append(".*?>");
            pattern.Append("(?<content>.*?)");
            pattern.Append(@"</\k<tag>>");

            Regex reg = new Regex(pattern.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(html);
            foreach (Match ch in mc)
            {
                list.Add(ch.Value);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 从 html 文本中获取指定元素或具有指定属性的元素的 html 内容。
        /// </summary>
        /// <param name="html">要解析的 html 文本。</param>
        /// <param name="tag">要获取内容的元素的标签。</param>
        /// <param name="attr">要获取内容的元素的属性。</param>
        /// <param name="attrValue">要获取内容的元素的属性的值。</param>
        /// <returns></returns>
        public static string[] GetInnerHtmls(string html, string tag, string attr, string attrValue)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(html))
            {
                return new string[] { };
            }

            StringBuilder pattern = new StringBuilder();
            pattern.Append("<");
            if (!string.IsNullOrEmpty(tag))
            {
                pattern.AppendFormat("(?<tag>{0})[^>]*", tag);
            }
            else
            {
                pattern.Append(@"(?<tag>\w+)[^>]*?");
            }
            if (!string.IsNullOrEmpty(attr))
            {
                pattern.Append(attr);
            }
            if (!string.IsNullOrEmpty(attrValue))
            {
                pattern.AppendFormat(@"\s*=\s*(""|')?{0}(""|')?\b?", attrValue);
            }
            pattern.Append(".*?>");
            pattern.Append("(?<content>.*?)");
            pattern.Append(@"</\k<tag>>");

            Regex reg = new Regex(pattern.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(html);
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    list.Add(m.Groups["content"].Value);
                }
                return list.ToArray();
            }
            return new string[] { };
        }
        public static string[] GetInnerHtmls(string html, string tag, string attr)
        {
            return GetInnerHtmls(html, tag, attr);
        }
        public static string[] GetInnerHtmls(string html, string tag)
        {
            return GetInnerHtmls(html, tag, null, null);
        }

        public static string GetInnerHtml(string html, string tag, string attr, string attrValue)
        {
            string[] result = GetInnerHtmls(html, tag, attr, attrValue);
            if (result.Length > 0)
            {
                return result[0];
            }
            return "";
        }
        public static string GetInnerHtml(string html, string tag, string attr)
        {
            return GetInnerHtml(html, tag, attr, null);
        }
        public static string GetInnerHtml(string html, string tag)
        {
            return GetInnerHtml(html, tag, null, null);
        }

        public static string[] GetAttributeValues(string html, string tag, string filterAttr, string filterVal, string attr)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(html))
            {
                return new string[] { };
            }

            StringBuilder pattern = new StringBuilder();
            pattern.Append("<");
            if (!string.IsNullOrEmpty(tag))
            {
                pattern.AppendFormat("{0}[^>]*", tag);
            }
            else
            {
                pattern.Append(@"\w+[^>]*?");
            }
            if (!string.IsNullOrEmpty(filterAttr))
            {
                pattern.Append(@"\b*");
                pattern.Append(filterAttr);
            }
            if (!string.IsNullOrEmpty(filterVal))
            {
                pattern.AppendFormat(@"\s*=\s*(""|')?{0}(""|')?", filterVal);
            }
            //if (!string.IsNullOrEmpty(attr)) {
            //    pattern.AppendFormat(@"[^>]*\b*{0}\s*=\s*(""|')?(?<value>[^>""']*)(""|')?\b*", attr);
            //}
            pattern.Append("[^>]*>");

            Regex reg = new Regex(pattern.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection mc = reg.Matches(html);
            if (mc.Count > 0 && !string.IsNullOrEmpty(attr))
            {
                foreach (Match match in mc)
                {
                    string tmp = match.Value;

                    string strReg = string.Format(@"[^>]*\b*{0}\s*=\s*(""|')?(?<value>[^>""']*)(""|')?\b*", attr);

                    //Match m = Regex.Match(tmp, string.Format(@"[^>]*\b*{0}\s*=\s*(""|')?(?<value>[^>""']*)(""|')?\b*",attr));
                    Match m = Regex.Match(tmp, strReg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    //Match m = Regex.Match(tmp, string.Format(@"<[^>]*value=(?<xx>[^>]*)\s.*?>", attr));
                    if (m.Success)
                    {
                        list.Add(m.Groups["value"].Value);
                    }
                    //list.Add(m.Groups["value"].Value);
                }
                return list.ToArray();
            }
            return new string[] { };
        }
        public static string GetAttributeValue(string html, string tag, string filterAttr, string filterVal, string attr)
        {
            string[] result = GetAttributeValues(html, tag, filterAttr, filterVal, attr);
            if (result.Length > 0)
            {
                return result[0];
            }
            return "";
        }

        public static string PickData(string strData)
        {
            return Regex.Match(strData, @"<!\[CDATA\[(?<data>.*?)\]\]>").Groups["data"].Value;
        }

        private static int FindTagStart(string html, string tag)
        {
            if (string.IsNullOrEmpty(html))
            {
                return -1;
            }

            StringBuilder tagBuilder = new StringBuilder();
            bool inTag = false;
            bool inQuote = false;

            char chrQuote = '\0';

            for (int i = 0; i < html.Length; i++)
            {
                char chr = html[i];

                switch (chr)
                {
                    case QuoteSingle:
                    case QuoteDouble:
                        if (inQuote && chr == chrQuote)
                        {
                            inQuote = false;
                            tagBuilder.Append(chr);
                        }
                        break;
                }

                if (html[i] == '<')
                {
                    if (!inTag)
                    {
                        inTag = true;
                    }
                }
            }
            return -1;
        }
        public static string[] GetInnerHtmlCarefully(string html, string tag, string attrName, string attrValue)
        {
            if (string.IsNullOrEmpty(tag)) { return new string[] { html }; }
            if (html.Length == 0) { return new string[0]; }

            List<string> result = new List<string>();
            const char tagStart = '<';
            const char tagEnd = '>';

            bool inTag = false;
            bool inContent = false;
            bool inQuote = false;

            StringBuilder tagBuilder = new StringBuilder();
            StringBuilder propBuilder = new StringBuilder();
            StringBuilder valBuilder = new StringBuilder();

            for (int i = 0; i < html.Length; i++)
            {
                char chr = html[i];
                if (!inQuote)
                {
                    switch (chr)
                    {
                        case tagStart:
                            inTag = true;
                            tagBuilder.Append(chr);
                            break;
                        case tagEnd:
                            if (inTag)
                            {
                                inTag = false;
                                inContent = true;
                            }
                            tagBuilder.Append(chr);
                            break;
                    }
                }
                else
                {
                }
            }

            return result.ToArray();
        }
        public static string TrimComment(string html)
        {
            return Regex.Replace(html, @"<!--.*?-->", "", RegexOptions.Singleline);
        }

        public static string RemoveTag(string html, string tag, string attr = "", string attrValue = "")
        {
            StringBuilder pattern = new StringBuilder();
            pattern.Append("<");
            if (!string.IsNullOrEmpty(tag))
            {
                pattern.AppendFormat("(?<tag>{0})[^>]*", tag);
            }
            else
            {
                pattern.Append(@"(?<tag>\w+)[^>]*?");
            }
            if (!string.IsNullOrEmpty(attr))
            {
                pattern.Append(attr);
            }
            if (!string.IsNullOrEmpty(attrValue))
            {
                pattern.AppendFormat(@"\s*=\s*(""|')?{0}(""|')?\b?", attrValue);
            }
            pattern.Append(".*?>");
            pattern.Append("(?<content>.*?)");
            pattern.Append(@"</\k<tag>>");

            Regex reg = new Regex(pattern.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return reg.Replace(html, "");
        }
    }
}
