using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object CalledActionCallback(DependencyObject obj, object[] arg, object returnValue);
}
