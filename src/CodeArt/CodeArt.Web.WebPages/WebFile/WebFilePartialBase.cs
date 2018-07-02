using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 适用于视频等文件分批下载、断点下载
    /// </summary>
    //[AspectRemove(typeof(WebPageInitializer))]
    //public abstract class WebFilePartialBase : WebPage
    //{
    //    protected override void ProcessGET()
    //    {
    //        Stream content = LoadFile();
    //        var range = HttpRequestRange.New(this.Request);
    //        range.Process(this.Response, content);
    //    }

    //    protected abstract Stream LoadFile();
    //}
}
