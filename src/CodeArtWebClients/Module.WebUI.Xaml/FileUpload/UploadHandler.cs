using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web;
using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Util;
using CodeArt.Web.WebPages;
using CodeArt.ServiceModel;
using Module.File;

namespace Module.WebUI.Xaml.Pages
{
    public class UploadHandler : UploadPage
    {
        public UploadHandler()
        {
        }

        protected override void Save(UploadedFile file)
        {
            var size = file.ContentLength;
            var storeKey = file.ServerKey;
            var name = file.GetName();
            var extension = file.GetExtension();
            int fileSize = GetFileSize(); //这是用来效验大小的，如果这个值比size高，那么证明文件被取消了

            if(fileSize > 0 && size != fileSize)
            {
                //如果指定了fileSize效验长度，并且size与fileSize不同那么取消
                Delete(storeKey);
                return;
            }

            try
            {
                var directoryId = GetDirectoryId();
                var result = ServiceContext.Invoke("addFile", (arg) =>
                {
                    arg["name"] = name;
                    arg["storeKey"] = storeKey;
                    arg["extension"] = extension;
                    arg["size"] = size;
                    arg["directoryId"] = directoryId;
                });
                this.Response.Write(result.GetCode(false, false));
            }
            catch (Exception ex)
            {
                Delete(storeKey);
                throw ex;
            }

        }


        private void Delete(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            FileStorage.Instance.Delete(key);

            ServiceContext.Invoke("deleteFile", (arg) =>
            {
                arg["storeKey"] = key;
            });
        }

        private Guid GetDirectoryId()
        {
            var directoryId = this.Request.Headers["directoryId"];
            if (directoryId == null) throw new WebException(Strings.UploadNoDirectory);
            return Guid.Parse(directoryId);
        }

        private int GetFileSize()
        {
            var fileSize = this.Request.Headers["fileSize"];
            if (fileSize == null) return 0;
            return int.Parse(fileSize);
        }

    }
}
   