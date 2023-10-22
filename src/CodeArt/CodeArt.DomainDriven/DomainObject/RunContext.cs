using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 执行上下文，用于监视领域行为或属性在执行中的状态，起到防止嵌套无限循环执行等作用
    /// </summary>
    internal sealed class RunContext
    {
#if DEBUG

        public Guid Id { get; private set; }

#endif

        /// <summary>
        /// 获取或设置是否运行在回调方法、事件方法中
        /// </summary>
        public bool InCallBack { get; set; }

        public int MethodIndex { get; set; }

        public RunContext()
        {
#if DEBUG
            this.Id = Guid.NewGuid();
#endif
            this.InCallBack = false;
            this.MethodIndex = -1;
        }
    }
}