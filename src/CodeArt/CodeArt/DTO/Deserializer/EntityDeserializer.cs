using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DTO
{
    internal static class EntityDeserializer
    {

        public static DTEObject Deserialize(string code, bool isReadOnly, bool isPinned)
        {
            if (string.IsNullOrEmpty(code)) return DTOPool.CreateDTEObject(isPinned);
            return Deserialize(new StringSegment(code), isReadOnly, isPinned);
        }

        private static DTEObject Deserialize(StringSegment code, bool isReadOnly, bool isPinned)
        {
            if (code.IsEmpty()) return DTOPool.CreateDTEObject(isPinned);
            var node = ParseNode(CodeType.Object, code);

            DTEObject result = null;
            using (var temp = DTOPool.EntitiesPool.Borrow())
            {
                List<DTEntity> entities = temp.Item;
                FillEntities(entities, node, isReadOnly, isPinned);
                result = entities.First() as DTEObject;
            }
            return result;
        }

        private static void FillEntities(List<DTEntity> entities, CodeTreeNode node, bool isReadOnly, bool isPinned)
        {
            var name = JSON.GetStringValue(node.Name.ToString());
            if (node.Type == CodeType.Object)
            {
                var members = DTOPool.CreateDTEntities(isPinned);
                //收集成员
                foreach (CodeTreeNode item in node.Childs)
                {
                    FillEntities(members, item, isReadOnly, isPinned);
                }

                var obj = DTOPool.CreateDTEObject(members, isPinned);
                obj.Name = name;
                entities.Add(obj);
            }
            else if (node.Type == CodeType.List)
            {
                var childs = DTOPool.CreateObjects(isPinned);
                using (var temp = DTOPool.EntitiesPool.Borrow())
                {
                    //收集成员
                    var tempChilds = temp.Item;
                    foreach (CodeTreeNode item in node.Childs)
                    {
                        FillEntities(tempChilds, item, isReadOnly, isPinned);
                    }

                    foreach(var e in tempChilds)
                    {
                        var item = CreateDTObject(e, isReadOnly, isPinned);
                        childs.Add(item);
                    }
                }

                var list = DTOPool.CreateDTEList(childs, isPinned);
                list.Name = name;
                entities.Add(list);
            }
            else
            {
                object value = GetNodeValue(node);
                var dte = DTOPool.CreateDTEValue(name, value, isPinned);
                entities.Add(dte);
            }
        }

        private static object GetNodeValue(CodeTreeNode node)
        {
            if(node.Type == CodeType.StringValue)
            {
                var value = JSON.GetStringValue(node.Value);
                Guid guid = Guid.Empty;
                if (Guid.TryParse(value, out guid)) return guid;
                return value;
            }
            return JSON.GetValue(node.Value);
        }


        private static StringSegment TrimSign(StringSegment code)
        {
            return code.Substring(1, code.Length - 2).Trim();
        }

        private static DTObject CreateDTObject(DTEntity root, bool isReadOnly, bool isPinned)
        {
            var o = root as DTEObject;
            if (o != null) return DTOPool.CreateObject(o, isReadOnly, isPinned);

            var members = DTOPool.CreateDTEntities(isPinned);
            members.Add(root);

            o = DTOPool.CreateDTEObject(members, isPinned);
            return DTOPool.CreateObject(o, isReadOnly, isPinned);
        }


        #region 分析节点




        private static CodeTreeNode ParseNode(CodeType parentCodeType, StringSegment nodeCode)
        {
            var nv = PreHandle(parentCodeType, nodeCode);
            var name = nv.Name;
            var value = nv.Value;

            CodeTreeNode node;

            if (CodeTreeNode.IsObject(value))
            {
                value = TrimSign(value);
                var childs = ParseNodes(CodeType.Object, value);
                node = new CodeTreeNode(name, value, CodeType.Object, childs);
            }
            else if (CodeTreeNode.IsList(value))
            {
                value = TrimSign(value);
                var childs = ParseNodes(CodeType.List, value);
                node = new CodeTreeNode(name, value, CodeType.List, childs);
            }
            else
            {
                var codeType = CodeTreeNode.GetValueType(value);
                node = new CodeTreeNode(name, value, codeType);
            }
            return node;
        }

        private struct NameAndValue
        {
            public StringSegment Name;
            public StringSegment Value;

            public NameAndValue(StringSegment name, StringSegment value)
            {
                this.Name = name;
                this.Value = value;
            }

        }


        /// <summary>
        /// 预处理节点代码
        /// </summary>
        /// <param name="nodeCode"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static NameAndValue PreHandle(CodeType parentCodeType, StringSegment nodeCode)
        {
            StringSegment name, value;
            var code = nodeCode.Trim();
            if (CodeTreeNode.IsObject(code) || CodeTreeNode.IsList(code))
            {
                name = StringSegment.Null;
                value = code;
            }
            else
            {
                var info = DeserializerUtil.Find(code, 0, ':');
                if (parentCodeType == CodeType.Object)
                {
                    name = !info.Finded ? code : code.Substring(0, info.KeyPosition).Trim();
                    value = !info.Finded ? StringSegment.Null : code.Substring(info.KeyPosition + 1).Trim();
                }
                else
                {
                    name = !info.Finded ? StringSegment.Null : code.Substring(0, info.KeyPosition).Trim();
                    value = !info.Finded ? code : code.Substring(info.KeyPosition + 1).Trim();
                }
            }

            return new NameAndValue(name, value);
        }

        private static CodeTreeNode[] ParseNodes(CodeType parentCodeType, StringSegment code)
        {
            CodeTreeNode[] result = null;
            using (var temp = _nodesPool.Borrow())
            {
                List<CodeTreeNode> nodes = temp.Item;

                int startIndex = 0;
                var info = DeserializerUtil.Find(code, startIndex, ',');
                while (!info.IsEmpty())
                {
                    nodes.Add(ParseNode(parentCodeType, info.Pass));
                    info = DeserializerUtil.Find(code, info.KeyPosition + 1, ',');
                }

                result = nodes.ToArray();
            }
            return result;
        }

        #endregion

        private static Pool<List<CodeTreeNode>> _nodesPool;

        static EntityDeserializer()
        {
            _nodesPool = new Pool<List<CodeTreeNode>>(() =>
            {
                return new List<CodeTreeNode>();
            }, (nodes, phase) =>
             {
                 nodes.Clear();
                 return true;
             }, new PoolConfig()
             {
                 MaxRemainTime = 300 //5分钟之内没有被使用，那么销毁
             });

            
        }
    }
}