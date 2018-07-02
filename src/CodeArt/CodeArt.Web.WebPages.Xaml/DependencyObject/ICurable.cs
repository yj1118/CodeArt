using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface ICurable
    {
        /// <summary>
        /// 加载已固化值
        /// </summary>
        void LoadPinned();
    }
}
