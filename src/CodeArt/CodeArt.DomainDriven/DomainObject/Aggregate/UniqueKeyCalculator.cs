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
            if (root != null) return GetUniqueKey(objectType, root.GetIdentity());

            throw new DomainDrivenException(string.Format(Strings.GetUniqueKeyError, objectType.FullName));
        }

        public static string GetUniqueKey(Type objectType, object id)
        {
            return string.Format("{0}+{1}", objectType.Name, id.ToString());
        }
    }
}
