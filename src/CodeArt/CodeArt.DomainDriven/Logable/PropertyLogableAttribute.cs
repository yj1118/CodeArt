using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示属性具有日志的能力
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class PropertyLogableAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }

        public TrackLevel Level
        {
            get;
            set;
        }

        /// <summary>
        /// 架构代码，决定需要存储的与对象自身有关的信息
        /// 值类型不用填写该值
        /// </summary>
        public string SchemaCode
        {
            get;
            set;
        }

        public PropertyLogableAttribute(TrackLevel level, string schemaCode)
        {
            this.Level = level;
            this.SchemaCode = schemaCode;
        }

        public PropertyLogableAttribute(TrackLevel level)
            : this(level, string.Empty)
        {
        }

        public PropertyLogableAttribute()
            : this(TrackLevel.Pro, string.Empty)
        {
        }
    }

    public enum TrackLevel : byte
    {
        /// <summary>
        /// 简单的记录下被更改的事实
        /// </summary>
        Slim = 1,
        /// <summary>
        /// 仅记录新值
        /// </summary>
        Normal =2,
        /// <summary>
        /// 完整的记录新旧值
        /// </summary>
        Pro= 3
    }

}
