using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

[assembly: PreApplicationStart(typeof(CodeArt.Web.PreApplicationStart), "Initialize", PreApplicationStartPriority.Height)]

namespace CodeArt.Web
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            AppSession.Register(CombineAppSession.Instance);
        }
    }
}
