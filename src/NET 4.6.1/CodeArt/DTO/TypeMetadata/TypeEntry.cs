using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DTO
{
    public abstract class TypeEntry
    {
        /// <summary>
        /// 条目名称
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName
        {
            get;
            internal set;
        }

        /// <summary>
        /// 类型条目的元数据代码
        /// </summary>
        public string MetadataCode
        {
            get;
            internal set;
        }


        /// <summary>
        /// 条目类型
        /// </summary>
        public abstract EntryCategory Category
        {
            get;
        }

        /// <summary>
        /// 所属元数据
        /// </summary>
        public TypeMetadata Owner
        {
            get;
            internal set;
        }

        internal TypeIndex Index
        {
            get
            {
                return this.Owner.Index;
            }
        }

        public TypeEntry Parent
        {
            get
            {
                return this.Owner.Root;
            }
        }


        public TypeEntry(TypeMetadata owner, string name, string typeName, string metadataCode)
        {
            this.Owner = owner;
            this.Name = name;
            this.TypeName = typeName;
            this.MetadataCode = metadataCode;
        }


        public abstract TypeEntry Clone();



    }
}
