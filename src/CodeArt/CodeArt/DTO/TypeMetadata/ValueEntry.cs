using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DTO
{
    /// <summary>
    /// 基本值类型的条目
    /// </summary>
    public class ValueEntry : TypeEntry
    {
        /// <summary>
        /// 描述项的集合
        /// </summary>
        public IList<string> Descriptions
        {
            get;
            private set;
        }

        public override EntryCategory Category => EntryCategory.Value;

        public bool IsString
        {
            get
            {
                return this.TypeName == "string" || this.TypeName == "ascii";
            }
        }

        public ValueEntry(TypeMetadata owner, string name, string typeName, string metadataCode, IList<string> descriptions)
            : base(owner, name, typeName, metadataCode)
        {
            this.Descriptions = descriptions;
        }

        public override TypeEntry Clone()
        {
            return new ValueEntry(this.Owner, this.Name, this.TypeName, this.MetadataCode, this.Descriptions);
        }
    }
}
