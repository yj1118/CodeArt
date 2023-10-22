using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestPlatform
{
    public class TestContext
    {
        public bool Success
        {
            get;
            internal set;
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return _stopwatch.ElapsedMilliseconds;
            }
        }

        private Stopwatch _stopwatch;


        public TestContext()
        {
            this.Success = false;
            _stopwatch = new Stopwatch();
        }


        internal void Start()
        {
            _stopwatch.Start();
        }


        internal void Stop()
        {
            _stopwatch.Stop();
        }


        internal static TestContext Current = null;
    }
}
