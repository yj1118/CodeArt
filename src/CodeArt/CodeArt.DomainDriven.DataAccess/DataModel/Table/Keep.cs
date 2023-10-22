using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Collections;

using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.DTO;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        public void Keep(DomainObject obj)
        {
            if (obj.DataProxy is DataProxyPro) return; //已经赋予了持久层数据代理，不必再执行

            var data = GetData(obj);
            var dataProxy = obj.DataProxy;

            obj.DataProxy = new DataProxyPro(data, this, false);
            (obj.DataProxy as DataProxyPro).DeepCopy(dataProxy); //深度拷贝数据
        }

        private DynamicData GetData(DomainObject obj)
        {
            Type objectType = this.ObjectType;

            var tips = Util.GetPropertyTips(objectType);
            var data = new DynamicData(); //由于对象会被缓存，因此不从池中获取DynamicData
            foreach (var tip in tips)
            {
                CollectValue(obj, tip, data);
            }

            if (this.IsSessionEnabledMultiTenancy)
            {
                data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
            }

            return data;
        }

        private void CollectValue(DomainObject current, PropertyRepositoryAttribute tip, DynamicData data)
        {
            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.Primitive:
                    {
                        var value = GetPrimitivePropertyValue(current, tip);
                        data.Add(tip.PropertyName, value);
                    }
                    break;
                case DomainPropertyType.PrimitiveList:
                    {

                    }
                    break;
                case DomainPropertyType.ValueObject:
                    {
                        CollectValueObject(current, tip, data);
                    }
                    break;
                case DomainPropertyType.AggregateRoot:
                    {
                        var field = GetQuoteField(this, tip.PropertyName);
                        object obj = current.GetValue(tip.Property);
                        var id = GetObjectId(obj);
                        data.Add(field.Name, id);
                    }
                    break;
                case DomainPropertyType.EntityObject:
                    {
                        var obj = current.GetValue(tip.Property) as DomainObject;

                        var id = GetObjectId(obj);
                        var field = GetQuoteField(this, tip.PropertyName);
                        data.Add(field.Name, id);  //收集外键
                    }
                    break;
                case DomainPropertyType.AggregateRootList:
                    {

                    }
                    break;
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.EntityObjectList:
                    {
                        
                    }
                    break;
            }
        }

        private void CollectValueObject(DomainObject current, PropertyRepositoryAttribute tip, DynamicData data)
        {
            var field = GetQuoteField(this, tip.PropertyName);
            var obj = current.GetValue(tip.Property) as DomainObject;

            if (obj.IsEmpty())
            {
                data.Add(field.Name, Guid.Empty);
            }
            else
            {
                (obj as IValueObject).TrySetId(Guid.NewGuid());
                var id = GetObjectId(obj);
                data.Add(field.Name, id);
            }
        }


    }

}
