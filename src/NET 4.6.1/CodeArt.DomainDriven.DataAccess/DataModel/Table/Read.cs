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
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        /// <summary>
        /// 1对多的引用关系的读取
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private object ReadOneToMore(ParameterRepositoryAttribute prmTip, object parent, object rootId, object masterId)
        {
            using (var temp = SqlHelper.BorrowDatas())
            {
                var datas = temp.Item;
                QueryRootAndSlaveIds(rootId, masterId, datas);

                Type implementType = null;
                if (parent == null)
                {
                    //说明还在构造阶段,或者是内部调用
                    implementType = prmTip?.ImplementType ?? this.Middle.ObjectType; //middle表对应的是属性的基类类型
                }
                else
                {
                    implementType = this.Middle.ObjectType; //middle表对应的是属性的基类类型
                }
                var list = CreateList(implementType);
                var elementType = this.Middle.ElementType;

                if (this.Type == DataTableType.AggregateRoot)
                {
                    //引用了多个外部内聚根
                    var model = DataModel.Create(elementType);
                    var root = model.Root;
                    var slaveIdName = root.TableIdName;
                    var slaveTip = root.ObjectTip;

                    if (slaveTip.Snapshot)  //如果外部内聚根是有快照的，那么当数据不存在时，加载快照
                    {
                        foreach (var data in datas)
                        {
                            var slaveId = data.Get(slaveIdName);
                            var item = root.QuerySingle(slaveId, QueryLevel.None);
                            if (((IDomainObject)item).IsEmpty())
                            {
                                //加载快照
                                item = model.Snapshot.QuerySingle(slaveId, QueryLevel.None);
                            }
                            list.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var data in datas)
                        {
                            var slaveId = data.Get(slaveIdName);
                            var item = root.QuerySingle(slaveId, QueryLevel.None);
                            list.Add(item);
                        }
                    }
                }
                else
                {
                    var slaveIdName = this.TableIdName;
                    foreach (var data in datas)
                    {
                        var slaveId = data.Get(slaveIdName);
                        var item = this.QuerySingle(rootId, slaveId);
                        if (!((IDomainObject)item).IsEmpty())
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        private IList CreateList(Type listType)
        {
            if (_isDomainCollection(listType))
            {
                var constructor = _getDomainCollectionConstructor(listType);
                using (var temp = ArgsPool.Borrow2())
                {
                    var args = temp.Item;
                    args[0] = this.MemberDomainProperty;
                    return constructor.CreateInstance(args) as IList;
                }
            }
            return listType.CreateInstance() as IList;
        }

        private static Func<Type, bool> _isDomainCollection = LazyIndexer.Init<Type, bool>((type) =>
        {
            return type.IsImplementOrEquals(typeof(DomainCollection<>));
        });


        private static Func<Type, ConstructorInfo> _getDomainCollectionConstructor = LazyIndexer.Init<Type, ConstructorInfo>((type) =>
        {
            return type.GetConstructor(new Type[] { typeof(DomainProperty) });
        });

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private DomainObject CreateObject(Type objectType, DynamicData data)
        {
            DomainObject obj = null;

            if (this.IsDynamic)
            {
                if (data.IsEmpty()) obj = (DomainObject)DomainObject.GetEmpty(this.ObjectType);
                else
                {
                    obj = CreateObjectImpl(this.DynamicType, this.DynamicType.ObjectType, data);
                    (obj as IDynamicObject).Define = this.DynamicType.Define;
                }
            }
            else
            {
                if (data.IsEmpty()) obj = (DomainObject)DomainObject.GetEmpty(objectType);
                else
                {
                    obj = CreateObjectImpl(objectType, objectType, data);
                }
            }
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defineType">如果是动态领域对象，那么该类型为定义领域属性的类型（也就是定义类型），否则是对象的实际类型</param>
        /// <param name="objectType">实际存在内存中的实际类型</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private DomainObject CreateObjectImpl(Type defineType, Type objectType, DynamicData data)
        {
            //构造对象
            var obj = ConstructObject(objectType, data);

            //为了避免死循环，我们先将对象加入到构造上下文中
            AddToConstructContext(obj, data);

            //加载属性
            LoadProperties(defineType, data, obj);

            RemoveFromConstructContext(obj);

            //补充信息
            Supplement(obj, data);
            return obj;
        }


        private void AddToConstructContext(DomainObject obj, DynamicData data)
        {
            object id = data.Get(EntityObject.IdPropertyName);
            if (this.Type == DataTableType.AggregateRoot)
            {
                ConstructContext.Add(id, obj);
            }
            else
            {
                object rootId = data.Get(this.Root.TableIdName);
                ConstructContext.Add(rootId, id, obj);
            }
        }

        private void RemoveFromConstructContext(DomainObject obj)
        {
            ConstructContext.Remove(obj);
        }


        private DomainObject ConstructObject(Type objectType, DynamicData data)
        {
            var constructorTip = ConstructorRepositoryAttribute.GetTip(objectType);
            var constructor = constructorTip.Constructor;
            var args = CreateArguments(constructorTip, data);
            return (DomainObject)constructor.CreateInstance(args);
        }

        /// <summary>
        /// 加载属性
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        private void LoadProperties(Type objectType, DynamicData data, DomainObject obj)
        {
            var propertyTips = Util.GetPropertyAllTips(objectType);  //此处不必考虑是否为派生类，直接赋值所有属性
            foreach (var propertyTip in propertyTips)
            {
                //只有是可以公开设置的属性和不是延迟加载的属性我们才会主动赋值
                //有些属性是私有设置的，这种属性有可能是仅获取外部的数据而不需要赋值的
                if (propertyTip.IsPublicSet && !propertyTip.Lazy)
                {
                    var value = ReadPropertyValue(obj, propertyTip, null, data); //已不是构造，所以不需要prmTip参数
                    if (value == null)
                        throw new DataAccessException(string.Format(Strings.LoadPropertyError, propertyTip.Path));
                    obj.SetValue(propertyTip.Property, value);
                }
            }
        }

        private void SetDataProxy(DomainObject obj, DynamicData data)
        {
            //设置代理对象
            obj.DataProxy = new DataProxyPro(data, this);
        }

        private void Supplement(DomainObject obj, DynamicData data)
        {
            //设置代理对象
            SetDataProxy(obj, data);

            var valueObject = obj as ValueObject;
            if (valueObject != null)
            {
                object id = null;
                if (data.TryGetValue(EntityObject.IdPropertyName, out id))
                {
                    valueObject.SetId((Guid)id);
                }
            }

            obj.MarkClean(); //对象从数据库中读取，是干净的
        }

        private object[] CreateArguments(ConstructorRepositoryAttribute tips, DynamicData data)
        {
            var length = tips.Parameters.Count();
            if (length == 0) return Array.Empty<object>();
            object[] args = new object[length];
            foreach (var prm in tips.Parameters)
            {
                var arg = CreateArgument(prm, data);
                args[prm.Index] = arg;
            }
            return args;
        }

        private object CreateArgument(ConstructorRepositoryAttribute.ConstructorParameterInfo prm, DynamicData data)
        {
            object value = null;
            //看构造特性中是否定义了加载方法
            if (prm.TryLoadData(this.ObjectType, data, out value))
            {
                return value;
            }
            var tip = prm.PropertyTip;
            if (tip == null) throw new DataAccessException(Strings.ConstructionParameterNoProperty);

            //从属性定义中加载
            value = ReadPropertyValue(null, tip, prm.Tip, data); //在构造时，还没有产生对象，所以parent为 null
            if (value == null)
                throw new DataAccessException(string.Format(Strings.ConstructionParameterError, prm.DeclaringType.FullName, prm.Name));
            return value;
        }

        public object ReadPropertyValue(object parent, PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DynamicData data)
        {
            //看对应的属性特性中是否定义了加载方法，优先执行自定义方法
            object value = null;
            if (tip.TryLoadData(this.ObjectType, data, out value))
            {
                return value;
            }

            //自动加载
            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.Primitive:
                    {
                        return ReadPrimitive(tip, data);
                    }
                case DomainPropertyType.PrimitiveList:
                    {
                        return ReadPrimitiveList(tip, data);
                    }
                case DomainPropertyType.AggregateRoot:
                    {
                        return ReadAggregateRoot(tip, data);
                    }
                case DomainPropertyType.ValueObject:
                case DomainPropertyType.EntityObject:
                case DomainPropertyType.EntityObjectPro:
                    {
                        return ReadMember(tip, data);
                    }
                case DomainPropertyType.EntityObjectList:
                case DomainPropertyType.EntityObjectProList:
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.AggregateRootList:
                    {
                        return ReadMembers(parent, tip, prmTip, data);
                    }
            }
            return null;
        }

        #region 读取基础的值数据

        private object ReadPrimitive(PropertyRepositoryAttribute tip, DynamicData data)
        {
            var value = tip.Lazy ? ReadValueByLazy(tip, data) : ReadValueFromData(tip, data);
            if (!tip.IsEmptyable) return value;
            if (value == null)
            {
                //Emptyable类型的数据有可能存的是null值
                return tip.CreateDefaultEmptyable.Invoke(null, null);
            }
            using (var temp = ArgsPool.Borrow1())
            {
                var args = temp.Item;
                args[0] = value;
                return tip.EmptyableConstructor.CreateInstance(args);
            }
        }

        private object ReadValueFromData(PropertyRepositoryAttribute tip, DynamicData data)
        {
            object value = null;
            if (data.TryGetValue(tip.PropertyName, out value)) return value;
            return null;
        }

        private object ReadValueByLazy(PropertyRepositoryAttribute tip, DynamicData data)
        {
            object id = null;
            if (data.TryGetValue(EntityObject.IdPropertyName, out id))
            {
                object rootId = null;
                var rootIdName = this.Type == DataTableType.AggregateRoot
                                                ? EntityObject.IdPropertyName
                                                : this.Root.TableIdName;
                if (data.TryGetValue(rootIdName, out rootId))
                {
                    return QueryDataScalar(rootId, id, tip.PropertyName);
                }
            }
            return null;
        }

        #endregion

        #region 读取基础值的集合数据

        private object ReadPrimitiveList(PropertyRepositoryAttribute tip, DynamicData data)
        {
            return tip.Lazy ? ReadValueListByLazy(tip, data) : ReadValueListFromData(tip, data);
        }


        private object ReadValueListFromData(PropertyRepositoryAttribute tip, DynamicData data)
        {
            object value = null;
            if (data.TryGetValue(tip.PropertyName, out value))
            {
                return ParseValueList(tip, value);
            }
            return null;
        }

        private object ParseValueList(PropertyRepositoryAttribute tip, object value)
        {
            var propertyType = tip.PropertyType;
            var elementType = propertyType.ResolveElementType();
            IEnumerable temp = null;
            if(tip.IsEmptyable)
            {
                temp = value.ToString().Split(',').Select((item) =>
                {
                    var itemValue = DataUtil.ToValue(item.FromBase64(), tip.EmptyableValueType);

                    object emptyableValue = null;
                    using (var argsTemp = ArgsPool.Borrow1())
                    {
                        var args = argsTemp.Item;
                        args[0] = itemValue;
                        emptyableValue = tip.EmptyableConstructor.CreateInstance(args);
                    }
                    return emptyableValue;
                });
            }
            else
            {
                temp = value.ToString().Split(',').Select((item) =>
                {
                    return DataUtil.ToValue(item.FromBase64(), elementType);
                });
            }

            var list = CreateList(propertyType);
            foreach (var t in temp)
                list.Add(t);
            return list;
        }


        private object ReadValueListByLazy(PropertyRepositoryAttribute tip, DynamicData data)
        {
            object id = null;
            if (data.TryGetValue(EntityObject.IdPropertyName, out id))
            {
                object rootId = null;
                var rootIdName = this.Type == DataTableType.AggregateRoot
                                                ? EntityObject.IdPropertyName
                                                : this.Root.TableIdName;
                if (data.TryGetValue(rootIdName, out rootId))
                {
                    var value = QueryDataScalar(rootId, id, tip.PropertyName);
                    return ParseValueList(tip, value);
                }
            }
            return null;
        }

        #endregion

        private object ReadAggregateRoot(PropertyRepositoryAttribute tip, DynamicData data)
        {
            var dataKey = _getIdName(tip.PropertyName);

            object id = null;
            if (data.TryGetValue(dataKey, out id))
            {
                var model = DataModel.Create(tip.PropertyType);
                return model.Root.QuerySingle(id, QueryLevel.None); //通过属性读取外部根，我们都是无锁的查询方式
            }
            return null;
        }

        private object ReadMember(PropertyRepositoryAttribute tip, DynamicData data)
        {
            return tip.Lazy ? ReadMemberByLazy(tip, data) : ReadMemberFromData(tip, data);
        }

        private object ReadMemberFromData(PropertyRepositoryAttribute tip, DynamicData data)
        {
            var name = _getNameWithSeparated(tip.PropertyName);
            using (var temp = SqlHelper.BorrowData())
            {
                var subData = temp.Item;
                foreach (var p in data)
                {
                    var dataName = p.Key;
                    if (dataName.StartsWith(name))
                    {
                        var subName = _getNextName(dataName);
                        subData.Add(subName, p.Value);
                    }
                }

                if (subData.IsEmpty())
                {
                    if(tip.DomainPropertyType == DomainPropertyType.AggregateRoot)
                    {
                        var idName = _getIdName(tip.PropertyName);
                        var id = data.Get(idName);
                        return ReadSnapshot(tip, id);
                    }
                    return DomainObject.GetEmpty(tip.PropertyType);
                }
                
                
                var typeKey = (string)subData.Get(GeneratedField.TypeKeyName);
                var objectType = string.IsNullOrEmpty(typeKey) ? tip.PropertyType : DerivedClassAttribute.GetDerivedType(typeKey);
         
                var child = GetRuntimeTable(this, tip.PropertyName, objectType);
                //先尝试中构造上下文中得到
                return  child.GetObjectFromConstruct(subData) ?? child.CreateObject(objectType, subData);
            }
        }

        private object ReadMemberByLazy(PropertyRepositoryAttribute tip, DynamicData data)
        {
            var child = GetChildTableByRuntime(this, tip);
            var dataKey = _getIdName(tip.PropertyName);

            object id = null;
            if (data.TryGetValue(dataKey, out id))
            {
                object rootId = null;
                var rootIdName = this.Type == DataTableType.AggregateRoot
                                                ? EntityObject.IdPropertyName
                                                : this.Root.TableIdName;
                if (data.TryGetValue(rootIdName, out rootId))
                {
                    var member = child.QuerySingle(rootId, id);
                    if(tip.DomainPropertyType == DomainPropertyType.AggregateRoot)
                    {
                        //尝试加载快照
                        if (member.IsNull())
                            return ReadSnapshot(tip, id);
                    }
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private object ReadSnapshot(PropertyRepositoryAttribute tip, object id)
        {
            var objectTip = ObjectRepositoryAttribute.GetTip(tip.PropertyType, true);
            if (objectTip.Snapshot)  //如果外部内聚根是有快照的，那么当数据不存在时，加载快照
            {
                var idName = _getIdName(tip.PropertyName);
                var root = DataModel.Create(objectTip.ObjectType).Root;
                return root.QuerySingle(id, QueryLevel.None);
            }
            return DomainObject.GetEmpty(tip.PropertyType);
        }

        private object ReadMembers(object parent, PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DynamicData data)
        {
            object rootId = null;
            var rootIdName = this.Type == DataTableType.AggregateRoot
                                            ? EntityObject.IdPropertyName
                                            : this.Root.TableIdName;
            if (data.TryGetValue(rootIdName, out rootId))
            {
                //当前对象的编号，就是子对象的masterId
                object masterId = null;
                data.TryGetValue(EntityObject.IdPropertyName, out masterId);

                var child = GetChildTableByRuntime(this, tip);
                return child.ReadOneToMore(prmTip, parent, rootId, masterId);
            }
            return null;
        }

        /// <summary>
        /// 读取对象集合的内部调用版本，此方法不是用于构造对象，而是为了查询用
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="data"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        private void QueryMembers(PropertyRepositoryAttribute tip, DynamicData data, List<object> objs)
        {
            object rootId = null;
            var rootIdName = this.Type == DataTableType.AggregateRoot
                                            ? EntityObject.IdPropertyName
                                            : this.Root.TableIdName;
            if (data.TryGetValue(rootIdName, out rootId))
            {
                //当前对象的编号，就是子对象的masterId
                object masterId = null;
                data.TryGetValue(EntityObject.IdPropertyName, out masterId);

                var child = GetChildTableByRuntime(this, tip);
                child.QueryOneToMore(rootId, masterId, objs);
            }
        }

        private void QueryOneToMore(object rootId, object masterId, List<object> objs)
        {
            using (var temp = SqlHelper.BorrowDatas())
            {
                var datas = temp.Item;
                QueryRootAndSlaveIds(rootId, masterId, datas);

                var slaveIdName = this.TableIdName;
                foreach (var data in datas)
                {
                    var slaveId = data.Get(slaveIdName);
                    var item = this.QuerySingle(rootId, slaveId);
                    objs.Add(item);
                }
            }
        }

        private object QueryMember(PropertyRepositoryAttribute tip, DynamicData data)
        {
            var child = GetChildTableByRuntime(this, tip); //通过基本表就可以实际查出数据，查数据会自动识别数据的真实类型的

            object rootId = null;
            var rootIdName = this.Type == DataTableType.AggregateRoot
                                            ? EntityObject.IdPropertyName
                                            : this.Root.TableIdName;
            if (data.TryGetValue(rootIdName, out rootId))
            {
                object id = null;

                var field = GetQuoteField(this, tip.PropertyName);

                if (data.TryGetValue(field.Name, out id))
                {
                    return child.QuerySingle(rootId, id);
                }
            }
            return null;
        }

        #region 查询数据

        /// <summary>
        /// 查询单值数据，不必缓存，因此延迟就在后就被加载到内存中已被缓存的对象了
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object QueryDataScalar(object rootId, object id, string propertyName)
        {
            var query = GetScalarByIdExpression(this, propertyName);
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                if (this.Type != DataTableType.AggregateRoot)
                {
                    param.Add(this.Root.TableIdName, rootId);
                }
                param.Add(EntityObject.IdPropertyName, id);

                var sql = query.Build(param);
                return SqlHelper.ExecuteScalar(this.ConnectionName, sql, param);
            }
        }


        /// <summary>
        /// 查询1对多引用的成员数据
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="masterId"></param>
        /// <param name="datas"></param>
        private void QueryRootAndSlaveIds(object rootId, object masterId, List<DynamicData> datas)
        {
            //查询涉及到中间表,对对象本身没有任何条件可言
            var query = GetSlaveIds.Create(this);
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                param.Add(this.Root.TableIdName, rootId);
                //if (!this.Root.IsEqualsOrDerivedOrInherited(this.Master))
                if(!this.Root.IsEqualsOrDerivedOrInherited(this.Master))
                {
                    param.Add(this.Master.TableIdName, masterId);
                }

                var sql = query.Build(param);
                SqlHelper.Query(this.ConnectionName, sql, param, datas);
            }
        }

        #endregion

        private static QueryExpression GetScalarByIdExpression(DataTable table, string propertyName)
        {
            return _getScalarById(table)(propertyName);
        }

        private static Func<DataTable, Func<string, QueryExpression>> _getScalarById = LazyIndexer.Init<DataTable, Func<string, QueryExpression>>((table) =>
        {
            return LazyIndexer.Init<string, QueryExpression>((propertyName) =>
            {
                string expression = null;

                if (table.Type == DataTableType.AggregateRoot)
                {
                    expression = string.Format("[{0}=@{0}][select {1}]", EntityObject.IdPropertyName, propertyName);
                }
                else
                {
                    expression = string.Format("[{0}=@{0} and {1}=@{1}][select {2}]", table.Root.TableIdName, EntityObject.IdPropertyName, propertyName);
                }
                return QueryObject.Create(table, expression, QueryLevel.None);
            });
        });
    }
}