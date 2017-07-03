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
    /// 理论上，每触发一次信号，就执行一次行为，但是在行为执行的过程中，多次触发信号将只会再次执行一次行为
    /// 行为执行的过程中，触发信号不会导致线程等待
    /// </summary>
    public sealed class AutoResetPipeline : AutoResetPipelineBase
    {
        private Thread _thread;

        public AutoResetPipeline(Action action, long maxConcurrent)
            : base(action, maxConcurrent)
        {
        }

        public AutoResetPipeline(Action action, Action<Exception> catchException)
            : base(action, catchException)
        {
        }

        public AutoResetPipeline(Action action)
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
