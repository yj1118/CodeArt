using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.TestTools
{
    public interface ITestLog
    {
        void Write(long userId,long batchId, DTObject content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="batchId"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        IEnumerable<TestLog> Read(long userId, long batchId, long ticks);
    }
}
