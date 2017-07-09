using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    internal static class LockManager
    {
        public static void Lock(IEnumerable<IAggregateRoot> roots)
        {
            if (_manager == null)
                throw new DomainDrivenException(Strings.NotExistLockManager);
            _manager.Lock(roots);
        }

        private static ILockManager _manager;

        public static void Register(ILockManager manager)
        {
            _manager = manager;
        }
    }
}
