using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.ModuleNest;
using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

namespace Module.WebUI
{
    [SafeAccess()]
    [ModuleHandler("menu.show")]
    public class MenuShow : ModuleHandlerBase
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var result = (dynamic)DTObject.Create();
            result.MenuCode = MenuHelper.GetMenuCode(Language.Current.Name, arg.MarkedCode, arg.IsLocal);
            return result;
        }
    }


}
