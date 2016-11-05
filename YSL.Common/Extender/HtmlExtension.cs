using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YSL.Common.Extender
{
    public static class HtmlElementExtension {
        public static IEnumerable<HtmlElement> GetElements(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                var elements = new List<HtmlElement>();
                foreach(HtmlElement child in value.Children) {
                    if(predicate == null || predicate(child)) {
                        elements.Add(child);
                    }
                    if(child.Children.Count > 0) {
                        var elementOfChildren = GetElements(child, predicate);
                        elements.AddRange(elementOfChildren);
                    }
                }
                return elements;
            }
            return Enumerable.Empty<HtmlElement>();
        }

        public static IEnumerable<HtmlElement> GetChildren(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                return predicate == null ? value.Children.Cast<HtmlElement>().ToList() : value.Children.Cast<HtmlElement>().Where(predicate).ToList();
            }
            return Enumerable.Empty<HtmlElement>();
        }

        public static HtmlElement GetFirstChild(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                for(int i = 0; i < value.Children.Count; i++) {
                    var child = value.Children[i];
                    if(predicate == null || predicate(child))
                        return child;
                }
            }
            return null;
        }

        public static HtmlElement GetFirstElement(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                for(int i = 0; i < value.Children.Count; i++) {
                    var child = value.Children[i];
                    if(predicate == null || predicate(child)) {
                        return child;
                    }
                    if(child.Children.Count > 0) {
                        var elementOfChildren = GetFirstElement(child, predicate);
                        if(elementOfChildren != null)
                            return elementOfChildren;
                    }
                }
            }
            return null;
        }

        public static HtmlElement GetLastChild(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                for(int i = value.Children.Count - 1; i >= 0; i--) {
                    var child = value.Children[i];
                    if(predicate == null || predicate(child))
                        return child;
                }
            }
            return null;
        }

        public static HtmlElement GetLastElement(this HtmlElement value, Func<HtmlElement, bool> predicate) {
            if(value != null) {
                for(int i = value.Children.Count - 1; i >= 0; i--) {
                    var child = value.Children[i];
                    if(predicate == null || predicate(child)) {
                        return child;
                    }
                    if(child.Children.Count > 0) {
                        var elementOfChildren = GetFirstElement(child, predicate);
                        if(elementOfChildren != null)
                            return elementOfChildren;
                    }
                }
            }
            return null;
        }

        public static HtmlElement GetFirstElementByClassName(this HtmlElement value, string className) {
            return GetFirstElement(value, item => HasClassName(item, className));
        }

        public static HtmlElement GetFirstChildByClassName(this HtmlElement value, string className) {
            return GetFirstChild(value, item => HasClassName(item, className));
        }

        public static IEnumerable<HtmlElement> GetElementsByClassName(this HtmlElement value, string className) {
            return GetElements(value, item => HasClassName(item, className));
        }

        public static IEnumerable<HtmlElement> GetChildrenByClassName(this HtmlElement value, string className) {
            return GetChildren(value, item => HasClassName(item, className));
        }

        public static HtmlElement GetFirstElementByAttibute(this HtmlElement value, string attributeName, string attributeValue) {
            return GetFirstElement(value, item => Validate(item, attributeName, attributeValue));
        }

        public static HtmlElement GetFirstChildByAttibute(this HtmlElement value, string attributeName, string attributeValue) {
            return GetFirstChild(value, item => Validate(item, attributeName, attributeValue));
        }

        public static IEnumerable<HtmlElement> GetElementsByAttibute(this HtmlElement value, string attributeName, string attributeValue) {
            return GetElements(value, item => Validate(item, attributeName, attributeValue));
        }

        public static IEnumerable<HtmlElement> GetChildrenByAttibute(this HtmlElement value, string attributeName, string attributeValue) {
            return GetChildren(value, item => Validate(item, attributeName, attributeValue));
        }

        public static HtmlElement GetFirstElementByTagName(this HtmlElement value, string tagName) {
            return GetFirstElement(value, item => item.TagName == tagName);
        }

        public static HtmlElement GetFirstChildByTagName(this HtmlElement value, string tagName) {
            return GetFirstChild(value, item => item.TagName == tagName);
        }

        public static HtmlElement GetLastElementByTagName(this HtmlElement value, string tagName) {
            return GetLastElement(value, item => item.TagName == tagName);
        }

        public static HtmlElement GetLastChildByTagName(this HtmlElement value, string tagName) {
            return GetLastChild(value, item => item.TagName == tagName);
        }

        public static bool HasClassName(this HtmlElement value, string className) {
            if(value == null || string.IsNullOrWhiteSpace(className))
                return false;

            var classAttribute = value.GetAttribute("className");

            if(string.IsNullOrWhiteSpace(classAttribute))
                return false;

            className = className.Trim();
            return classAttribute.Split(' ').Any(item => System.String.Compare(item, className, System.StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static bool Validate(this HtmlElement value, string attributeName, string attributeValue) {
            if(value == null || string.IsNullOrWhiteSpace(attributeName))
                return false;

            return value.GetAttribute(attributeName) == attributeValue;
        }

        public static HtmlElement GetLastChild(this HtmlElement value) {
            if(value == null || value.Children.Count == 0)
                return null;
            return value.Children[value.Children.Count - 1];
        }

        public static string GetSelfText(this HtmlElement value) {
            var pattern = "<(((?<tagName>[a-zA-Z]+)[^>]*>((?<nestTag><\\k<tagName>[^>]*>)|</\\k<tagName>>(?<-nestTag>)|.*?)*</\\k<tagName>)|([a-zA-Z]+/)|(br/?))>";
            return Regex.Replace(value.InnerHtml, pattern, string.Empty, RegexOptions.Singleline | RegexOptions.Compiled);
        }
    }

    public static class HtmlDocumentExtension {
        public static IEnumerable<HtmlElement> GetElements(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetElements(predicate);
            }
            return Enumerable.Empty<HtmlElement>();
        }

        public static IEnumerable<HtmlElement> GetChildren(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetChildren(predicate);
            }
            return Enumerable.Empty<HtmlElement>();
        }

        public static HtmlElement GetFirstChild(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetFirstChild(predicate);
            }
            return null;
        }

        public static HtmlElement GetFirstElement(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetFirstElement(predicate);
            }
            return null;
        }

        public static HtmlElement GetLastChild(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetLastChild(predicate);
            }
            return null;
        }

        public static HtmlElement GetLastElement(this HtmlDocument document, Func<HtmlElement, bool> predicate) {
            if(document != null && document.Body != null) {
                return document.Body.GetLastElement(predicate);
            }
            return null;
        }

        public static HtmlElement GetFirstElementByClassName(this HtmlDocument document, string className) {
            return GetFirstElement(document, item => item.HasClassName(className));
        }

        public static HtmlElement GetFirstChildByClassName(this HtmlDocument document, string className) {
            return GetFirstChild(document, item => item.HasClassName(className));
        }

        public static IEnumerable<HtmlElement> GetElementsByClassName(this HtmlDocument document, string className) {
            return GetElements(document, item => item.HasClassName(className));
        }

        public static IEnumerable<HtmlElement> GetChildrenByClassName(this HtmlDocument document, string className) {
            return GetChildren(document, item => item.HasClassName(className));
        }

        public static HtmlElement GetFirstElementByAttibute(this HtmlDocument document, string attributeName, string attributeValue) {
            return GetFirstElement(document, item => item.Validate(attributeName, attributeValue));
        }

        public static HtmlElement GetFirstChildByAttibute(this HtmlDocument document, string attributeName, string attributeValue) {
            return GetFirstChild(document, item => item.Validate(attributeName, attributeValue));
        }

        public static IEnumerable<HtmlElement> GetElementsByAttibute(this HtmlDocument document, string attributeName, string attributeValue) {
            return GetElements(document, item => item.Validate(attributeName, attributeValue));
        }

        public static IEnumerable<HtmlElement> GetChildrenByAttibute(this HtmlDocument document, string attributeName, string attributeValue) {
            return GetChildren(document, item => item.Validate(attributeName, attributeValue));
        }

        public static HtmlElement GetFirstElementByTagName(this HtmlDocument document, string tagName) {
            return GetFirstElement(document, item => item.TagName == tagName);
        }

        public static HtmlElement GetFirstChildByTagName(this HtmlDocument document, string tagName) {
            return GetFirstChild(document, item => item.TagName == tagName);
        }

        public static HtmlElement GetLastElementByTagName(this HtmlDocument document, string tagName) {
            return GetLastElement(document, item => item.TagName == tagName);
        }

        public static HtmlElement GetLastChildByTagName(this HtmlDocument document, string tagName) {
            return GetLastChild(document, item => item.TagName == tagName);
        }
    }
}