using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


using CodeArt.AppSetting;


[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.PreApplicationStart), "Initialize")]

namespace CodeArt.Web
{
    public class PreApplicationStart
    {
        public static void Initialize()
        {
            AppInitializer.Initialize();
            AppSession.Register(CombineAppSession.Instance);
        }
    }
}
