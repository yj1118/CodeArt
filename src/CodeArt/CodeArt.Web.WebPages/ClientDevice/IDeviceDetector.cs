using System;
using System.Web;
using System.Text;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceDetector
    {
        ClientDevice Detect(WebPageContext context);
    }
}
