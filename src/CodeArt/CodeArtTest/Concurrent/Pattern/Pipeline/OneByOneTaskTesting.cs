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
    public class OneByOneTaskTesting : PipelineActionBaseTesting
    {
        protected override int GetMaxTimes()
        {
            return 50;
        }

        public OneByOneTaskTesting()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        protected override IPipelineAction GetInstance(Action action)
        {
            return new OneByOnePipelineTask(action);
        }

    }
}
