using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 数据上下文发生回滚时的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RolledBackEventHandler(object sender, RolledBackEventArgs e);
}
