using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Log;

namespace CodeArt.Concurrent.Pattern
{
    public interface IPipelineAction : IDisposable
    {
        void AllowOne();

        bool IsWorking { get; }

    }
}
