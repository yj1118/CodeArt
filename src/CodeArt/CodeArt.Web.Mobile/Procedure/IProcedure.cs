using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;

namespace CodeArt.Web.Mobile
{
    public interface IProcedure
    {
        DTObject Invoke(DTObject arg);

        ICache GetCache();

    }
}
