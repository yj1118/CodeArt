using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 对象类型的条目
    /// </summary>
    public class ObjectEntry : TypeEntry
    {
        private TypeMetadata _metadata;

        /// <summary>
        /// 对象的元数据
        /// </summary>
        public TypeMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = CreateMetadata();
                return _metadata;
            }
        }

        public IEnumerable<TypeEntry> Childs
        {
            get
            {
                return this.Metadata.Entries;
            }
        }

        /// <summary>
        /// 根据成员名称查找成员
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeEntry GetMemberByName(string name)
        {
            return this.Childs.FirstOrDefault((e) => e.Name.EqualsIgnoreCase(name));
        }


        public override EntryCategory Category => EntryCategory.Object;

        internal ObjectEntry(TypeMetadata owner, string name,string typeName, string metadataCode)
            : base(owner, name, typeName, metadataCode)
        {
            if (this.Index.Contains(typeName)) return; //为了避免死循环，对于已经分析过的类型我们不再分析，但是当xx.Metadata的时候，依然会自动分析
            //这里执行一遍，是为了索引里可以找到类型
            this.Index.Add(this);
            _metadata = CreateMetadata();
        }


        internal ObjectEntry(TypeMetadata metadata)
            : base(metadata, string.Empty, string.Empty, metadata.MetadataCode)
        {
            _metadata = metadata;
        }

        private TypeMetadata CreateMetadata()
        {
            return new TypeMetadata(this, this.MetadataCode, this.Owner);
        }

        public override TypeEntry Clone()
        {
            var entry = new ObjectEntry(this.Owner, this.Name, this.TypeName, this.MetadataCode);
            entry._metadata = this._metadata;
            return entry;
        }

    }
}
