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
    [TestClass]
    public class AutoResetTaskTesting : PipelineActionBaseTesting
    {
        protected override int GetMaxTimes()
        {
            return 1000;
        }

        public AutoResetTaskTesting()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        protected override IPipelineAction GetInstance(Action action)
        {
            return new AutoResetPipelineTask(action);
        }

    }
}
