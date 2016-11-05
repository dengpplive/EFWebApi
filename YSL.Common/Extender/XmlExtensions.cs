using System;
using System.Xml;

namespace YSL.Common.Extender
{
    public static class XmlNodeExtensions {
        public static string GetAttributeValue(this XmlNode node, string name, bool throwException = false) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("属性名称不能为空、空字符串或者全是空白字符。");
            }
            if (node.Attributes != null) {
                var attr = node.Attributes[name];
                if (attr != null) {
                    return attr.Value;
                }
                if (throwException) { throw new AttributeNotFoundException(); }
            }
            if (throwException) { throw new AttributesNotFoundException(); }
            return string.Empty;
        }
    }


    public class AttributeNotFoundException : System.Exception {
        public AttributeNotFoundException() : base("节点上没有指定名称的属性。") { }
        public AttributeNotFoundException(string name) : base(string.Format("节点上没有名为{0}的属性", name)) { }
    }
    public class AttributesNotFoundException : System.Exception {
        public AttributesNotFoundException() : base("节点上没有任何属性。") { }
    }
}
