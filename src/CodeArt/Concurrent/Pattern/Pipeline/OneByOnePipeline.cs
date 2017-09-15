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
    /// 异步由thread实现
    /// </summary>
    public sealed class OneByOnePipeline : OneByOnePipelineBase
    {
        private Thread _thread;

        public OneByOnePipeline(Action action, Action<Exception> catchException)
            : base(action, catchException)
        {
        }

        public OneByOnePipeline(Action action)
            : base(action)
        {
        }

        protected override void InitHost(Action backgroundAction)
        {
            _thread = new Thread(new ThreadStart(backgroundAction));
            _thread.IsBackground = true;
            _thread.Name = new Guid().ToString() + " Thread";
            _thread.Start();
        }

        protected override void DisposeHost()
        {
            _thread.Abort();
        }
    }
}
