using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;

namespace CodeArt.Web.Mobile
{
    public abstract class Procedure : IProcedure
    {
        public virtual DTObject Invoke(DTObject arg)
        {
            return InvokeDynamic(arg);
        }

        protected virtual DTObject InvokeDynamic(dynamic arg)
        {
            return DTObject.Empty;
        }

        /// <summary>
        /// 默认情况下不适用缓存功能
        /// </summary>
        /// <returns></returns>
        public virtual ICache GetCache()
        {
            return null;
        }
    }
}
