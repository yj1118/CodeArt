using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.DTO
{
    internal static class EntityDeserializer2
    {

        public static DTEObject Deserialize(string code)
        {
            if (string.IsNullOrEmpty(code)) return new DTEObject();
            TreeNode node = ParseNode(code);
            List<DTEntity> entities = new List<DTEntity>();
            FillEntities(entities, node);
            return entities.First() as IDTObjectEntity;
        }

        private static TreeNode Parse(string nodeText)
        {
            string nameStr = null, valueStr = null;
            PretreatmentNode(nodeText, ref nameStr, ref valueStr);

            var name = JSON.GetStringValue(JavaScriptUtil.UnencodeMemberValue(nameStr));

            TreeNode node = new TreeNode(name);

            if (IsObjectCode(valueStr))
            {
                List<TreeNode> childs = ParseNodeList(TrimSign(valueStr));
                foreach (TreeNode item in childs)
                {
                    node.AddChild(item);
                }
                node.Type = "object";
                node.Value = TrimSign(valueStr);
            }
            else if (IsListCode(valueStr))
            {
                node.Type = "list";
                string value = TrimSign(valueStr);
                node.Value = value;
            }
            else if (IsBlobCode(valueStr))
            {
                node.Type = "blob";
                string value = valueStr.Substring(5, valueStr.Length - 6).Trim();
                node.Value = value;
            }
            else
            {
                string type = null;
                node.Value = JavaScriptUtil.UnencodeMemberValue(valueStr, ref type);
                node.Type = type;
            }
            return node;
        }




        private static void FillEntities(List<DTEntity> entities, TreeNode node)
        {
            if (node.Type == "object")
            {
                //收集成员
                var members = new List<DTEntity>(node.Childs.Count());
                foreach (TreeNode item in node.Childs)
                {
                    FillEntities(members, item);
                }

                entities.Add(new DTEObject(node.Name, members.ToList<IDTEntity>()));
            }
            else if (node.Type == "list")
            {
                entities.Add(new DTEList(node.Name, node.Value));
            }
            else if (node.Type == "blob")
            {
                entities.Add(new DTEBLob(node.Name, GetBlob(node.Value)));
            }
            else
            {
                if (node.Type == "string")
                {
                    entities.Add(new DTEValue(node.Name, JSON.GetStringValue(node.Value)));
                }
                else
                {
                    entities.Add(new DTEValue(node.Name, JSON.GetValue(node.Value)));
                }
            }
        }

        private static byte[] GetBlob(string blobString)
        {
            //var temp = blobString.Split(',').Select((t) => { return byte.Parse(t); }).ToArray();
            //return Compressor.Decompress(temp);
            return blobString.Split(',').Select((t) => { return byte.Parse(t); }).ToArray();
        }

        private static bool IsListCode(string code)
        {
            if (code != null && code.StartsWith("["))
            {
                if (!code.EndsWith("]"))
                    throw new CodeFormatErrorException("以 [ 作为首字符，但是没有以 ] 结尾！ ");
                return true;
            }
            return false;
        }

        private static bool IsBlobCode(string code)
        {
            if (code != null && code.StartsWith("blob["))
            {
                if (!code.EndsWith("]"))
                    throw new CodeFormatErrorException("以 blob[ 作为首字符，但是没有以 ] 结尾！ ");
                return true;
            }
            return false;
        }

        private static bool IsObjectCode(string code)
        {
            if (code != null && code.StartsWith("{"))
            {
                if (!code.EndsWith("}"))
                    throw new CodeFormatErrorException("以 { 作为首字符，但是没有以 } 结尾！ ");
                return true;
            }
            return false;
        }

        private static string TrimSign(string code)
        {
            return code.Substring(1, code.Length - 2).Trim();
        }

        #region 分析节点

        private static void PretreatmentNode(string nodeText, ref string nameString, ref string valueString)
        {
            nodeText = nodeText.Trim();
            if (IsObjectCode(nodeText) || IsListCode(nodeText) || IsBlobCode(nodeText))
            {
                nameString = string.Empty;
                valueString = nodeText;
            }
            else
            {
                int pos = nodeText.IndexOf(':');
                nameString = pos == -1 ? nodeText : nodeText.Substring(0, pos).Trim();
                valueString = pos == -1 ? null : nodeText.Substring(pos + 1).Trim();
            }
        }

        private static TreeNode ParseNode(string nodeText)
        {
            string nameStr = null, valueStr = null;
            PretreatmentNode(nodeText, ref nameStr, ref valueStr);

            var name = JSON.GetStringValue(JavaScriptUtil.UnencodeMemberValue(nameStr));

            TreeNode node = new TreeNode(name);

            if (IsObjectCode(valueStr))
            {
                List<TreeNode> childs = ParseNodeList(TrimSign(valueStr));
                foreach (TreeNode item in childs)
                {
                    node.AddChild(item);
                }
                node.Type = "object";
                node.Value = TrimSign(valueStr);
            }
            else if (IsListCode(valueStr))
            {
                node.Type = "list";
                string value = TrimSign(valueStr);
                node.Value = value;
            }
            else if (IsBlobCode(valueStr))
            {
                node.Type = "blob";
                string value = valueStr.Substring(5, valueStr.Length - 6).Trim();
                node.Value = value;
            }
            else
            {
                string type = null;
                node.Value = JavaScriptUtil.UnencodeMemberValue(valueStr, ref type);
                node.Type = type;
            }
            return node;
        }

        private static List<TreeNode> ParseNodeList(string code)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            int startIndex = 0;
            var info = JavaScriptUtil.Find(code, startIndex, ',');
            while (!info.IsEmpty())
            {
                nodes.Add(ParseNode(info.PassText));
                info = JavaScriptUtil.Find(code, info.KeyPosition + 1, ',');
            }

            return nodes;
        }

        #endregion

        private class TreeNode
        {
            private string _name;
            public string Name
            {
                get { return _name; }
            }

            private string _type;
            public string Type
            {
                get { return _type; }
                set { _type = value; }
            }

            private string _value;
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public TreeNode(string name)
            {
                _name = name;
            }

            private List<TreeNode> _childs;
            private List<TreeNode> FriendChilds
            {
                get
                {
                    if (_childs == null) _childs = new List<TreeNode>();
                    return _childs;
                }
            }

            internal IEnumerable<TreeNode> Childs
            {
                get
                {
                    return FriendChilds;
                }
            }

            public void AddChild(TreeNode child)
            {
                child.SetParent(this);
            }


            private TreeNode _parent;
            internal TreeNode Parent
            {
                get { return _parent; }
            }

            internal void SetParent(TreeNode parent)
            {
                parent.FriendChilds.Add(this);
                _parent = parent;
            }


        }

    }
}