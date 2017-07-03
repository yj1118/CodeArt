using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DTO
{
    /// <summary>
    /// 对象类型的条目
    /// </summary>
    public class ListEntry : TypeEntry
    {
        private TypeMetadata _itemMetadata;

        /// <summary>
        /// 对象的元数据
        /// </summary>
        public TypeMetadata ItemMetadata
        {
            get
            {
                if (_itemMetadata == null)
                    Init();
                return _itemMetadata;
            }
        }

        private TypeEntry _itemEntry;

        public TypeEntry ItemEntry
        {
            get
            {
                if (_itemEntry == null)
                    Init();
                return _itemEntry;
            }
        }


        public override EntryCategory Category => EntryCategory.List;

        internal ListEntry(TypeMetadata owner, string name,string typeName, string metadataCode)
            : base(owner, name, typeName, metadataCode)
        {
            if (this.Index.Contains(typeName)) return; //为了避免死循环，对于已经分析过的类型我们不再分析，但是当xx.Metadata的时候，依然会自动分析
            //这里执行一遍，是为了索引里可以找到类型
            this.Index.Add(this);
            Init();
        }


        private void Init()
        {
            _itemMetadata = new TypeMetadata(this.MetadataCode, this.Owner);
            _itemEntry = _itemMetadata.Entries.First();
        }

        public override TypeEntry Clone()
        {
            return new ListEntry(this.Owner, this.Name, this.TypeName, this.MetadataCode);
        }

    }
}
