using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YSL.Common.Utility
{
    /// <summary>
    /// XElement动态解析
    /// </summary>
    public class DynamicXml : DynamicObject, IEnumerable
    {
        private XElement _root;


        public DynamicXml(string xml)
        {
            _root = XElement.Parse(xml);
        }
        public DynamicXml(XElement xe)
        {
            _root = xe;
        }


        #region  属性索引器
        /// <summary>
        /// 属性索引器
        /// </summary>
        /// <param name="name">属性的Name</param>
        /// <returns>属性的值</returns>
        public string this[string name]
        {
            get
            {
                if (_root == null)
                {
                    return String.Empty;
                }
                var attr = _root.Attribute(name);
                if (attr != null)
                {
                    return attr.Value;
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        #endregion

        #region ///Load 加载xml文件
        public DynamicXml()
        {
        }
        public void Load(string uri)
        {
            _root = XElement.Load(uri);
        }
        #endregion

        #region ///Parse 序列化xml字符串
        public void Parse(string xml)
        {
            _root = XElement.Parse(xml);
        }
        #endregion

        #region ///Parse 序列化xml字符串
        public void Parse(XElement xe)
        {
            _root = xe;
        }
        //    protected DynamicXml(IEnumerable<XElement> elements)
        //{
        //    _elements = new List<XElement>(elements);
        //}

        #endregion

        #region ///ToString
        /// <summary>
        /// 返回此节点的缩进 XML或者文本数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_root == null)
            {
                return String.Empty;
            }
            return _root.ToString();
        }
        #endregion

        #region ///TryGetMember
        /// <summary>
        /// 当获取非静态声明代码调用
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = String.Empty;
            //还没有匹配xml，返回空字符串
            if (_root == null)
            {
                result = String.Empty;
            }
            //获取子列表
            var xList = _root.Elements(binder.Name);
            //异常
            if (xList == null)
            {
                return true;
            }
            //子列表不为空
            if (xList.Any())
            {
                var list = new List<DynamicXml>();
                foreach (var item in xList)
                {
                    list.Add(new DynamicXml(item));
                }
                result = list;
            }
            else
            {
                //获取属性值
                result = this[binder.Name];
            }
            Console.WriteLine("TryGetMember被调用了,Name:{0}", binder.Name);
            //return base.TryGetMember(binder, out result);
            return true;
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames();
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Console.WriteLine("TrySetMember被调用了,Name:{0}", binder.Name);
            return base.TrySetMember(binder, value);
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            Console.WriteLine("TryInvoke被调用了");
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Console.WriteLine("TryInvokeMember被调用了,Name:{0}", binder.Name);
            return base.TryInvokeMember(binder, args, out result);
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }
        #endregion

        public IEnumerator GetEnumerator()
        {
            foreach (var element in _root.Elements())
            {
                yield return new DynamicXml(element);
            }
        }
    }
}
