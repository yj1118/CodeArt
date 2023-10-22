using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.AppSetting
{
    public interface IAppDarkFilter
    {
        bool InDark(DTObject arg);
    }
}
