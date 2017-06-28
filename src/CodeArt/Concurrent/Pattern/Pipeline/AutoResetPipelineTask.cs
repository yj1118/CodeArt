using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Log;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 基于信号的行为
    /// 理论上，每触发一次信号，就执行一次行为，但是在行为执行的过程中，多次触发信号不会阻塞
    /// 行为执行的过程中，触发信号不会导致线程等待
    /// 异步由task实现
    /// </summary>
    public sealed class AutoResetPipelineTask : AutoResetPipelineBase
    {
        private Task _task;
        public AutoResetPipelineTask(Action action, Action<Exception> catchException)
            : base(action, catchException)
        {
        }

        public AutoResetPipelineTask(Action action)
            : base(action)
        {
        }

        protected override void InitHost(Action backgroundAction)
        {
            _task = Task.Factory.StartNew(backgroundAction);
        }

        protected override void DisposeHost()
        {
        }
    }
}
