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
using System.Xml.Linq;
using System.Reflection.Emit;

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
        private object ReadOneToMore(PropertyRepositoryAttribute tip,ParameterRepositoryAttribute prmTip, DomainObject parent, object rootId, object masterId, QueryLevel level)
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
                var list = CreateList(parent, implementType, tip.Property);
                var elementType = this.Middle.ElementType;

                if (this.Type == DataTableType.AggregateRoot)
                {
                    //引用了多个外部内聚根
                    var model = DataModel.Create(elementType);
                    var root = model.Root;
                    var slaveIdName = GeneratedField.SlaveIdName;
                    var slaveTip = root.ObjectTip;

                    var queryLevel = GetQueryAggreateRootLevel(level);
                    if (slaveTip.Snapshot)  //如果外部内聚根是有快照的，那么当数据不存在时，加载快照
                    {
                        foreach (var data in datas)
                        {
                            var slaveId = data.Get(slaveIdName);
                            var item = root.QuerySingle(slaveId, queryLevel);
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
                            var item = root.QuerySingle(slaveId, queryLevel);
                            if (!((IDomainObject)item).IsEmpty())
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    var slaveIdName = GeneratedField.SlaveIdName;
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

        /// <summary>
        /// 读取基础数据的集合值
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="prmTip"></param>
        /// <param name="parent"></param>
        /// <param name="rootId"></param>
        /// <param name="masterId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private object ReadValues(PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DomainObject parent, object rootId, object masterId, QueryLevel level)
        {
            using (var temp = SqlHelper.BorrowDatas())
            {
                var datas = temp.Item;
                QueryPrimitiveValues(rootId, masterId, datas);

                Type implementType = null;
                if (parent == null)
                {
                    //说明还在构造阶段,或者是内部调用
                    implementType = prmTip?.ImplementType ?? this.ObjectType; //middle表对应的是属性的基类类型
                }
                else
                {
                    implementType = this.ObjectType; //middle表对应的是属性的基类类型
                }
                var list = CreateList(parent, implementType, tip.Property);
                var elementType = this.ElementType;

                var valueName = GeneratedField.PrimitiveValueName;
                foreach (var data in datas)
                {
                    var value = data.Get(valueName);
                    list.Add(value);
                }
                return list;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="listType"></param>
        /// <param name="property">集合在对象中的属性定义</param>
        /// <returns></returns>
        private IList CreateList(DomainObject parent, Type listType, DomainProperty property)
        {
            if (_isDomainCollection(listType))
            {
                var constructor = _getDomainCollectionConstructor(listType);
                using (var temp = ArgsPool.Borrow1())
                {
                    var args = temp.Item;
                    args[0] = property;
                    var collection = constructor.CreateInstance(args) as IDomainCollection;
                    collection.Parent = parent;
                    return collection as IList;
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
        private DomainObject CreateObject(Type objectType, DynamicData data, QueryLevel level)
        {
            DomainObject obj = null;

            DataContext.Using(()=>
            {
                if (this.IsDynamic)
                {
                    if (data.IsEmpty()) obj = (DomainObject)DomainObject.GetEmpty(this.ObjectType);
                    else
                    {
                        obj = CreateObjectImpl(this.DynamicType, this.DynamicType.ObjectType, data, level);
                    }
                }
                else
                {
                    if (data.IsEmpty()) obj = (DomainObject)DomainObject.GetEmpty(objectType);
                    else
                    {
                        obj = CreateObjectImpl(objectType, objectType, data, level);
                    }
                }
            });
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defineType">如果是动态领域对象，那么该类型为定义领域属性的类型（也就是定义类型），否则是对象的实际类型</param>
        /// <param name="objectType">实际存在内存中的实际类型</param>
        /// <param name="data"></param>
        /// <returns></returns>
        private DomainObject CreateObjectImpl(Type defineType, Type objectType, DynamicData data, QueryLevel level)
        {
            DomainObject obj = null;
            try
            {
                DataContext.Current.InBuildObject = true;

                if (this.IsDynamic)
                {
                    //构造对象
                    obj = ConstructDynamicObject(objectType, this.DynamicType.Define);
                }
                else
                {
                    //构造对象
                    obj = ConstructObject(objectType, data, level);
                }

                //设置代理对象
                SetDataProxy(obj, data, level == QueryLevel.Mirroring);

                //为了避免死循环，我们先将对象加入到构造上下文中
                AddToConstructContext(obj, data);

                //加载属性
                LoadProperties(defineType, data, obj, level);

                RemoveFromConstructContext(obj);

                //补充信息
                Supplement(obj, data, level);
            }
            catch
            {
                throw;
            }
            finally
            {
                DataContext.Current.InBuildObject = false;
            }
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
                object rootId = data.Get(GeneratedField.RootIdName);
                ConstructContext.Add(rootId, id, obj);
            }
        }

        private void RemoveFromConstructContext(DomainObject obj)
        {
            ConstructContext.Remove(obj);
        }


        private DomainObject ConstructObject(Type objectType, DynamicData data, QueryLevel level)
        {
            var constructorTip = ConstructorRepositoryAttribute.GetTip(objectType);
            var constructor = constructorTip.Constructor;
            var args = CreateArguments(constructorTip, data, level);
            return (DomainObject)constructor.CreateInstance(args);
        }

        private DomainObject ConstructDynamicObject(Type objectType, TypeDefine define)
        {
            var constructor = ConstructorRepositoryAttribute.GetDynamicConstructor(objectType);
            using (var temp = ArgsPool.Borrow2())
            {
                var args = temp.Item;
                args[0] = define;
                args[1] = false; //不为空 empty参数
                return (DomainObject)constructor.CreateInstance(args);
            }
        }

        /// <summary>
        /// 加载属性
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        private void LoadProperties(Type objectType, DynamicData data, DomainObject obj, QueryLevel level)
        {
            var propertyTips = Util.GetPropertyAllTips(objectType);  //此处不必考虑是否为派生类，直接赋值所有属性
            foreach (var propertyTip in propertyTips)
            {
                //只有是可以公开设置的属性和不是延迟加载的属性我们才会主动赋值
                //有些属性是私有设置的，这种属性有可能是仅获取外部的数据而不需要赋值的
                //如果做了inner处理，那么立即加载
                if ((propertyTip.IsPublicSet && !propertyTip.Lazy) || ContainsObjectData(propertyTip, data))
                {
                    var value = ReadPropertyValue(obj, propertyTip, null, data, level); //已不是构造，所以不需要prmTip参数
                    if (value == null)
                    {
                        if(obj.IsSnapshot && !propertyTip.Snapshot)
                        {
                            //对象是镜像，并且属性没有标记为镜像，那么当加载不到对应数据时，赋予默认值
                            value = propertyTip.Property.GetDefaultValue(obj, propertyTip.Property);
                        }
                        else
                        {
                            throw new DataAccessException(string.Format(Strings.LoadPropertyError, propertyTip.Path));
                        }
                    }
                        
                    obj.SetValue(propertyTip.Property, value);
                }
            }
        }

        private void SetDataProxy(DomainObject obj, DynamicData data, bool isMirror)
        {
            //设置代理对象
            obj.DataProxy = new DataProxyPro(data, this, isMirror);
        }

        private void Supplement(DomainObject obj, DynamicData data, QueryLevel level)
        {
            var valueObject = obj as IValueObject;
            if (valueObject != null)
            {
                object id = null;
                if (data.TryGetValue(EntityObject.IdPropertyName, out id))
                {
                    valueObject.TrySetId((Guid)id);
                }
            }

            obj.MarkClean(); //对象从数据库中读取，是干净的
        }

        private object[] CreateArguments(ConstructorRepositoryAttribute tip, DynamicData data, QueryLevel level)
        {
            var length = tip.Parameters.Count();
            if (length == 0) return Array.Empty<object>();
            object[] args = new object[length];
            foreach (var prm in tip.Parameters)
            {
                var arg = CreateArgument(prm, data, level);
                args[prm.Index] = arg;
            }
            return args;
        }

        private object CreateArgument(ConstructorRepositoryAttribute.ConstructorParameterInfo prm, DynamicData data, QueryLevel level)
        {
            object value = null;
            //看构造特性中是否定义了加载方法
            if (prm.TryLoadData(this.ObjectType, data, level, out value))
            {
                return value;
            }
            var tip = prm.PropertyTip;
            if (tip == null)
                throw new DataAccessException(string.Format(Strings.ConstructionParameterNoProperty,this.ObjectType.FullName, prm.Name));

            //从属性定义中加载
            value = ReadPropertyValue(null, tip, prm.Tip, data, level); //在构造时，还没有产生对象，所以parent为 null
            if (value == null)
                throw new DataAccessException(string.Format(Strings.ConstructionParameterError, prm.DeclaringType.FullName, prm.Name));
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tip"></param>
        /// <param name="prmTip"></param>
        /// <param name="data"></param>
        /// <param name="level">对象在被加载时用到的查询级别</param>
        /// <returns></returns>
        public object ReadPropertyValue(DomainObject parent, PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DynamicData data, QueryLevel level)
        {
            //看对应的属性特性中是否定义了加载方法，优先执行自定义方法
            object value = null;
            if (tip.TryLoadData(this.ObjectType, data, level, out value))
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
                        return ReadPrimitiveList(parent, tip, prmTip, data, level);
                    }
                case DomainPropertyType.AggregateRoot:
                    {
                        return ReadAggregateRoot(tip, data, level);
                    }
                case DomainPropertyType.ValueObject:
                case DomainPropertyType.EntityObject:
                    {
                        return ReadMember(tip, data);
                    }
                case DomainPropertyType.EntityObjectList:
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.AggregateRootList:
                    {
                        return ReadMembers(parent, tip, prmTip, data, level);
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
                                                : GeneratedField.RootIdName;
                if (data.TryGetValue(rootIdName, out rootId))
                {
                    return QueryDataScalar(rootId, id, tip.PropertyName);
                }
            }
            return null;
        }

        #endregion

        #region 读取基础值的集合数据

        private object ReadPrimitiveList(DomainObject parent, PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DynamicData data, QueryLevel level)
        {
            object rootId = null;
            var rootIdName = this.Type == DataTableType.AggregateRoot
                                            ? EntityObject.IdPropertyName
                                            : GeneratedField.RootIdName;
            if (data.TryGetValue(rootIdName, out rootId))
            {
                //当前对象的编号，就是子对象的masterId
                object masterId = null;
                data.TryGetValue(EntityObject.IdPropertyName, out masterId);

                var child = GetChildTableByRuntime(this, tip);
                return child.ReadValues(tip, prmTip, parent, rootId, masterId, level);
            }
            return null;

        }

        #endregion


        private static QueryLevel GetQueryAggreateRootLevel(QueryLevel masterLevel)
        {
            //除了镜像外，通过属性读取外部根，我们都是无锁的查询方式
            return masterLevel == QueryLevel.Mirroring ? QueryLevel.Mirroring : QueryLevel.None;
        }

        /// <summary>
        /// 获得子对象的数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryGetObjectData(DynamicData data, PropertyRepositoryAttribute tip, out DynamicData value)
        {
            value = null;

            //以前缀来最大化收集，因为会出现类似Good_Unit_Name 这种字段，不是默认字段，但是也要收集，是通过inner good.unit的语法来的
            var prefix = $"{tip.PropertyName}_";

            foreach(var p in data)
            {
                if (p.Key.StartsWith(prefix))
                {
                    if(value == null) value = new DynamicData();

                    value[p.Key.Substring(prefix.Length)] = p.Value;
                }
            }

            return value != null;

            //var fields = model.Root.DefaultQueryFields;
            //foreach (var field in fields)
            //{
            //    var name = $"{model.Root.Name}_{field.Name}";
            //    if (!data.ContainsKey(name))
            //    {
            //        value = null;
            //        return false;
            //    }
            //}

            //value = new DynamicData();

            //foreach (var field in fields)
            //{
            //    var name = $"{model.Root.Name}_{field.Name}";
            //    value[field.Name] = data[name];
            //}

            //return true;
        }

        private bool ContainsObjectData(PropertyRepositoryAttribute tip,DynamicData data)
        {
            DataTable table = null;

            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.AggregateRoot:
                    {
                        var model = DataModel.Create(tip.PropertyType);
                        table = model.Root;
                        break;
                    }
                case DomainPropertyType.EntityObject:
                case DomainPropertyType.ValueObject:
                    {
                        table = GetChildTableByRuntime(this, tip);
                        break;
                    }
                default: 
                    return false;
            }

            //以默认字段来验证
            var fields = table.DefaultQueryFields;

            foreach(var field in fields)
            {
                var name = $"{tip.PropertyName}_{field.Name}";
                if (!data.ContainsKey(name)) return false;
            }
            return true;


            //var prefix = $"{tip.PropertyName}_";

            //foreach (var p in data)
            //{
            //    if (p.Key.StartsWith(prefix)) return true;
            //}
            //return false;
        }

        private object ReadAggregateRoot(PropertyRepositoryAttribute tip, DynamicData data, QueryLevel level)
        {
            var model = DataModel.Create(tip.PropertyType);

            if (TryGetObjectData(data, tip, out var item))
            {
                DynamicData entry = item as DynamicData;
                var obj = model.Root.CreateObject(tip.PropertyType, entry, QueryLevel.None); //从数据中直接加载的根对象信息，一定是不带锁的

                //数据填充的对象，不加载镜像（为了提高性能）
                //if (((IDomainObject)obj).IsEmpty() && model.ObjectTip.Snapshot)
                //{
                //    //加载快照
                //    obj = model.Snapshot.QuerySingle(id, QueryLevel.None);
                //}

                return obj;
            }


            var dataKey = _getIdName(tip.PropertyName);

            object id = null;
            if (data.TryGetValue(dataKey, out id))
            {
                var queryLevel = GetQueryAggreateRootLevel(level);
                var obj = model.Root.QuerySingle(id, queryLevel);

                if (((IDomainObject)obj).IsEmpty() && model.ObjectTip.Snapshot)
                {
                    //加载快照
                    obj = model.Snapshot.QuerySingle(id, QueryLevel.None);
                }

                return obj;
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
            var subData = new DynamicData(); //由于subData要参与构造，所以不从池中取
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
                if (tip.DomainPropertyType == DomainPropertyType.AggregateRoot)
                {
                    var idName = _getIdName(tip.PropertyName);
                    var id = data.Get(idName);
                    return ReadSnapshot(tip, id);
                }
                return DomainObject.GetEmpty(tip.PropertyType);
            }


            var typeKey = (string)subData.Get(GeneratedField.TypeKeyName);
            Type objectType = null;
            if (this.IsDynamic)
            {
                objectType = tip.PropertyType;
            }
            else
            {
                objectType = string.IsNullOrEmpty(typeKey) ? tip.PropertyType : DerivedClassAttribute.GetDerivedType(typeKey);
            }


            var child = GetRuntimeTable(this, tip.PropertyName, objectType);
            //先尝试中构造上下文中得到
            return child.GetObjectFromConstruct(subData) ?? child.CreateObject(objectType, subData, QueryLevel.None); //成员始终是QueryLevel.None的方式加载
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
                                                : GeneratedField.RootIdName;
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

        private object ReadMembers(DomainObject parent, PropertyRepositoryAttribute tip, ParameterRepositoryAttribute prmTip, DynamicData data, QueryLevel level)
        {
            object rootId = null;
            var rootIdName = this.Type == DataTableType.AggregateRoot
                                            ? EntityObject.IdPropertyName
                                            : GeneratedField.RootIdName;
            if (data.TryGetValue(rootIdName, out rootId))
            {
                //当前对象的编号，就是子对象的masterId
                object masterId = null;
                data.TryGetValue(EntityObject.IdPropertyName, out masterId);

                var child = GetChildTableByRuntime(this, tip);
                return child.ReadOneToMore(tip, prmTip, parent, rootId, masterId, level);
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
                                            : GeneratedField.RootIdName;
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

                var slaveIdName = GeneratedField.SlaveIdName;
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
                                            : GeneratedField.RootIdName;
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
                    param.Add(GeneratedField.RootIdName, rootId);
                }
                param.Add(EntityObject.IdPropertyName, id);

                var sql = query.Build(param, this);
                return SqlHelper.ExecuteScalar(sql, param);
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
                param.Add(GeneratedField.RootIdName, rootId);
                //if (!this.Root.IsEqualsOrDerivedOrInherited(this.Master))
                if(!this.Root.IsEqualsOrDerivedOrInherited(this.Master))
                {
                    param.Add(GeneratedField.MasterIdName, masterId);
                }

                AddToTenant(param);

                var sql = query.Build(param, this);
                SqlHelper.Query(sql, param, datas);
            }
        }

        /// <summary>
        /// 查询基础值的集合的数据
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="masterId"></param>
        /// <param name="datas"></param>
        private void QueryPrimitiveValues(object rootId, object masterId, List<DynamicData> datas)
        {
            //查询涉及到中间表,对对象本身没有任何条件可言
            var query = GetPrimitiveValues.Create(this);
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                param.Add(GeneratedField.RootIdName, rootId);
                if (!this.Root.IsEqualsOrDerivedOrInherited(this.Master))
                {
                    param.Add(GeneratedField.MasterIdName, masterId);
                }

                if (this.IsSessionEnabledMultiTenancy)
                {
                    param.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                }

                var sql = query.Build(param, this);
                SqlHelper.Query(sql, param, datas);
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
                    expression = string.Format("[{0}=@{0} and {1}=@{1}][select {2}]", GeneratedField.RootIdName, EntityObject.IdPropertyName, propertyName);
                }
                return QueryObject.Create(table, expression, QueryLevel.None);
            });
        });
    }
}