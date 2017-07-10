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
    /// 状态事件的类型
    /// </summary>
    public enum StatusEventType : byte
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
        /// 提交新增操作到仓储之后，
        /// 注意，该事件只是领域对象被提交要保存，而不是真的已保存，有可能由于工作单元
        /// 此时还未真正保存到仓储，要捕获真是保存到仓储之后的事件请用Committed版本
        /// </summary>
        Added = 4,

        /// <summary>
        /// 提交修改操作到仓储之前
        /// </summary>
        PreUpdate = 5,
        /// <summary>
        /// 提交修改操作到仓储之后
        /// 注意，该事件只是领域对象被提交要保存，而不是真的已保存，有可能由于工作单元
        /// 此时还未真正保存到仓储，要捕获真是保存到仓储之后的事件请用Committed版本
        /// </summary>
        Updated = 6,

        /// <summary>
        /// 提交删除操作到仓储之前
        /// </summary>
        PreDelete = 7,
        /// <summary>
        /// 提交删除操作到仓储之后
        /// 注意，该事件只是领域对象被提交要保存，而不是真的已保存，有可能由于工作单元
        /// 此时还未真正保存到仓储，要捕获真是保存到仓储之后的事件请用Committed版本
        /// </summary>
        Deleted = 8,


        /// <summary>
        /// 对象被真实提交的前一刻
        /// </summary>
        AddPreCommit = 9,

        /// <summary>
        /// 对象被真实提交到仓储保存后
        /// </summary>
        AddCommitted = 10,

        /// <summary>
        /// 对象被真实提交到仓储修改的前一刻
        /// </summary>
        UpdatePreCommit = 11,

        /// <summary>
        /// 对象被真实提交到仓储修改后
        /// </summary>
        UpdateCommitted = 12,

        /// <summary>
        /// 对象被真实提交到仓储删除后
        /// </summary>
        DeletePreCommit = 13,

        /// <summary>
        /// 对象被真实提交到仓储删除后
        /// </summary>
        DeleteCommitted = 14,

        /// <summary>
        /// 通用的状态事件
        /// </summary>
        Any = 99,
    }
}