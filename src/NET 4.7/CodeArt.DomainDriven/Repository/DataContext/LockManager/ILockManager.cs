using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface ILockManager
    {
        /// <summary>
        /// 以不会造成死锁的形式锁定指定的聚合根对象
        /// </summary>
        /// <param name="roots"></param>
        void Lock(IEnumerable<IAggregateRoot> roots);
    }
}
