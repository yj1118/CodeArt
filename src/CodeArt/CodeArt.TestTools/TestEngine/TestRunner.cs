using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using CodeArt.DTO;
using CodeArt.Util;
using System.Collections.Concurrent;
using System.Timers;

namespace CodeArt.TestTools
{
    internal class TestRunner
    {
        private Dictionary<Type, object> _documents;
        private ITestLog _log;

        public long UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// 运行的批次号，
        /// 同一个用户的运行器的批次号是不同的，
        /// 越晚执行的运行器，批次号越大
        /// </summary>
        public long BatchId
        {
            get;
            private set;
        }

        public DateTime? StartTime
        {
            get;
            private set;
        }


        public DateTime? EndTime
        {
            get;
            private set;
        }

        public bool IsExpired
        {
            get
            {
                return this.EndTime != null && this.EndTime.Value.AddMinutes(10) < DateTime.Now;
            }
        }

        public bool IsStop
        {
            get;
            private set;
        }

        public void Stop()
        {
            this.IsStop = true;
        }


        private TestRunner(long userId)
        {
            this.UserId = userId;
            this.BatchId = long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss"));
            this.IsStop = false;

            _documents = new Dictionary<Type, object>();
            _log = TestLogFactory.Create();
        }

        private void Start()
        {
            this.StartTime = DateTime.Now;

            DTObject content = DTObject.Create();
            content["type"] = LogContentType.Start;
            content["message"] = "运行开始";

            _log.Write(this.UserId,this.BatchId, content);

        }

        private void End()
        {
            DTObject content = DTObject.Create();
            if(this.IsStop)
            {
                content["type"] = LogContentType.End;
                content["message"] = "运行已终止";
            }
            else
            {
                content["type"] = LogContentType.End;
                content["message"] = "运行结束";
            }

            _log.Write(this.UserId, this.BatchId, content);

            this.EndTime = DateTime.Now;
        }


        /// <summary>
        /// 获取测试方法所在的测试类的示例
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public object GetDocumentInstance(TestCaseAttribute attr)
        {
            object instance = null;
            if (!_documents.TryGetValue(attr.DocumentType, out instance))
            {
                instance = Activator.CreateInstance(attr.DocumentType);
                _documents.Add(attr.DocumentType, instance);
            }
            return instance;
        }

        /// <summary>
        /// 将测试方法的运行情况写入到日志中
        /// </summary>
        /// <param name="case"></param>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        public void WriteLog(TestCaseAttribute @case, string message, RunStatus status)
        {
            DTObject content = DTObject.Create();
            content["type"] = LogContentType.Run;
            content["caseKey"] = @case.Key;
            content["message"] = message;
            content["status"] = (byte)status;
            _log.Write(this.UserId, this.BatchId, content);
        }

        public IEnumerable<TestLog> GetLogs(long ticks)
        {
            return _log.Read(this.UserId, this.BatchId, ticks);
        }

        #region 静态

        /// <summary>
        /// 正在运行的运行器
        /// </summary>
        private static Dictionary<long, TestRunner> _runtime = new Dictionary<long, TestRunner>();

        private static ConcurrentDictionary<long, TestRunner> _over = new ConcurrentDictionary<long, TestRunner>();

        /// <summary>
        /// 重置运行状态（在开始运行之前，重置运行状态）
        /// </summary>
        /// <param name="userId"></param>
        private static void Reset(long userId)
        {
            _over.TryRemove(userId, out var temp);
        }

        public static void Run(long userId, Action<TestRunner> action)
        {
            TestRunner runner = null;
            lock (_runtime)
            {
                if (!_runtime.ContainsKey(userId))
                {
                    Reset(userId);
                    runner = new TestRunner(userId);
                    _runtime.Add(userId, runner);
                }
                else
                    throw new UserUIException("每个用户同一时间只能运行一个测试");
            }

            runner.Start();

            Task.Run(() =>
            {
                try
                {
                    action(runner);
                }
                catch(Exception)
                {
                    //不做任何处理
                }
                finally
                {
                    //运行完毕后移除实例
                    runner.End();
                    _runtime.Remove(userId);

                    //并将实例放入结束区
                    _over.AddOrUpdate(userId, runner, (key, old) =>
                    {
                        return runner;
                    });
                }
            });
        }

        public static void Stop(long userId)
        {
            lock (_runtime)
            {
                if(_runtime.TryGetValue(userId,out var runner))
                {
                    runner.Stop();
                }
            }
        }


        /// <summary>
        /// 获得运行时的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static TestRuntime GetRuntime(long userId, long ticks)
        {
            var logs = GetRecentLogs(userId, ticks);

            if (logs == null) return TestRuntime.CreateEmpty();

            var isOver = IsOver(userId);

            var nextTicks = isOver ? 0L : GetNextTicks(ticks);

            return new TestRuntime(logs, nextTicks, isOver);

            long GetNextTicks(long prevTicks)
            {
                if (logs.Count() == 0)
                {
                    return prevTicks;
                } 
                return logs.Last().Ticks;
            }
        }


        /// <summary>
        /// 获得日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private static IEnumerable<TestLog> GetRecentLogs(long userId, long ticks)
        {
            {
                if (_runtime.TryGetValue(userId, out var runner))
                {
                    return runner.GetLogs(ticks);
                }
            }

            {
                if (_over.TryGetValue(userId, out var runner))
                {
                    return runner.GetLogs(ticks);
                }
            }

            return null; //表示没有测试在运行
        }

        private static bool IsOver(long userId)
        {
            return _over.ContainsKey(userId);
        }

        #endregion

        private static void ClearExpired()
        {
            var overs = _over.Values.Where((t)=>
            {
                return t.IsExpired;
            });
            
            foreach(var over in overs)
            {
                _over.TryRemove(over.UserId, out var value);
            }
        }

        static TestRunner()
        {
            StartOver();
        }


        private static Timer _timer;

        private static void StartOver()
        {
            _timer = new Timer(60*60*1000); //1小时一次

            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            ClearExpired();
        }

    }

}
