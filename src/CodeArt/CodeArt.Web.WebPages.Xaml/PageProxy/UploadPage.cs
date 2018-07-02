using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.AOP;

namespace CodeArt.Web.WebPages.Xaml
{
    [Aspect(typeof(UploadFileInitializer))]
    [Aspect(typeof(WebCrossDomain))]
    public abstract class UploadPage : PageProxy
    {
        protected override void ProcessPOST()
        {
            if (UploadContext.Current != null)
            {
                var files = UploadContext.Current.UploadedFiles;
                if (files.Count > 0)
                {
                    foreach (UploadedFile file in files)
                    {
                        Save(file);
                    }
                    return;
                }
            }
            base.ProcessPOST();
        }

        protected abstract void Save(UploadedFile file);

    }
}
