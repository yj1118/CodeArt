using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 领域舞台，为领域驱动测试提供可实施的环境
    /// 每一个测试单元就是舞台中的一个场景，场景有多种事件方法可以使用
    /// </summary>
    public class DomainStage : UnitTestStage
    {
        #region 事务

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="isTimely">是否开启即时事务</param>
        protected void BeginTransaction(bool isTimely = false)
        {
            DataContext.Current.BeginTransaction();
            if (isTimely)
            {
                DataContext.Current.OpenTimelyMode();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        protected void Commit()
        {
            DataContext.Current.Commit();
        }

        #endregion
    }
}
