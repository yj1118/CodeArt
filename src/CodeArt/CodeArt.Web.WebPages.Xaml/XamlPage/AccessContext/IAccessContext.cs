using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 访问时环境
    /// </summary>
    public interface IAccessContext
    {
        bool IsGET { get; }


        string VirtualPath { get; }

        bool IsMobileDevice { get; }


        T GetQueryValue<T>(string name, T defaultValue);
    }
}
