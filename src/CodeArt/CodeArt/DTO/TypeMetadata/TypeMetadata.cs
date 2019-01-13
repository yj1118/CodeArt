using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DTO
{
    /// <summary>
    /// 类型元数据
    /// </summary>
    public sealed class TypeMetadata
    {
        public string MetadataCode
        {
            get;
            private set;
        }

        public IEnumerable<TypeEntry> Entries
        {
            get;
            private set;
        }

        internal TypeIndex Index
        {
            get;
            private set;
        }

        /// <summary>
        /// 所属元数据
        /// </summary>
        public TypeMetadata Parent
        {
            get;
            private set;
        }

        public ObjectEntry Root
        {
            get;
            private set;
        }


        public TypeEntry GetEntry(string typeName)
        {
            TypeEntry entry = null;
            if (this.Index.TryGet(typeName, out entry)) return entry;
            return null;
        }


        /// <summary>
        /// 该构造是一切的起点
        /// </summary>
        /// <param name="metadataCode"></param>
        internal TypeMetadata(string metadataCode)
        {
            this.MetadataCode = metadataCode;
            this.Index = new TypeIndex();
            this.Root = new ObjectEntry(this);
            var dto = DTObject.Create(metadataCode);

            var root = dto.GetRoot();
            //设置了根类型的名称
            this.Root.Name = this.Root.TypeName = root.Name;
            this.Index.Add(this.Root); //对根类型建立索引

            this.Entries = Parse(root);
        }

        internal TypeMetadata(ObjectEntry root, string metadataCode, TypeMetadata parent)
        {
            this.Root = root;
            this.MetadataCode = metadataCode;
            this.Parent = parent;
            this.Index = parent.Index;
            var dto = DTObject.Create(metadataCode);
            this.Entries = Parse(dto.GetRoot());
        }

        internal TypeMetadata(string metadataCode, TypeMetadata parent)
        {
            this.MetadataCode = metadataCode;
            this.Parent = parent;
            this.Index = parent.Index;
            this.Root = new ObjectEntry(this);

            var dto = DTObject.Create(metadataCode);
            this.Entries = Parse(dto.GetRoot());
        }

        #region 分析

        private List<TypeEntry> Parse(DTEObject root)
        {
            List<TypeEntry> entries = new List<TypeEntry>();

            var entities = root.GetEntities();
            foreach (var entity in entities)
            {
                var value = entity as DTEValue;
                if (value != null)
                {
                    entries.Add(CreateEntry(value));
                    continue;
                }

                var obj = entity as DTEObject;
                if (obj != null)
                {
                    entries.Add(CreateEntry(obj));
                    continue;
                }

                var list = entity as DTEList;
                if(list != null)
                {
                    entries.Add(CreateEntry(list));
                    continue;
                }

            }

            return entries;
        }


        private TypeEntry CreateEntry(DTEValue e)
        {
            var entryName = e.Name; //条目名称
            if (e.Value == null) throw new DTOException(string.Format(Strings.DTONotSpecifyType, GetPathName(entryName)));
            var valueCode = e.Value.ToString().Trim();
            if (valueCode.Length == 0) throw new DTOException(string.Format(Strings.DTONotSpecifyType, GetPathName(entryName)));

            TypeEntry target = null;
            if (this.Index.TryGet(valueCode, out target))
            {
                //valueCode是已有类型的名称
                var entry = target.Clone();
                entry.Name = entryName;
                entry.TypeName = valueCode;
                entry.Owner = this;
                return entry;
            }
            else
            {
                var temp = valueCode.Split(',').Select((t) => t.Trim()).ToList();
                var typeName = temp[0]; //第一项作为类型名
                temp.RemoveAt(0);
                var descriptions = temp;
                return new ValueEntry(this, entryName, typeName, e.GetCode(false, true), descriptions);
            }
        }

        private TypeEntry CreateEntry(DTEObject e)
        {
            var name = e.Name; //条目名称
            var metadataCode = e.GetCode(false, true);
            string typeName = GetPathName(name);
            return new ObjectEntry(this, name, typeName, metadataCode);
        }

        private string GetPathName(string name)
        {
            return string.IsNullOrEmpty(this.Root.TypeName)
                    ? name : string.Format("{0}.{1}", this.Root.TypeName, name);
        }


        private TypeEntry CreateEntry(DTEList e)
        {
            var name = e.Name; //条目名称
            string typeName = GetPathName(name);
            if (e.Items.Count == 0)
                throw new DTOException(string.Format(Strings.DTONotSpecifyType, typeName));
            if (e.Items.Count > 1)
                throw new DTOException(string.Format(Strings.DTOListTypeCountError, typeName));

            var metadataCode = GetItemMetadataCode(typeName, e.Items[0]);
            return new ListEntry(this, name, typeName, metadataCode);
        }

        private string GetItemMetadataCode(string listTypeName, DTObject item)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("{");
                code.AppendFormat("{0}.item:", listTypeName);
                code.Append(item.GetCode(false, true));
                code.Append("}");
                return code.ToString();
            }
        }


        #endregion


    }
}
