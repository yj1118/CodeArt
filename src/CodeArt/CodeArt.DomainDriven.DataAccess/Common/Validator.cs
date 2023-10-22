using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 使用数据访问层提供的常用验证方法，这些方法会通过数据门户获取数据，并在领域层验证规则数据
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// 常用于根据某一项数据的值，判断对象是否已经存在
        /// 例如：在注册和修改账户对象时时，账户的的名称属性是不能重复的,
        /// 该方法仅用于判断字符串类型的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="findByValue">该方法是用仓储根据属性的值加载对象，请指定QueryLevel.HoldSingle锁，来避免并发冲突</param>
        /// <returns></returns>
        private static bool IsPropertyRepeated<T>(T obj, DomainProperty property, out object value) where T : class, IAggregateRoot
        {
            value = null;
            if (!obj.IsPropertyDirty(property)) return false;
            var propertyValue = obj.GetPropertyValue(property.Name);

            var stringValue = propertyValue as string;
            if (stringValue != null)
            {
                if(string.IsNullOrEmpty(stringValue))
                    return false;
            }
            else
            {
                if (DataUtil.IsDefaultValue(propertyValue)) return false;
            }

            var exp = _getPropertyNameCondition(property.Name);
            var target = DataContext.Current.QuerySingle<T>(exp, (data) =>
            {
                data.Add(property.Name, propertyValue);
            }, QueryLevel.HoldSingle);

            value = propertyValue;
            if (target.IsEmpty()) return false;  //如果没有找到，那么没有重复
            if (target.Equals(obj)) return false; //如果找到了但是跟obj一样，那么也不算重复
            return true;
        }

        private static Func<string, string> _getPropertyNameCondition = LazyIndexer.Init<string, string>((propertyName) =>
        {
            return string.Format("{0}=@{0}", propertyName);
        });

        /// <summary>
        /// 验证属性值是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static void CheckPropertyRepeated<T>(T obj, string propertyName, ValidationResult result) where T : class, IAggregateRoot
        {
            var property = DomainProperty.GetProperty(typeof(T), propertyName);
            CheckPropertyRepeated(obj, property, result);
        }

        /// <summary>
        /// 验证属性值是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static void CheckPropertyRepeated<T>(T obj, DomainProperty property, ValidationResult result) where T : class, IAggregateRoot
        {
            object propertyValue = null;
            if (IsPropertyRepeated(obj, property, out propertyValue))
            {
                var code = _getProppertyRepeatedErrorCode(property);
                result.AddError(code, string.Format(Strings.PropertyValueRepeated, property.Call, propertyValue));
                return;
            }
        }

        private static Func<DomainProperty, string> _getProppertyRepeatedErrorCode = LazyIndexer.Init<DomainProperty, string>((property) =>
        {
            return string.Format("{0}.{1}Repeated", property.OwnerType.Name, property.Name);
        });
    }
}
