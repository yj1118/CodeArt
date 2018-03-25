using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Data.MSSql
{
    public enum SqlLockLevel : byte
    {
        /// <summary>
        /// 无锁
        /// </summary>
        None,
        /// <summary>
        /// 其他线程不能读、不能修改，但是可以添加符合查询条件的数据
        /// </summary>
        XLock,
        /// <summary>
        /// 其他线程不能读、不能修改也不能添加符合查询条件的数据
        /// </summary>
        HoldLock,
        /// <summary>
        /// 共享锁，可以同时读，但是不能读的时候写入，写入的时候也不能读
        /// </summary>
        ShareLock
    }
}
