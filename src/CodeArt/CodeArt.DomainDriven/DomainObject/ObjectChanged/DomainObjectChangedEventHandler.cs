using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public delegate void DomainObjectChangedEventHandler(object sender, DomainObjectChangedEventArgs e);

    /// <summary>
    /// 不带参数e的轻量级事件处理器
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DomainObjectChangedSlimEventHandler(object sender);
}