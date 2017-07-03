using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Runtime;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 边界事件的类型
    /// </summary>
    public enum BoundedEvent : byte
    {
        /// <summary>
        /// 构造领域对象之后
        /// </summary>
        Constructed = 1,

        /// <summary>
        /// 领域对象被改变后
        /// </summary>
        Changed = 2,

        /// <summary>
        /// 提交新增操作到仓储之前
        /// </summary>
        PreAdd = 3,
        /// <summary>
        /// 提交新增操作到仓储之后
        /// </summary>
        Added = 4,

        /// <summary>
        /// 提交修改操作到仓储之前
        /// </summary>
        PreUpdate = 5,
        /// <summary>
        /// 提交修改操作到仓储之后
        /// </summary>
        Updated = 6,

        /// <summary>
        /// 提交删除操作到仓储之前
        /// </summary>
        PreDelete = 7,
        /// <summary>
        /// 提交删除操作到仓储之后
        /// </summary>
        Deleted = 8,
        /// <summary>
        /// 通用的边界事件
        /// </summary>
        Any = 9
    }
}