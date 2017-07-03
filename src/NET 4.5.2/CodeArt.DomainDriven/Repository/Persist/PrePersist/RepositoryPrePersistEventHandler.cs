using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 仓储在实际执行CUD操作之前触发的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RepositoryPrePersistEventHandler(object sender, RepositoryPrePersistEventArgs e);
}
