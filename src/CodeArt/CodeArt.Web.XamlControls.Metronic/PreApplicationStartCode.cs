using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using System.IO;
using CodeArt.Web.WebPages;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.XamlControls.Metronic.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.XamlControls.Metronic
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                WebMethodSupporterFactory.RegisterExtractor(MetronicExtractor.Instance);
            }
        }
    }
}
