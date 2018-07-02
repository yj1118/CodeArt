using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using HtmlAgilityPack;

namespace CodeArt.HtmlWrapper
{

    internal class HtmlNodeNavigator : XPathNavigator
    {
        // Fields
        private int _attindex;
        private HtmlNode _currentnode;
        private readonly HtmlDocument _doc;
        private readonly HtmlNameTable _nametable;
        internal bool Trace;

        // Methods
        internal HtmlNodeNavigator()
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this.Reset();
        }

        private HtmlNodeNavigator(HtmlNodeNavigator nav)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            if (nav == null)
            {
                throw new ArgumentNullException("nav");
            }
            this._doc = nav._doc;
            this._currentnode = nav._currentnode;
            this._attindex = nav._attindex;
            this._nametable = nav._nametable;
        }

        public HtmlNodeNavigator(Stream stream)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(stream);
            this.Reset();
        }

        public HtmlNodeNavigator(TextReader reader)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(reader);
            this.Reset();
        }

        public HtmlNodeNavigator(string path)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(path);
            this.Reset();
        }

        internal HtmlNodeNavigator(HtmlDocument doc, HtmlNode currentNode)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }
            if (currentNode.OwnerDocument != doc)
            {
                throw new ArgumentException("Reference node must be a child of this node");
            }
            this._doc = doc;
            this.Reset();
            this._currentnode = currentNode;
        }

        public HtmlNodeNavigator(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(stream, detectEncodingFromByteOrderMarks);
            this.Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(stream, encoding);
            this.Reset();
        }

        public HtmlNodeNavigator(string path, bool detectEncodingFromByteOrderMarks)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(path, detectEncodingFromByteOrderMarks);
            this.Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(path, encoding);
            this.Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(stream, encoding, detectEncodingFromByteOrderMarks);
            this.Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(path, encoding, detectEncodingFromByteOrderMarks);
            this.Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(stream, encoding, detectEncodingFromByteOrderMarks, buffersize);
            this.Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            this._doc = new HtmlDocument();
            this._nametable = new HtmlNameTable();
            this._doc.Load(path, encoding, detectEncodingFromByteOrderMarks, buffersize);
            this.Reset();
        }

        public override XPathNavigator Clone()
        {
            return new HtmlNodeNavigator(this);
        }

        public override string GetAttribute(string localName, string namespaceURI)
        {
            HtmlAttribute attribute = this._currentnode.Attributes[localName];
            if (attribute == null)
            {
                return null;
            }
            return attribute.Value;
        }

        public override string GetNamespace(string name)
        {
            return string.Empty;
        }

      

        public override bool IsSamePosition(XPathNavigator other)
        {
            HtmlNodeNavigator navigator = other as HtmlNodeNavigator;
            if (navigator == null)
            {
                return false;
            }
            return (navigator._currentnode == this._currentnode);
        }

        public override bool MoveTo(XPathNavigator other)
        {
            HtmlNodeNavigator navigator = other as HtmlNodeNavigator;
            if ((navigator != null) && (navigator._doc == this._doc))
            {
                this._currentnode = navigator._currentnode;
                this._attindex = navigator._attindex;
                return true;
            }
            return false;
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            int attributeIndex = this._currentnode.Attributes.GetAttributeIndex(localName);
            if (attributeIndex == -1)
            {
                return false;
            }
            this._attindex = attributeIndex;
            return true;
        }

        public override bool MoveToFirst()
        {
            if (this._currentnode.ParentNode == null)
            {
                return false;
            }
            if (this._currentnode.ParentNode.FirstChild == null)
            {
                return false;
            }
            this._currentnode = this._currentnode.ParentNode.FirstChild;
            return true;
        }

        public override bool MoveToFirstAttribute()
        {
            if (!this.HasAttributes)
            {
                return false;
            }
            this._attindex = 0;
            return true;
        }

        public override bool MoveToFirstChild()
        {
            if (!this._currentnode.HasChildNodes)
            {
                return false;
            }
            this._currentnode = this._currentnode.ChildNodes[0];
            return true;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        public override bool MoveToId(string id)
        {
            HtmlNode elementbyId = this._doc.GetElementbyId(id);
            if (elementbyId == null)
            {
                return false;
            }
            this._currentnode = elementbyId;
            return true;
        }

        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            if (this._currentnode.NextSibling == null)
            {
                return false;
            }
            this._currentnode = this._currentnode.NextSibling;
            return true;
        }

        public override bool MoveToNextAttribute()
        {
            if (this._attindex >= (this._currentnode.Attributes.Count - 1))
            {
                return false;
            }
            this._attindex++;
            return true;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        public override bool MoveToParent()
        {
            if (this._currentnode.ParentNode == null)
            {
                return false;
            }
            this._currentnode = this._currentnode.ParentNode;
            return true;
        }

        public override bool MoveToPrevious()
        {
            if (this._currentnode.PreviousSibling == null)
            {
                return false;
            }
            this._currentnode = this._currentnode.PreviousSibling;
            return true;
        }

        public override void MoveToRoot()
        {
            this._currentnode = this._doc.DocumentNode;
        }

        private void Reset()
        {
            this._currentnode = this._doc.DocumentNode;
            this._attindex = -1;
        }

        // Properties
        public override string BaseURI
        {
            get
            {
                return this._nametable.GetOrAdd(string.Empty);
            }
        }

        public HtmlDocument CurrentDocument
        {
            get
            {
                return this._doc;
            }
        }

        public HtmlNode CurrentNode
        {
            get
            {
                return this._currentnode;
            }
        }

        public override bool HasAttributes
        {
            get
            {
                return (this._currentnode.Attributes.Count > 0);
            }
        }

        public override bool HasChildren
        {
            get
            {
                return (this._currentnode.ChildNodes.Count > 0);
            }
        }

        public override bool IsEmptyElement
        {
            get
            {
                return !this.HasChildren;
            }
        }

        public override string LocalName
        {
            get
            {
                if (this._attindex != -1)
                {
                    return this._nametable.GetOrAdd(this._currentnode.Attributes[this._attindex].Name);
                }
                return this._nametable.GetOrAdd(this._currentnode.LocalName());
            }
        }

        public override string Name
        {
            get
            {
                return this._nametable.GetOrAdd(this._currentnode.Name);
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return this._nametable.GetOrAdd(string.Empty);
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return this._nametable;
            }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                switch (this._currentnode.NodeType)
                {
                    case HtmlNodeType.Document:
                        return XPathNodeType.Root;

                    case HtmlNodeType.Element:
                        if (this._attindex == -1)
                        {
                            return XPathNodeType.Element;
                        }
                        return XPathNodeType.Attribute;

                    case HtmlNodeType.Comment:
                        return XPathNodeType.Comment;

                    case HtmlNodeType.Text:
                        return XPathNodeType.Text;
                }
                throw new NotImplementedException("Internal error: Unhandled HtmlNodeType: " + this._currentnode.NodeType);
            }
        }

        public override string Prefix
        {
            get
            {
                return this._nametable.GetOrAdd(string.Empty);
            }
        }

        public override string Value
        {
            get
            {
                switch (this._currentnode.NodeType)
                {
                    case HtmlNodeType.Document:
                        return "";

                    case HtmlNodeType.Element:
                        if (this._attindex == -1)
                        {
                            return this._currentnode.InnerText;
                        }
                        return this._currentnode.Attributes[this._attindex].Value.ToLower();//属性的值忽略大小写

                    case HtmlNodeType.Comment:
                        return ((HtmlCommentNode)this._currentnode).Comment;

                    case HtmlNodeType.Text:
                        return ((HtmlTextNode)this._currentnode).Text;
                }
                throw new NotImplementedException("Internal error: Unhandled HtmlNodeType: " + this._currentnode.NodeType);
            }
        }

        public override string XmlLang
        {
            get
            {
                return this._nametable.GetOrAdd(string.Empty);
            }
        }

        #region 解决命名空间查询

        public override XPathNodeIterator Select(string xpath)
        {
            xpath = xpath.ToLower();
            return base.Select(xpath, this);
        }

        public override XPathNavigator SelectSingleNode(string xpath)
        {
            xpath = xpath.ToLower();
            return base.SelectSingleNode(xpath, this);
        }

        public override string LookupNamespace(string prefix)
        {
            return string.Empty;
        }

        #endregion

    }
}