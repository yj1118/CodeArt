using System;
using System.Linq;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 对象断言
    /// </summary>
    public static class ObjectAssert
    {
        public static void IsNotNullOrEmpty(INotNullObject obj, string objectName)
        {
            if (obj == null) throw new ArgumentNullException(objectName);
            if (obj.IsEmpty()) throw new ArgumentException(objectName + " 不能为empty", objectName);
        }
        
    }
}
