using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.TestTools
{
    public struct TestLog
    {
        public DTObject Content
        {
            get;
            private set;
        }

        public DateTime Time
        {
            get;
            private set;
        }

        public long Ticks
        {
            get;
            private set;
        }

        public TestLog(DTObject content, DateTime time, long ticks)
        {
            this.Content = content;
            this.Time = time;
            this.Ticks = ticks;
        }

    }
}
