﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 执行上下文，用于监视领域行为或属性在执行中的状态，起到防止嵌套无限循环执行等作用
    /// </summary>
    internal sealed class RunContext
    {
        /// <summary>
        /// 获取或设置是否运行在回调方法、事件方法中
        /// </summary>
        public bool InCallBack { get; set; }

        public RunContext()
        {
            this.InCallBack = false;
        }
    }
}