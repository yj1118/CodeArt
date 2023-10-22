using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public class EventArgAttribute : DTOMemberAttribute
    {
        public EventArgAttribute()
        {
        }
    }


    /// <summary>
    /// 表示这个领域参数是传递给调用方的
    /// </summary>
    public class EventReturnAttribute : EventArgAttribute
    {
        public EventReturnAttribute()
        {

        }
    }
}
