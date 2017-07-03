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
    /// 每触发一次信号，就执行一次行为，在行为执行的过程中，触发信号的线程将等待
    /// 异步由task实现
    /// </summary>
    public sealed class OneByOnePipelineTask : OneByOnePipelineBase
    {
        private Task _task;
        public OneByOnePipelineTask(Action action, Action<Exception> catchException)
            : base(action, catchException)
        {
        }

        public OneByOnePipelineTask(Action action)
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
