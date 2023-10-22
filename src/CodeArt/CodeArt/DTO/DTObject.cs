using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Dynamic;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.IO;
using CodeArt.AppSetting;
using System.Linq.Expressions;
using CodeArt.Concurrent;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace CodeArt.DTO
{
    [DebuggerDisplay("{GetCode()}")]
    public class DTObject : DynamicObject, INullProxy
    {
        #region 根

        internal DTEObject _root;

        internal DTObject(DTEObject root,bool isReadOnly)
        {
            _root = root;
            this.IsReadOnly = isReadOnly;
        }

        internal DTEObject GetRoot()
        {
            return _root;
        }

        //internal DTObject(DTEObject root, bool isReadOnly)
        //{
        //    _root = root;
        //    this.IsReadOnly = isReadOnly;
        //}

        internal DTEntity Parent
        {
            get
            {
                return _root.Parent;
            }
            set
            {
                _root.Parent = value;
            }
        }

        #endregion

        #region 只读控制

        public bool IsReadOnly
        {
            get;
            private set;
        }

        private void ValidateReadOnly()
        {
            if (this.IsReadOnly)
                throw new DTOException(Strings.DTOReadOnly);
        }

        #endregion

        #region 值

        public object this[string findExp]
        {
            get
            {
                return GetValue(findExp);
            }
            set
            {
                SetValue(findExp, value);
            }
        }

        private DTEntity CreateEntity(string name, object value)
        {
            if(IsList(value))
            {
                var list = value as IEnumerable;
                if (list != null) return CreateListEntity(name, list);
            }
            
            var dto = value as DTObject;
            if (dto != null)
            {
                var root = dto.GetRoot();
                root.Name = name;
                return root;
            }
            else
            {
                return new DTEValue(name, value);
            }
        }

        private DTEList CreateListEntity(string name, IEnumerable list)
        {
            var dte = new DTEList();
            dte.Name = name;

            foreach (var item in list)
            {
                dte.CreateAndPush((dto) =>
                {
                    dto.SetValue(item);
                });
            }
            return dte;
        }

        public void SetValue(string findExp, object value)
        {
            ValidateReadOnly();

            var dtoValue = value as DTObject;
            if (dtoValue != null)
            {
                SetObject(findExp, dtoValue);
                return;
            }

            var eitities = FindEntities(findExp, false);
            if (eitities.Length == 0)
            {
                var query = QueryExpression.Create(findExp);
                _root.SetEntity(query, (name) =>
                {
                    return CreateEntity(name, value);
                });
            }
            else
            {
                var isPureValue = IsPureValue(value);
                foreach (var e in eitities)
                {
                    if(e.Type == DTEntityType.Value && isPureValue)
                    {
                        var ev = e as DTEValue;
                        ev.Value = value;
                        continue;
                    }

                    var parent = e.Parent as DTEObject;
                    if (parent == null) throw new DTOException("表达式错误" + findExp);

                    var query = QueryExpression.Create(e.Name);
                    parent.SetEntity(query, (name) =>
                    {
                        return CreateEntity(name, value);
                    });
                }
            }
        }

        public void SetValue(object value)
        {
            SetValue(string.Empty, value);
        }

        private object GetValue(DTEntity entity)
        {
            switch (entity.Type)
            {
                case DTEntityType.Value:
                    {
                        var ev = entity as DTEValue;
                        if (ev != null) return ev.Value;
                    }
                    break;
                case DTEntityType.Object:
                    {
                        var eo = entity as DTEObject;
                        if (eo != null) return new DTObject(eo,this.IsReadOnly);
                    }
                    break;
                case DTEntityType.List:
                    {
                        var el = entity as DTEList;
                        if (el != null) return el.GetObjects();
                    }
                    break;
            }
            return null;
        }


        public object GetValue(string findExp)
        {
            return GetValue(findExp, true);
        }

        public object GetValue(string findExp, bool throwError)
        {
            DTEntity entity = FindEntity(findExp, throwError);
            if (entity == null) return null;
            return GetValue(entity);
        }

        public object GetValue(string findExp, object defaultValue)
        {
            var value = GetValue(findExp, false);
            if (IsValueEmpty(value)) return defaultValue;
            return value;
        }

        public T GetValue<T>(string findExp)
        {
            return DataUtil.ToValue<T>(GetValue(findExp));
        }

        public T GetValue<T>(string findExp, bool throwError)
        {
            return DataUtil.ToValue<T>(GetValue(findExp, throwError));
        }

        public bool GetBooleanValue(string findExp, bool defaultValue)
        {
            var value = GetValue(findExp, false);
            if (value == null) return defaultValue;
            return DataUtil.ToValue<bool>(value);
        }

        /// <summary>
        /// 由于T GetValue<T>(string findExp, T defaultValue)不适合bool类型
        /// 因为与T GetValue<T>(string findExp, bool throwError)冲突了
        /// 所以有该方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="findExp"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueWithDefault<T>(string findExp, T defaultValue)
        {
            return GetValue<T>(findExp, defaultValue);
        }

        public T GetValue<T>(string findExp, T defaultValue)
        {
            DTEValue entity = FindEntity<DTEValue>(findExp, false);
            if (IsValueEmpty(entity)) return defaultValue;
            return DataUtil.ToValue<T>(entity.Value);
        }

        private bool IsValueEmpty(DTEValue entity)
        {
            if (entity == null) return true;
            return IsValueEmpty(entity.Value);
        }

        private bool IsValueEmpty(object value)
        {
            if (value == null) return true;
            var strValue = value as string;
            if (strValue != null) return string.IsNullOrEmpty(strValue);
            return false;
        }

        public bool TryGetValue<T>(string findExp, out T value)
        {
            DTEValue entity = FindEntity<DTEValue>(findExp, false);
            if (IsValueEmpty(entity))
            {
                value = default(T);
                return false;
            }
            value = DataUtil.ToValue<T>(entity.Value);
            return true;
        }

        /// <summary>
        /// 如果不存在<paramref name="findExp"/>，那么直接返回null，否则执行<paramref name="covert"/>，并返回结果
        /// </summary>
        /// <param name="findExp"></param>
        /// <param name="covert"></param>
        /// <returns></returns>
        public T TryGetValue<T>(string findExp, Func<object, T> covert)
        {
            var value = GetValue(findExp, false);
            if (value == null) return default(T);
            return covert(value);
        }

        public T TryGetValue<T>(string findExp)
        {
            return TryGetValue(findExp, (value) =>
            {
                return DataUtil.ToValue<T>(value);
            });
        }

        public T TryGetEnum<T>(string findExp, Func<byte, T> covert)
        {
            var value = GetValue(findExp, false);
            if (value == null) return default(T);
            return covert(DataUtil.ToValue<byte>(value));
        }

        public byte? TryGetValue(string findExp, Func<object, byte?> covert)
        {
            var value = GetValue(findExp, false);
            if (value == null) return null;
            return covert(value);
        }

        public object GetValue()
        {
            return GetValue(string.Empty);
        }

        public object GetValue(bool throwError)
        {
            return GetValue(string.Empty, throwError);
        }

        public object GetValue(object defaultValue)
        {
            return GetValue(string.Empty, defaultValue);
        }

        public T GetValue<T>()
        {
            return DataUtil.ToValue<T>(GetValue());
        }

        public T GetValue<T>(bool throwError)
        {
            return DataUtil.ToValue<T>(GetValue(throwError));
        }

        public T GetValue<T>(T defaultValue)
        {
            return GetValue<T>(string.Empty, defaultValue);
        }

        public object[] GetValues(string findExp, bool throwError)
        {
            DTEntity[] entities = FindEntities(findExp, throwError);
            object[] values = new object[entities.Length];
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var value = GetValue(entity);
                values[i] = value;
            }
            return values;
        }

        public IEnumerable<T> GetValues<T>(string findExp)
        {
            DTEntity[] entities = FindEntities(findExp, false);
            using (var temp = ListPool<T>.Borrow())
            {
                List<T> values = temp.Item;
                for (var i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    var value = GetValue(entity);
                    switch (entity.Type)
                    {
                        case DTEntityType.Value:
                            {
                                values.Add(DataUtil.ToValue<T>(value));
                            }
                            break;
                        case DTEntityType.Object:
                            {
                                values.Add((value as DTObject).GetValue<T>());
                            }
                            break;
                        case DTEntityType.List:
                            {
                                values.AddRange((value as DTObjects).ToArray<T>());
                            }
                            break;
                    }
                }
                return values.ToArray();
            }
        }

        #endregion

        #region 集合

        public void Push(string findExp, int count, Action<DTObject, int> action)
        {
            ValidateReadOnly();

            var entity = GetOrCreateList(findExp);

            for (int i = 0; i < count; i++)
            {
                DTObject dto = entity.CreateAndPush();
                action(dto, i);
            }
        }

        public void Push(int count, Action<DTObject, int> action)
        {
            this.Push(string.Empty, count, action);
        }

        #region 填充dto成员，然后追加到集合，不用重复查找，比较高效

        /// <summary>
        /// 填充dto成员，然后追加到集合，不用重复查找，比较高效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="findExp"></param>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public void Push<T>(string findExp, IEnumerable<T> list, Action<DTObject, T> action)
        {
            ValidateReadOnly();

            DTEList entity = GetOrCreateList(findExp);
            foreach (T item in list)
            {
                DTObject dto = entity.CreateAndPush();
                action(dto, item);
            }
        }

        public void Push<T>(IEnumerable<T> list, Action<DTObject, T> action)
        {
            this.Push<T>(string.Empty, list, action);
        }

        public void Push(string findExp, IEnumerable list, Action<DTObject, object> action)
        {
            ValidateReadOnly();

            DTEList entity = GetOrCreateList(findExp);
            foreach (object item in list)
            {
                DTObject dto = entity.CreateAndPush();
                action(dto, item);
            }
        }

        public void Push(IEnumerable list, Action<DTObject, object> action)
        {
            this.Push(string.Empty, list, action);
        }

        #endregion

        #region 创建dto成员，然后追加到集合，不用重复查找，比较高效

        /// <summary>
        /// 不用重复查找，比较高效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="findExp"></param>
        /// <param name="list"></param>
        /// <param name="factory"></param>
        public void Push<T>(string findExp, IEnumerable<T> list, Func<T, DTObject> factory)
        {
            ValidateReadOnly();

            var entity = GetOrCreateList(findExp);

            foreach (T item in list)
            {
                DTObject dto = factory(item);
                entity.Push(dto);
            }
        }

        public void Push<T>(IEnumerable<T> list, Func<T, DTObject> factory)
        {
            this.Push<T>(string.Empty, list, factory);
        }


        public void Push(string findExp, IEnumerable list, Func<object, DTObject> factory)
        {
            ValidateReadOnly();

            var entity = GetOrCreateList(findExp);

            foreach (object item in list)
            {
                DTObject dto = factory(item);
                entity.Push(dto);
            }
        }

        public void Push(IEnumerable list, Func<object, DTObject> factory)
        {
            this.Push(string.Empty, list, factory);
        }

        #endregion

        /// <summary>
        /// 向集合追加一个成员
        /// </summary>
        /// <param name="findExp"></param>
        /// <param name="member"></param>
        public void Push(string findExp, DTObject member)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(findExp, false);
            if (entity == null)
            {
                var query = QueryExpression.Create(findExp);
                _root.SetEntity(query, (name) =>
                {
                    var dte = new DTEList()
                    {
                        Name = name
                    };
                    return dte;
                });
                entity = FindEntity<DTEList>(findExp, true);
            };
            if (member == null) return;

            entity.Push(member);
        }

        public void SetList(string findExp, IEnumerable<DTObject> items)
        {
            ValidateReadOnly();

            Push(findExp, null);//以此来防止当items个数为0时，没有创建的bug
            foreach (var item in items)
            {
                Push(findExp, item);
            }
        }

        /// <summary>
        /// 如果不存在findExp对应的列表，那么创建
        /// </summary>
        /// <param name="findExp"></param>
        public void SetList(string findExp)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(findExp, false);
            if (entity == null)
            {
                var query = QueryExpression.Create(findExp);
                _root.SetEntity(query, (name) =>
                {
                    var dte = new DTEList();
                    dte.Name = name;
                    return dte;
                });
            };
        }

        public DTObject CreateAndPush(string findExp)
        {
            ValidateReadOnly();

            DTEList entity = GetOrCreateList(findExp);
            return entity.CreateAndPush();
        }

        private DTEList GetOrCreateList(string findExp)
        {
            DTEList entity = FindEntity<DTEList>(findExp, false);
            if (entity == null)
            {
                var query = QueryExpression.Create(findExp);
                _root.SetEntity(query, (name) =>
                {
                    var dte = new DTEList();
                    dte.Name = name;
                    return dte;
                });
                entity = FindEntity<DTEList>(findExp, true);
            }
            return entity;
        }


        public DTObject CreateAndPush()
        {
            return this.CreateAndPush(string.Empty);
        }

        public DTObject CreateAndInsert(string findExp, int index)
        {
            ValidateReadOnly();

            DTEList entity = GetOrCreateList(findExp);
            return entity.CreateAndInsert(index);
        }


        public void Each(string findExp, Action<DTObject> action)
        {
            var list = GetList(findExp, false);
            if (list == null) return;
            foreach (var dto in list)
            {
                action(dto);
            }
        }

        public DTObject Top(string findExp, int count)
        {
            List<DTObject> data = new List<DTObject>(count);

            var list = GetList(findExp, false) ?? new DTObjects();

            foreach (var dto in list)
            {
                if (data.Count == count) break;
                data.Add(dto);
            }

            DTObject result = DTObject.Create();
            result.SetList("rows", data);
            return result;
        }

        public DTObject Page(string findExp, int pageIndex,int pageSize)
        {
            var list = GetList(findExp, false) ?? new DTObjects();
            int dataCount = list.Count();


            var result = DTObject.Create();
            result.SetValue("pageIndex", pageIndex);
            result.SetValue("pageSize", pageSize);
            result.SetValue("dataCount", dataCount);


            var start = (pageIndex - 1) * pageSize;
            if (start >= dataCount)
            {
                result.SetList(findExp, Array.Empty<DTObject>());
                return result;
            }

            var end = start + pageSize - 1;
            if (end >= dataCount) end = dataCount - 1;

            List<DTObject> items = new List<DTObject>(end - start + 1);

            for (var i = start; i <= end; i++)
            {
                items.Add(list.ElementAt(i));
            }

            result.SetList(findExp, items);
            return result;
        }

        public void Each(string findExp, Func<DTObject, bool> action)
        {
            var list = GetList(findExp, false);
            if (list == null) return;
            foreach (var dto in list)
            {
                if (!action(dto)) return; //如果返回false，表示中断遍历操作
            }
        }

        /// <summary>
        /// 移除集合条目
        /// </summary>
        /// <param name="listExp">集合表达式</param>
        /// <param name="indexs">需要移除的项目的序号</param>
        public void RemoveAts(string listExp, IList<int> indexs)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(listExp, false);
            if (entity != null)
            {
                entity.RemoveAts(indexs);
            }
        }

        public void RemoveAt(string listExp, int index)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(listExp, false);
            if (entity != null)
            {
                entity.RemoveAt(index);
            }
        }

        public void Insert(string listExp, int index, DTObject item)
        {
            ValidateReadOnly();

            Push(listExp, null);//以此来防止当items个数为0时，没有创建的bug

            DTEList entity = FindEntity<DTEList>(listExp, false);
            entity.Insert(index, item);
        }

        /// <summary>
        /// 删除成员，仅限直系
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public bool Remove(string entityName)
        {
            return _root.Remove(entityName);
        }

        public bool Remove(string listExp, Func<DTObject, bool> predicate)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(listExp, false);
            if (entity != null)
            {
                return entity.Remove(predicate);
            }
            return false;
        }

        public void Clear(string listExp)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(listExp, false);
            if (entity != null)
            {
                entity.ClearData();
            }
        }

        /// <summary>
        /// 保留集合指定序号的条目，移除其他项
        /// </summary>
        /// <param name="listExp"></param>
        /// <param name="indexs"></param>
        public void RetainAts(string listExp, IList<int> indexs)
        {
            ValidateReadOnly();

            DTEList entity = FindEntity<DTEList>(listExp, false);
            if (entity != null)
            {
                entity.RetainAts(indexs);
            }
        }

        public DTObjects GetList(string findExp)
        {
            var list = GetList(findExp, false);
            return list ?? new DTObjects();
        }

        public IEnumerable<T> GetList<T>(string findExp)
        {
            DTEList entity = FindEntity<DTEList>(findExp, false);
            if (entity == null) return null;
            return entity.GetValues<T>();
        }

        public IEnumerable<T> GetList<T>()
        {
            return GetList<T>(string.Empty);
        }

        public DTObjects GetList(string findExp, bool throwError)
        {
            DTEList entity = FindEntity<DTEList>(findExp, throwError);
            if (entity == null) return null;
            return entity.GetObjects();
        }

        public DTObjects GetList()
        {
            return GetList(string.Empty);
        }

        public DTObjects GetList(bool throwError)
        {
            return GetList(string.Empty, throwError);
        }

        public int Count(string findExp)
        {
            return GetList(findExp).Count;
        }

        public int Count()
        {
            return GetList().Count;
        }


        #endregion

        #region 对象

        public void SetObject(string findExp, DTObject obj)
        {
            ValidateReadOnly();

            if (string.IsNullOrEmpty(findExp))
            {
                //dto.Set(newDTO) 这种表达式下说明此时需要替换整个dto
                //为了保证数据安全，需要克隆，{xxx:{a,b}},如果不克隆，那么b=xxx就会出现错误
                var newRoot = obj.GetRoot().Clone() as DTEObject;
                newRoot.Parent = _root.Parent;
                _root = newRoot;
            }
            else
            {
                var query = QueryExpression.Create(findExp);
                _root.SetEntity(query, (name) =>
                {
                    var e = obj.GetRoot().Clone();
                    e.Name = name;
                    return e;
                });
            }
        }

        /// <summary>
        /// 用<paramref name="obj"/>的内容替换当前对象
        /// </summary>
        /// <param name="obj"></param>
        public void Replace(DTObject obj)
        {
            SetObject(string.Empty, obj);
        }

        public DTObject GetOrCreateObject(string findExp)
        {
            var obj = GetObject(findExp, false);
            if(obj == null)
            {
                obj = DTObject.Create();
                this.SetObject(findExp, obj);
            }
            return obj;
        }

        public DTObject GetObject(string findExp)
        {
            return GetObject(findExp, true);
        }

        public DTObject GetObject(string findExp, DTObject defaultValue)
        {
            var entity = this.FindEntity<DTEObject>(findExp, false);
            if (entity == null) return defaultValue;
            return new DTObject(entity, this.IsReadOnly);
        }

        public DTObject GetObject(string findExp, bool throwError)
        {
            var entity = this.FindEntity<DTEObject>(findExp, throwError);
            if (entity == null) return null;
            return new DTObject(entity, this.IsReadOnly);
        }

        public bool TryGetObject(string findExp, out DTObject value)
        {
            value = GetObject(findExp, false);
            return value != null;
        }

        #endregion

        #region 键值对

        public Dictionary<string, object> GetDictionary()
        {
            return GetDictionary(string.Empty, false);
        }

        public Dictionary<string, object> GetDictionary(string findExp)
        {
            return GetDictionary(findExp, false);
        }

        public Dictionary<string, object> GetDictionary(string findExp, bool throwError)
        {
            var entities = this.FindEntities(findExp, throwError);
            var dictionary = new Dictionary<string, object>();
            foreach (var entity in entities)
            {
                var key = entity.Name;
                var value = CreateEntityValue(entity);
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        public void EachDictionary(string findExp, Action<string, object> action)
        {
            var entities = this.FindEntities(findExp, false);

            if(entities.Length == 1 && entities.First() is DTEObject)
            {
                var es = ((DTEObject)entities.First()).GetEntities();
                foreach (var entity in es)
                {
                    var key = entity.Name;
                    var value = CreateEntityValue(entity);
                    action(key, value);
                }
            }
            else
            {
                foreach (var entity in entities)
                {
                    var key = entity.Name;
                    var value = CreateEntityValue(entity);
                    action(key, value);
                }
            }

        }

        public void EachDictionary(Action<string, object> action)
        {
            EachDictionary(string.Empty, action);
        }


        //public Dictionary<string, T> GetDictionary<T>()
        //{
        //    return GetDictionary<T>(string.Empty, false);
        //}

        //public Dictionary<string, T> GetDictionary<T>(string findExp)
        //{
        //    return GetDictionary<T>(findExp, false);
        //}

        ///// <summary>
        ///// 本质上来说,json就是一组键值对，因此可以获取键值对形式的值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="findExp"></param>
        ///// <param name="throwError"></param>
        ///// <returns></returns>
        //public Dictionary<string, T> GetDictionary<T>(string findExp, bool throwError)
        //{
        //    var entities = this.FindEntities(findExp, throwError);
        //    var dictionary = new Dictionary<string, T>(entities.Length);
        //    foreach (var entity in entities)
        //    {
        //        var key = entity.Name;
        //        var value = CreateEntityValue(entity);
        //        if (value is T)
        //            dictionary.Add(key, (T)value);
        //    }
        //    return dictionary;
        //}

        private object CreateEntityValue(DTEntity entity)
        {
            switch(entity.Type)
            {
                case DTEntityType.Value:
                    {
                        var temp = entity as DTEValue;
                        if (temp != null) return temp.Value;
                    }
                    break;
                case DTEntityType.Object:
                    {
                        var temp = entity as DTEObject;
                        if (temp != null) return new DTObject(temp, this.IsReadOnly);
                    }
                    break;
                case DTEntityType.List:
                    {
                        var temp = entity as DTEList;
                        if (temp != null) return temp.GetObjects();
                    }
                    break;
            }
            throw new DTOException("在CreateEntityValue发生未知的错误,entity类型为" + entity.GetType());
        }

        #endregion

        /// <summary>
        /// 获取当前对象直系名称集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNames()
        {
            return _root.GetEntities().Select((e) =>
            {
                return e.Name;
            });
        }


        internal IEnumerable<DTEntity> GetEntities()
        {
            return _root.GetEntities();
        }


        #region 转换

        /// <summary>
        /// 批量变换dto结构，语法：
        /// <para>name=>newName 转换成员名称</para>
        /// <para>value=newValue 赋值</para>
        /// <para>!member 移除表达式对应的成员</para>
        /// <para>~member 保留表达式对应的成员，其余的均移除</para>
        /// 多个表达式可以用;号连接
        /// </summary>
        /// <param name="express">
        /// findExp=>name;findExp=>name
        /// </param>
        public void Transform(string express)
        {
            var expresses = TransformExpressions.Create(express);
            foreach (var exp in expresses)
            {
                exp.Execute(this);
            }
        }

        /// <summary>
        /// 该方法主要用于更改成员值
        /// </summary>
        /// <param name="express">
        /// findExp=valueFindExp
        /// 说明：
        /// valueFindExp 可以包含检索方式，默认的方式是在findExp检索出来的结果中所在的DTO对象中进行检索
        /// 带“@”前缀，表示从根级开始检索
        /// 带“*”前缀，表示返回值所在的对象
        /// </param>
        /// <param name="transformValue"></param>
        public void Transform(string express, Func<object, object> transformValue)
        {
            AssignExpression exp = AssignExpression.Create(express) as AssignExpression;
            if (exp == null) throw new DTOException("变换表达式错误" + express);
            exp.Execute(this, transformValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="action"></param>
        /// <param name="self">自身是否参与遍历</param>
        public void DeepEach(string listName, Action<DTObject> action, bool self = false)
        {
            this.Each(listName, (child) =>
            {
                child.DeepEach(listName, action);
                action(child);
            });
            if (self) action(this);
        }       


        #endregion

        #region 辅助

        public bool Exist(string findExp)
        {
            //return FindEntity(findExp, false) != null || GetObject(findExp, false) != null;
            return FindEntity(findExp, false) != null;
        }

        /// <summary>
        /// 确保<paramref name="findExp"/>对应的值是有效的
        /// </summary>
        /// <param name="findExp"></param>
        /// <returns></returns>
        public bool Valid(string findExp)
        {
            //return FindEntity(findExp, false) != null || GetObject(findExp, false) != null;
            var e = FindEntity(findExp, false);
            if (e == null) return false;
            var ve = e as DTEValue;
            if (ve != null)
            {
                var strValue = ve.Value as string;
                if (strValue != null && (strValue == "null" || strValue=="" || strValue == "undefined")) return false;
                return ve.Value != null;
            }
            return true;
        }

        internal DTEntity FindEntity(string findExp, bool throwError)
        {
            var query = QueryExpression.Create(findExp);

            DTEntity entity = null;
            var es = _root.FindEntities(query);
            if (es.Count() > 0) entity = es.First();

            if (entity == null)
            {
                if (throwError)
                    throw new NotFoundDTEntityException("没有找到" + findExp + "对应的DTO实体！");
                return null;
            }
            return entity;
        }

        internal DTEntity[] FindEntities(string findExp, bool throwError)
        {
            var query = QueryExpression.Create(findExp);

            using (var temp = ListPool<DTEntity>.Borrow())
            {
                var list = temp.Item;
                var es = _root.FindEntities(query);
                list.AddRange(es);

                if (list.Count == 0)
                {
                    if (throwError)
                        throw new NotFoundDTEntityException("没有找到" + findExp + "对应的DTO实体！");
                    return list.ToArray();
                }
                return list.ToArray();
            }
        }

        //internal T[] FindEntities<T>(string findExp, bool throwError) where T : DTEntity
        //{
        //    List<T> list = new List<T>();
        //    var query = QueryExpression.Create(findExp);
        //    var es = _root.FindEntities(query);
        //    foreach (var e in es)
        //    {
        //        var temp = e as T;
        //        if (temp != null) list.Add(temp);
        //    }

        //    if (list.Count == 0)
        //    {
        //        if (throwError)
        //            throw new NotFoundDTEntityException("没有找到" + findExp + "对应的DTO实体！");
        //        return list.ToArray();
        //    }
        //    return list.ToArray();
        //}

        private T FindEntity<T>(string findExp, bool throwError) where T : DTEntity
        {
            DTEntity e = FindEntity(findExp, throwError);
            if (e == null) return null;
            T entity = e as T;
            if (entity == null && throwError)
                throw new DTOTypeErrorException("表达式" + findExp + "对应的DTO不是" + typeof(T).FullName + "！");
            return entity;
        }

        /// <summary>
        /// 是否为单值dto，即：{value}的形式
        /// </summary>
        public bool IsSingleValue
        {
            get
            {
                return _root.IsSingleValue();
            }
        }

        //internal void OrderEntities()
        //{
        //    _root.OrderEntities();
        //}


        /// <summary>
        /// 是否为纯值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsPureValue(object value)
        {
            return !(value is DTObject || IsList(value));
        }

        private static bool IsList(object value)
        {
            return value is IEnumerable && !(value is string);
            //return value != null && (value is IList || value.GetType().IsAchieveOrEquals(typeof(IList<>)));
        }

        #endregion

        #region 数据

        public bool ContainsData()
        {
            return _root.ContainsData();
        }

        public void ClearData()
        {
            ValidateReadOnly();
            _root.ClearData();
        }

        /// <summary>
        /// 无视只读标记，强制清理数据
        /// </summary>
        internal void ForceClearData()
        {
            _root.ClearData();
        }

        public DTObject Clone()
        {
            return new DTObject(_root.Clone() as DTEObject, this.IsReadOnly);
        }

        #endregion

        #region 代码

        public string GetCode()
        {
            return GetCode(false, true);
        }

        public string GetCode(bool sequential)
        {
            return GetCode(sequential, true);
        }
   
        public string GetCode(bool sequential, bool outputKey)
        {
            return _root.GetCode(sequential, outputKey);
        }

        public string GetSchemaCode()
        {
            return GetSchemaCode(false, true);
        }

        public string GetSchemaCode(bool sequential, bool outputKey)
        {
            return _root.GetSchemaCode(sequential, outputKey);
        }

        #endregion

        public dynamic Dynamic
        {
            get
            {
                return (dynamic)this;
            }
        }

        #region 可重复使用的dto创建方法

        /// <summary>
        /// 完整的创建方法
        /// </summary>
        /// <param name="code"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="isPinned"></param>
        /// <returns></returns>
        private static DTObject CreateComplete(string code, bool isReadOnly)
        {
            var root = EntityDeserializer.Deserialize(code, isReadOnly);
            return new DTObject(root, isReadOnly);
        }


        /// <summary>
        /// 创建非只读的可重复使用的dto对象，该对象的使用周期与共生器同步
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DTObject Create(byte[] data)
        {
            var code = data.GetString(Encoding.UTF8);
            return Create(code);
        }

        /// <summary>
        /// 根据架构代码将对象的信息加载到dto中，该对象的使用周期与共生器同步
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DTObject Create(string schemaCode, object target)
        {
            var dy = target as IDTOSerializable;
            if (dy != null)
            {
                return Create(schemaCode, dy.GetData());
            }

            var dto = target as DTObject;
            if(dto != null)
            {
                DTObject result = DTObject.Create();
                result.Load(schemaCode, dto);
                return result;
            }
            return DTObjectMapper.Instance.Load(schemaCode, target);
        }

        #endregion

        #region 不会被共生器回收的dto创建方法


        /// <summary>
        /// 创建非只读的、不会被共生器回收的dto对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DTObject Create(string code)
        {
            if (string.IsNullOrEmpty(code)) return DTObject.Create();
            return CreateComplete(code, false);
        }

        /// <summary>
        /// 创建不会被共生器回收的dto对象
        /// </summary>
        /// <param name="code"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public static DTObject Create(string code, bool isReadOnly)
        {
            return CreateComplete(code, isReadOnly);
        }

        /// <summary>
        /// 创建非只读的、不会被共生器回收的dto对象
        /// </summary>
        /// <returns></returns>
        public static DTObject Create()
        {
            return CreateComplete("{}", false);
        }

        public static DTObject CreateXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string code = JsonConvert.SerializeXmlNode(doc, Formatting.None, true);
            return Create(code);
        }

        #endregion

        #region 检测架构代码包含

        /// <summary>
        /// 对象是否完全包含代码<paramref name="schemaCode"/>,这表示<paramref name="schemaCode"/>的所有成员都与目标对象一致
        /// 待测试todo...
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <returns></returns>
        public bool ContainsSchemaCode(string schemaCode)
        {
            var target = DTObject.Create(schemaCode);
            return ContainsSchemaCode(target);
        }

        /// <summary>
        /// 对象是否包含<paramref name="target"/>的架构代码
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool ContainsSchemaCode(DTObject target)
        {
            return ContainsSchemaCode(this.GetRoot(), target.GetRoot());
        }

        private static bool ContainsSchemaCode(DTEObject source, DTEObject target)
        {
            var es = target.GetEntities();
            foreach (var e in es)
            {
                var se = source.Find(e.Name);
                if (se == null) return false; //源中没有找到成员

                var obj = e as DTEObject;
                if (obj != null)
                {
                    //对比子项
                    var sObj = se as DTEObject;
                    if (sObj == null) return false; //类型不同
                    if (!ContainsSchemaCode(sObj, obj)) return false;
                }
                else
                {
                    var list = e as DTEList;
                    if (list != null)
                    {
                        //对比子项
                        var sList = se as DTEList;
                        if (sList == null) return false; //类型不同
                        if (!list.ItemTemplate.ContainsSchemaCode(sList.ItemTemplate)) return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 对象映射

        /// <summary>
        /// 根据架构代码，将dto的数据创建到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="schemaCode"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object Save(Type instanceType, string schemaCode)
        {
            return DTObjectMapper.Instance.Save(instanceType, schemaCode, this);
        }

        /// <summary>
        /// 根据架构代码，将dto的数据创建到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object Save(Type instanceType)
        {
            return Save(instanceType, string.Empty);
        }

        /// <summary>
        /// 根据架构代码，将dto中的数据全部保存到类型为<typeparamref name="T"/>的实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schemaCode"></param>
        /// <returns></returns>
        public void Save<T>(T obj, string schemaCode)
        {
            var instanceType = typeof(T);
            DTObjectMapper.Instance.Save(obj, schemaCode, this);
        }

        /// <summary>
        /// 将dto中的数据全部保存到类型为<typeparamref name="T"/>的实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void Save<T>(T obj)
        {
            Save<T>(obj, string.Empty);
        }

        /// <summary>
        /// 根据架构代码，将dto中的数据全部保存到类型为<typeparamref name="T"/>的实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schemaCode"></param>
        /// <returns></returns>
        public T Save<T>(string schemaCode)
        {
            var instanceType = typeof(T);
            return (T)Save(instanceType, schemaCode);
        }

        /// <summary>
        /// 将dto中的数据全部保存到类型为<typeparamref name="T"/>的实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Save<T>()
        {
            return Save<T>(string.Empty);
        }

        /// <summary>
        /// 根据架构代码将对象的信息加载到dto中
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public void Load(string schemaCode, object target)
        {
            var dy = target as IDTOSerializable;
            if(dy != null)
            {
                Load(schemaCode,dy.GetData());
                return;
            }

            var dto = target as DTObject;
            if (dto != null)
            {
                Load(schemaCode, target);
                return;
            }
            DTObjectMapper.Instance.Load(this, schemaCode, target);
        }


        private void Load(string schemaCode, DTObject target)
        {
            var schema = DTObject.Create(schemaCode);
            var entities = schema.GetEntities();
            foreach(var entity in entities)
            {
                var name = entity.Name;
                if (target.Exist(name))
                {
                    this[name] = target[name];
                }
            }
        }

        /// <summary>
        /// 将<paramref name="target"/>里面的所有属性的值加载到dto中
        /// </summary>
        /// <param name="target"></param>
        public void Load(object target)
        {
            Load(string.Empty, target);
        }

        #endregion

        #region 动态支持

        /// <summary>  
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name, false);
            return true; //无论什么情况下都返回true，表示就算dto没有定义值，也可以获取null
        }

        /// <summary>  
        /// 实现动态对象属性值设置的方法。  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }


        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Save(binder.Type);
            return true;
        }

        #endregion

        #region 序列化/反序列化

        /// <summary>
        /// 将dto对象反序列化到一个实体对象中,对象需要配置dto序列化特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            return DTObject.Deserialize<T>(this);
        }

        public object Deserialize(Type objectType)
        {
            return DTObject.Deserialize(objectType, this);
        }

        /// <summary>
        /// 将dto的内容反序列化到<paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        public void Deserialize(object instance)
        {
            DTObjectDeserializer.Instance.Deserialize(instance, this);
        }

        /// <summary>
        /// 将dto对象反序列化到一个实体对象中,对象需要配置dto序列化特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static T Deserialize<T>(DTObject dto)
        {
            return (T)Deserialize(typeof(T), dto);
        }

        public static object Deserialize(Type objectType, DTObject dto)
        {
            return DTObjectDeserializer.Instance.Deserialize(objectType, dto);
        }


        /// <summary>
        /// 将对象的数据序列化到dto对象中,对象需要配置dto序列化特性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isPinned">对象生命周期是否依赖于共生器，true:不依赖 ，false:依赖</param>
        /// <returns></returns>
        public static DTObject Serialize(object obj)
        {
            return DTObjectSerializer.Instance.Serialize(obj);
        }

        #endregion

        #region 类型解析

        public static TypeMetadata GetMetadata(string metadataCode)
        {
            return new TypeMetadata(metadataCode);
        }


        #endregion

        public DTObject AsReadOnly()
        {
            var code = this.GetCode();
            return DTObject.Create(code, true);
        }

        public static readonly Type Type = typeof(DTObject);

        #region 空对象

        public const string EmptyCode = "{__empty:true}";


        public static readonly DTObject Empty = DTObject.Create(EmptyCode, true);

        public bool IsEmpty()
        {
            return this.GetValue<bool>("__empty", false);
        }


        public bool IsNull()
        {
            return this.IsEmpty();
        }

        #endregion


        #region 常用对象

        public const string TrueCode = "{result:true}";

        public const string FalseCode = "{result:true}";


        public static readonly DTObject True = DTObject.Create(TrueCode, true).AsReadOnly();

        public static readonly DTObject False = DTObject.Create(FalseCode, true).AsReadOnly();

        #endregion

        public byte[] ToData()
        {
            return this.GetCode(false, true).GetBytes(Encoding.UTF8);
        }

        #region 唯一性

        public override bool Equals(object obj)
        {
            var target = obj as DTObject;
            if (target == null) return false;
            //sequential为true表示统一了键值对的排序，所以可以直接通过代码来比较是否相等
            return string.Equals(this.GetCode(true, true), target.GetCode(true, true), StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.GetCode(true, true).GetHashCode();
        }

        public static bool operator ==(DTObject a, DTObject b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(DTObject a, DTObject b)
        {
            return !(a == b);
        }

        #endregion

    }
}
