using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 状态对象，我们可以明确的知道领域对象的仓储状态
    /// </summary>
    public interface IStateObject 
    {
        /// <summary>
        /// 是否为脏对象
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// 是否为新建对象
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// 标记对象为干净对象
        /// </summary>
        void MarkClean();

        /// <summary>
        /// 标记对象为新建对象
        /// </summary>
        void MarkNew();

        /// <summary>
        /// 标记对象为脏对象
        /// </summary>
        void MarkDirty();

        /// <summary>
        /// 保存状态
        /// </summary>
        void SaveState();

        /// <summary>
        /// 从最后一次保存状态中加载
        /// </summary>
        void LoadState();
    }
}
