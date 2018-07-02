using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Event
{
    public abstract class ValidEventArgs
    {
        ///// <summary>
        ///// 当前参数是否与<paramref name="target"/>重叠，
        ///// 相互重叠的参数不一定每个属性都相同，但是蕴含的意义都是指向的同一个对象
        ///// </summary>
        ///// <param name="target"></param>
        ///// <returns></returns>
        //public abstract bool IsOverlap(ValidEventArgs target);

        /// <summary>
        /// 应用一个新参数
        /// </summary>
        public abstract void Apply(IList<ValidEventArgs> args, ValidEventArgs newArg);

        /// <summary>
        /// 参数是否具有历史性，具有历史性的参数会在事件处理器挂载时执行
        /// </summary>
        public abstract bool IsHistoric
        {
            get;
        }

    }
}
