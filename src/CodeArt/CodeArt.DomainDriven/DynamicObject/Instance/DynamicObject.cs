using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Runtime;


namespace CodeArt.DomainDriven
{
    [ObjectRepository(typeof(IDynamicRepository))]
    public class DynamicObject : DomainObject, IDynamicObject
    {
        /// <summary>
        /// 动态类型的定义
        /// </summary>
        public TypeDefine Define
        {
            get;
            private set;
        }

        protected override Type GetObjectType()
        {
            return this.Define.MetadataType;
        }

        private bool _isEmpty;


        public DynamicObject(TypeDefine define, bool isEmpty)
        {
            this.Define = define;
            _isEmpty = isEmpty;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public DynamicObject()
            : this(null, false)
        {
        }

        public override bool IsEmpty()
        {
            return _isEmpty;
        }

        #region 从dto中加载数据

        /// <summary>
        /// 从dto中加载数据
        /// </summary>
        /// <param name="data"></param>
        public void Load(DTObject data)
        {
            var properties = this.Define.Properties;
            foreach (var property in properties)
            {
                var value = data.GetValue(property.Name, false);
                if (value == null) continue;

                var obj = value as DTObject;
                if (obj != null)
                {
                    this.SetValue(property, GetObjectValue(property, obj));
                    continue;
                }

                var objs = value as DTObjects;
                if (objs != null)
                {
                    this.SetValue(property, GetListValue(property, objs));
                    continue;
                }
                this.SetValue(property, GetPrimitiveValue(property, value));
            }
        }

        public DTObject GetData()
        {
            var data = DTObject.Create();
            var properties = this.Define.Properties;
            foreach (var property in properties)
            {
                var value = this.GetValue(property);
                var obj = value as DynamicObject;
                if (obj != null)
                {
                    value = obj.GetData();  //对象
                    data.SetValue(property.Name, value);
                    continue;
                }

                var list = value as IEnumerable<DynamicObject>;
                if (list != null)
                {
                    //集合
                    data.Push(property.Name, list, (item) =>
                     {
                         return item.GetData();
                     });
                    continue;
                }

                data.SetValue(property.Name, value); //值
            }
            return data;
        }

        private object GetObjectValue(DomainProperty property, DTObject value)
        {
            switch (property.DomainPropertyType)
            {
                case DomainPropertyType.AggregateRoot:
                case DomainPropertyType.EntityObject:
                case DomainPropertyType.ValueObject:
                    {
                        var objType = property.PropertyType as RuntimeObjectType;
                        if (objType == null) throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
                        var objDefine = objType.Define;
                        DynamicObject obj = objDefine.CreateInstance(value);
                        return obj;
                    }
            }
            throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
        }

        private object GetListValue(DomainProperty property, DTObjects values)
        {
            IList list = property.PropertyType.CreateInstance() as IList;
            if (list == null) throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));

            switch (property.DomainPropertyType)
            {
                case DomainPropertyType.AggregateRootList:
                case DomainPropertyType.EntityObjectList:
                case DomainPropertyType.ValueObjectList:
                    {
                        var elementType = property.DynamicType as RuntimeObjectType;
                        if (elementType == null) throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
                        var objDefine = elementType.Define;
                        foreach (DTObject value in values)
                        {
                            DynamicObject obj = objDefine.CreateInstance(value);
                            list.Add(obj);
                        }
                        return list;
                    }
                case DomainPropertyType.PrimitiveList:
                    {
                        foreach (DTObject value in values)
                        {
                            if (!value.IsSingleValue)
                                throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
                            list.Add(value.GetValue());
                        }
                        return list;
                    }
            }
            throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
        }


        private object GetPrimitiveValue(DomainProperty property, object value)
        {
            if (property.DomainPropertyType == DomainPropertyType.Primitive)
            {
                return DataUtil.ToValue(value,property.PropertyType);
            }
            throw new DomainDrivenException(string.Format(Strings.DynamicObjectLoadError, this.Define.TypeName));
        }

        #endregion

        #region 同步对象

        /// <summary>
        /// 将对象<paramref name="target"/>的数据同步到当前对象中
        /// </summary>
        /// <param name="target"></param>
        internal void Sync(DynamicObject target)
        {
            var properties = this.Define.Properties;
            foreach (var property in properties)
            {
                var value = target.GetValue(property);
                this.SetValue(property, value);
            }
        }


        /// <summary>
        /// 标记为快照
        /// </summary>
        internal void MarkSnapshot()
        {
            this.SetValue(this.Define.SnapshotProperty, true);
        }

        #endregion

        #region 获取成员根对象

        /// <summary>
        /// 从dto中加载数据
        /// </summary>
        /// <param name="data"></param>
        public IEnumerable<DynamicRoot> GetRoots()
        {
            List<DynamicRoot> roots = new List<DynamicRoot>();
            var properties = this.Define.Properties;
            foreach (var property in properties)
            {
                switch (property.DomainPropertyType)
                {
                    case DomainPropertyType.AggregateRoot:
                        {
                            var value = this.GetValue(property);
                            var root = (DynamicRoot)value;
                            if(!root.IsEmpty())
                            {
                                roots.Add(root);
                                roots.AddRange(root.GetRoots());
                            }
                        }
                        break;
                    case DomainPropertyType.EntityObject:
                    case DomainPropertyType.ValueObject:
                        {
                            var value = this.GetValue(property);
                            var obj = (DynamicObject)value;
                            if (!obj.IsEmpty())
                            {
                                roots.AddRange(obj.GetRoots());
                            }
                        }
                        break;
                    case DomainPropertyType.AggregateRootList:
                        {
                            var list = (IEnumerable)this.GetValue(property);
                            foreach (DynamicRoot root in list)
                            {
                                if (!root.IsEmpty())
                                {
                                    roots.Add(root);
                                    roots.AddRange(root.GetRoots());
                                }
                            }
                        }
                        break;
                    case DomainPropertyType.EntityObjectList:
                    case DomainPropertyType.ValueObjectList:
                        {
                            var list = (IEnumerable)this.GetValue(property);
                            foreach(DynamicObject obj in list)
                            {
                                if (!obj.IsEmpty())
                                {
                                    roots.AddRange(obj.GetRoots());
                                }
                            }
                        }
                        break;
                }
            }
            return roots;
        }

        #endregion

        #region 动态支持

        /// <summary>  
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var propertyName = binder.Name;
            var property = this.Define.GetProperty(propertyName);
            if (property == null) throw new DomainDrivenException(string.Format(Strings.NoDefinitionProperty, this.Define.TypeName, propertyName));
            result = GetValue(property);
            return true;
        }

        /// <summary>  
        /// 实现动态对象属性值设置的方法。  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            var propertyName = binder.Name;
            var property = this.Define.GetProperty(propertyName);
            if (property == null) throw new DomainDrivenException(string.Format(Strings.NoDefinitionProperty, this.Define.TypeName, propertyName));
            SetValue(property, value);
            return true;
        }


        public override bool TryConvert(System.Dynamic.ConvertBinder binder, out object result)
        {
            result = this;
            return true;
        }


        #endregion
    }
}