using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using CodeArt.Web.WebPages;

[assembly: PreApplicationStartMethod(typeof(Module.WebUI.Xaml.PreApplicationStartCode), "Start")]

namespace Module.WebUI.Xaml
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                RegisterUploadRepository();
            }
        }

        private static void RegisterUploadRepository()
        {
            UploadRepositoryFactory.Register(UploadRepository.Instance);
        }

    }
}
