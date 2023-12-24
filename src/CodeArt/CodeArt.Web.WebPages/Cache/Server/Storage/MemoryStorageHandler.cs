using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeArt.Web.WebPages
{
    public class MemoryStorageHandler : WebPage
    {
        protected override void ProcessGET()
        {
            var code = this.GetQueryValue<string>("code", string.Empty);
            if (!string.IsNullOrEmpty(code))
                MemoryStorage.Instance.DeleteLocal(code);
        }
    }

}