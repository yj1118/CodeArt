using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.TestTools
{
    public struct TestRuntime
    {
        /// <summary>
        /// 最近的日志
        /// </summary>
        public IEnumerable<TestLog> Recent
        {
            get;
            private set;
        }

        /// <summary>
        /// 本次的时间戳
        /// </summary>
        public long Ticks
        {
            get;
            private set;
        }

        public bool IsOver
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get;
            private set;
        }

        internal TestRuntime(IEnumerable<TestLog> recent, long ticks, bool isOver)
        {
            this.Recent = recent;
            this.Ticks = ticks;
            this.IsOver = isOver;
            this.IsEmpty = false;
        }

        private TestRuntime(IEnumerable<TestLog> recent, long ticks, bool isOver,bool isEmpty)
        {
            this.Recent = recent;
            this.Ticks = ticks;
            this.IsOver = isOver;
            this.IsEmpty = isEmpty;
        }

        public DTObject ToDTO()
        {
            DTObject data = DTObject.Create();

            foreach (var log in this.Recent)
            {
                DTObject item = data.CreateAndPush("rows");

                item["status"] = log.Content.GetValue<int>("status", 0);
                item["message"] = log.Content.GetValue<string>("message");
                item["time"] = log.Time.ToString("MM/dd HH:mm:ss");
            }

            if (this.Ticks > 0)
            {
                data.Dynamic.ticks = this.Ticks.ToString(); //为了兼容JS，所以转为字符串
            }

            data.Dynamic.isOver = this.IsOver;

            System.Diagnostics.Debug.WriteLine(data.GetCode());

            return data;
        }

        internal static TestRuntime CreateEmpty()
        {
            return new TestRuntime(Array.Empty<TestLog>(), 0L, false, true);
        }

    }
}
