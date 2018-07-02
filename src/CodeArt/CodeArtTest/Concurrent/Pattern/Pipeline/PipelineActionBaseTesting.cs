using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent.Pattern;
using System.Threading.Tasks;

namespace CodeArtTest.Concurrent
{
    /// <summary>
    /// ReaderWriterLock 的摘要说明
    /// </summary>
    [TestClass]
    public abstract class PipelineActionBaseTesting
    {
        public PipelineActionBaseTesting()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private int _count;
        /// <summary>
        /// 获取执行多少次
        /// </summary>
        /// <returns></returns>
        protected abstract int GetMaxTimes();

        /// <summary>
        /// 令通道对象并行allow max次，一共做maxTimes次这样的操作
        /// 操作的内容是将数据累加1
        /// </summary>
        [TestMethod]
        public void WriteMore()
        {
            var pipeline = GetInstance(BackgroudAction);
            const int max = 100;
            int maxTimes = GetMaxTimes();

            int currentTimes = 0;
            while (currentTimes < maxTimes)
            {
                _count = 0;

                Parallel.For(0, max, (i) =>
                {
                    pipeline.AllowOne();
                });

                while (pipeline.IsWorking)
                {
                    System.Threading.Thread.Sleep(5);
                }

                Assert.AreEqual(max, _count);
                currentTimes++;
            }
        }

        private void BackgroudAction()
        {
            _count++;
        }

        protected abstract IPipelineAction GetInstance(Action action);

    }
}
