using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 唯一键计算器
    /// </summary>
    public static class UniqueKeyCalculator
    {
        public static string GetUniqueKey(DomainObject obj)
        {
            var objectType = obj.ObjectType;
            var root = obj as IAggregateRoot;
            if (root != null) return GetRootUniqueKey(objectType, root.GetIdentity());

            var eop = obj as IEntityObjectPro;
            if (eop != null) return GetEOPUniqueKey(objectType, eop.Root.GetIdentity(), eop.GetIdentity());

            throw new DomainDrivenException(string.Format(Strings.GetUniqueKeyError, objectType.FullName));
        }

        public static string GetRootUniqueKey(Type objectType, object id)
        {
            return string.Format("{0}+{1}", objectType.FullName, id.ToString());
        }

        public static string GetEOPUniqueKey(Type objectType, object rootId, object id)
        {
            return string.Format("{0}+{1}+{2}", objectType.FullName, rootId.ToString(), id.ToString());
        }

    }
}
