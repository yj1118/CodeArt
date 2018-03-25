using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 简单的管道行为，不会单独开辟线程，在调用县城中依次执行每个行为
    /// </summary>
    public class ActionPipelineSlim
    {
        private ConcurrentQueue<Action<Action>> _actions = new ConcurrentQueue<Action<Action>>();

        /// <summary>
        /// 同步计数，很多情况下，有可能多个动画或者异步操作完毕后才算真正的执行完毕，因此要使用同步计数
        /// </summary>
        private int _syncCount;

        public int ActionLength
        {
            get
            {
                return _actions.Count;
            }
        }

        public ActionPipelineSlim()
        {
            _syncCount = 0;
        }

        private volatile bool _locked = false;
        private object _syncObject = new object();

        /// <summary>
        /// 将方法<paramref name="action"/>放入队列中执行，<paramref name="action"/>不一定会立即执行，但是方法会立即返回，不会等待
        /// 请在action执行完毕后，主动调用传递的complete方法
        /// </summary>
        /// <param name="action"></param>
        public void Queue(Action<Action> action)
        {
            _actions.Enqueue(action);
            TryContinue();
        }

        private void TryContinue()
        {
            if (_locked) return;
            if (_actions.TryDequeue(out var action))
            {
                _locked = true;
                action(Complete);
            }
        }

        /// <summary>
        /// 累加异步计数，当有多个异步任务时，请使用该方法
        /// </summary>
        public void IncrementAsync()
        {
            Interlocked.Increment(ref _syncCount);
        }


        private void Complete()
        {
            if (_syncCount > 0)
            {
                Interlocked.Decrement(ref _syncCount);
            }

            if (_syncCount == 0)
            {
                _locked = false;
                TryContinue();
            }
        }
    }
}