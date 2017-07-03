using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 仓储事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RepositoryEventHandler(object sender, RepositoryEventArgs e);
}
