using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using Microsoft.Build.Utilities;

[assembly: PreApplicationStart(typeof(CodeArt.CI.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.Low)]


namespace CodeArt.CI
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
        }
    }
}
