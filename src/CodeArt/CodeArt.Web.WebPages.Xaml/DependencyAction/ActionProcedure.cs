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
    /// 行为过程
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public delegate object ActionProcedure(DependencyObject obj, params object[] args);
}