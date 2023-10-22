using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.AppSetting;


namespace CodeArt.DomainDriven
{
    /// <summary>
    /// <para>我们保证领域对象的读操作是线程安全的,但是写操作（例如属性赋值等）不是线程安全的，如果需要并发控制请根据业务要求自行编写代码</para>
    /// <para>虽然写操作不是线程安全的，但是通过带锁查询可以有效的保证多线程安全，这包括以下约定：</para>
    /// <para>用QueryLevel.None加载的对象，请不要做更改对象状态的操作，仅用于读操作</para>
    /// <para>用QueryLevel.Single和QueryLevel.HoldSingle加载的对象，由当前线程独占，其他线程不可访问，因此对于当前线程来说可以安全的读写，由于独占可能会引起性能的下降，也可能引起死锁</para>
    /// <para>用QueryLevel.Mirroring加载的对象属于当前线程独立的对象，与其他线程没有交集，对于当前线程是可以安全的读写，性能较高，当需要在一个事务中处理多个根对象时必须使用该加载方式</para>
    /// <para>对于领域对象属性是否改变的约定：
    /// 1.如果属性为普通类型（int、string等基础类型）,根据值是否发生了改变来判定属性是否改变
    /// 2.如果属性为值对象类型（ValueObject）,根据ValueObject的值是否发生了改变来判定属性是否改变
    /// 3.如果属性为实体对象类型（EntityObject,EntityObjectPro,Aggregate）,只要赋值，属性就会发生改变；
    ///   实体对象内部的属性发生改变，不影响外部属性的变化，因为他们的引用关系并没有被改变；
    /// 4.如果属性为实体对象类型的集合（ObjectCollection(EntityObject),ObjectCollection(EntityObjectPro),ObjectCollection(Aggregate)）,只要赋值，属性就会发生改变；
    ///   单个实体对象内部的属性发生改变，不影响外部属性的变化，因为他们的引用关系并没有被改变；
    ///   实体对象集合的成员（数量或者成员被替换了）发生了变化，那么属性被改变，因为集合代表的是引用对象和多个被引用对象之间的关系，集合成员变化了，代表引用关系也变化了
    /// </para>
    /// </summary>
    [MergeDomain]
    [FrameworkDomain]
    public abstract class DomainObject : System.Dynamic.DynamicObject, IDomainObject, INullProxy
    {
        /// <summary>
        /// 领域对象的类型，请注意，在动态领域对象下，该类型是对应的领域对象的类型，而不是DynamicRoot、DynamicValueObject等载体对象
        /// </summary>
        public Type ObjectType
        {
            get
            {
                return GetObjectType();
            }
        }

        protected virtual Type GetObjectType()
        {
            return this.InstanceType;//默认是实例的类型
        }

        protected Type InstanceType
        {
            get;
            private set;
        }


        /// <summary>
        /// 领域对象的继承深度（注意，仅从DomainObject基类开始算起）
        /// </summary>
        internal int TypeDepth
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否追踪属性变化的情况，该值为true，会记录属性更改前的值，这会消耗一部分内存
        /// </summary>
        public virtual bool TrackPropertyChange
        {
            get
            {
                //默认算法是，该对象需要写入日志，我们就追踪
                //程序员也可以根据需要，改变该属性的算法
                return ObjectLogableAttribute.GetTip(this.ObjectType) != null;
            }
        }

        public DomainObject()
        {
            this.IsConstructing = true;
            this.InstanceType = this.GetType(); 
            this.TypeDepth = GetTypeDepth();
            this.OnConstructed();
        }

        private int GetTypeDepth()
        {
            var depth = this.InstanceType.GetDepth();

            var baseType = typeof(DomainObject).BaseType;
            if (baseType == null) return depth;
            return depth - baseType.GetDepth();
        }

        #region 对象快照

        /// <summary>
        /// 对象是否为一个快照，如果该值为true，表示对象已经被仓储删除或者其属性值已被修改
        /// </summary>
        public bool IsSnapshot
        {
            get
            {
                return this.DataProxy.IsSnapshot; //通过数据代理我们得知数据是否为快照
            }
        }

        /// <summary>
        /// 对象是否为一个镜像
        /// </summary>
        public bool IsMirror
        {
            get
            {
                return this.DataProxy.IsMirror;
            }
        }

        /// <summary>
        /// 表示对象是否来自仓储中的快照区域，请注意该属性与IsSnapshot的区别：
        /// <para>一个对象有可能从仓储加载后，由于其他线程修改了它在仓储里对应的数据，导致这个对象由非快照状态变为快照</para>
        /// <para>这时候IsSnapshot为true，IsFromSnapshot为false</para>
        /// <para>也就是说，IsSnapshot比IsFromSnapshot更加严谨，是一定可以识别对象的快照状态的</para>
        /// <para>但是IsFromSnapshot的性能要比IsSnapshot好的多，它表示对象被删除后，加入到仓储的快照区域了，当对象再次被加载，它的IsSnapshot和IsFromSnapshot属性都被true</para>
        /// </summary>
        public bool IsFromSnapshot
        {
            get
            {
                return this.DataProxy.IsFromSnapshot; //通过数据代理我们得知数据是否为来自快照区域
            }
        }

        /// <summary>
        /// 对象是否为无效的，对象无效的原因可能有很多（比如：是个快照对象，或者其成员属性是快照，导致自身无效化了）
        /// 默认情况下，快照对象就是一个无效对象，可以通过重写该属性的算法，针对某些业务判定对象是无效的
        /// 无效的对象只能被删除，不能新增和修改
        /// </summary>
        public virtual bool Invalid
        {
            get
            {
                return this.IsSnapshot;
            }
        }

        #endregion

        public virtual bool IsEmpty()
        {
            //默认情况下领域对象是非空的
            return false;
        }

        public bool IsNull()
        {
            return this.IsEmpty();
        }

        #region 状态

        private StateMachine _machine = new StateMachine();

        /// <summary>
        /// 是否为脏对象
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _machine.IsDirty || HasDirtyProperty();
            }
        }

        /// <summary>
        /// 内存中新创建的对象
        /// </summary>
        public bool IsNew
        {
            get
            {
                return _machine.IsNew;
            }
        }

        /// <summary>
        /// 该对象是否被改变
        /// </summary>
        /// <returns></returns>
        public bool IsChanged
        {
            get
            {
                return _machine.IsChanged;
            }
        }

        public void MarkDirty()
        {
            _machine.MarkDirty();
        }

        public void MarkNew()
        {
            _machine.MarkNew();
        }

        /// <summary>
        /// 设置对象为干净的
        /// </summary>
        public virtual void MarkClean()
        {
            _machine.MarkClean();
            //当对象为干净对象时，那么对象的所有内聚成员应该也是干净的
            MarkCleanProperties();
        }

        private void MarkCleanProperties()
        {
            InvokeProperties((obj) => { obj.MarkClean(); });
        }

        private StateMachine _backupMachine;

        public void SaveState()
        {
            SaveStateProperties();
            if (_backupMachine == null)
            {
                _backupMachine = _machine.Clone();
            }
            else
            {
                _backupMachine.Combine(_machine);
            }
        }

        private void SaveStateProperties()
        {
            InvokeProperties((obj) => { obj.SaveState(); });
        }

        /// <summary>
        /// 加载备份的状态，此时备份会被删除
        /// </summary>
        public void LoadState()
        {
            LoadStateProperties();
            if (_backupMachine != null)
            {
                _machine = _backupMachine;
                _backupMachine = null;  //加载状态后，将备份给删除
            }
        }

        private void LoadStateProperties()
        {
            InvokeProperties((obj) => { obj.LoadState(); });
        }

        /// <summary>
        /// 当已存在状态备份时，如果有属性发生改变，那么需要同步状态的备份，这样才不会造成BUG
        /// </summary>
        /// <param name="propertyName"></param>
        private void DuplicateMachineSetPropertyChanged(string propertyName)
        {
            if (_backupMachine != null) _backupMachine.SetPropertyChanged(propertyName);
        }

        private void DuplicateMachineClearPropertyChanged(string propertyName)
        {
            if (_backupMachine != null) _backupMachine.ClearPropertyChanged(propertyName);
        }

        /// <summary>
        /// 根据对比结果，设置属性是否被更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        //private bool SetPropertyChanged<T>(DomainProperty property, T oldValue, T newValue)
        //{
        //    //要用Equals判断
        //    if (object.Equals(oldValue, newValue)) return false;
        //    _machine.SetPropertyChanged(property.Name);
        //    DuplicateMachineSetPropertyChanged(property.Name);
        //    return true;
        //}

        /// <summary>
        /// 强制设置属性已被更改
        /// </summary>
        /// <param name="propertyName"></param>
        internal void SetPropertyChanged(DomainProperty property)
        {
            _machine.SetPropertyChanged(property.Name);
            DuplicateMachineSetPropertyChanged(property.Name);
        }

        /// <summary>
        /// 清除属性被改变的状态
        /// </summary>
        /// <param name="property"></param>
        protected void ClearPropertyChanged(DomainProperty property)
        {
            _machine.ClearPropertyChanged(property.Name);
            DuplicateMachineClearPropertyChanged(property.Name);
        }

        /// <summary>
        /// 判断属性是否被更改，对于领域对象属性是否改变的约定：
        /// 1.如果属性为普通类型（int、string等基础类型）,根据值是否发生了改变来判定属性是否改变
        /// 2.如果属性为值对象类型（ValueObject）,根据ValueObject的值是否发生了改变来判定属性是否改变
        /// 3.如果属性为实体对象类型（EntityObject,EntityObjectPro,Aggregate）,只要赋值，属性就会发生改变；
        ///   实体对象内部的属性发生改变，虽然引用关系没有变，但是我们认为属性还是发生变化了；
        /// 4.如果属性为实体对象类型的集合（ObjectCollection(EntityObject),ObjectCollection(EntityObjectPro),ObjectCollection(Aggregate)）,只要赋值，属性就会发生改变；
        ///   单个实体对象内部的属性发生改变，不影响外部属性的变化，因为他们的引用关系并没有被改变；
        ///   实体对象集合的成员发生了变化，那么属性被改变，因为集合代表的是引用对象和多个被对象之间的关系，集合成员变化了，代表引用关系也变化了
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsPropertyChanged(DomainProperty property)
        {
            return _machine.IsPropertyChanged(property.Name);
        }

        /// <summary>
        /// 仅仅只是属性<paramref name="property"/>发生了改变
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool OnlyPropertyChanged(DomainProperty property)
        {
            return _machine.OnlyPropertyChanged(property.Name);
        }

        /// <summary>
        /// 判断属性是否被更改
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsPropertyChanged(string propertyName)
        {
            var property = DomainProperty.GetProperty(this.ObjectType, propertyName);
            return IsPropertyChanged(property);
        }

        /// <summary>
        /// 属性是否为脏的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IsPropertyDirty(DomainProperty property)
        {
            if (this.IsNew) return true; //如果是新建对象，那么属性一定是脏的
            var isChanged = IsPropertyChanged(property);
            if (isChanged) return true; //属性如果被改变，那么我们认为就是脏的，这是一种粗略的判断，因为就算属性改变了，有可能经过2次修改后变成与数据库一样的值，这样就不是脏的了，但是为了简单化处理，我们仅认为只要改变了就是脏的，这种判断不过可以满足100%应用
            //如果属性没有被改变，那么我们需要进一步判断属性的成员是否发生改变
            switch (property.DomainPropertyType)
            {
                case DomainPropertyType.ValueObject:
                case DomainPropertyType.EntityObject:
                    {
                        DomainObject obj = null;
                        if (TryGetValue<DomainObject>(property, ref obj))
                        {
                            //如果加载了，就进一步判断
                            return obj.IsDirty;
                        }
                    }
                    break;
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.EntityObjectList:
                    {
                        IEnumerable list = null;
                        if (TryGetValue<IEnumerable>(property, ref list))
                        {
                            //如果加载了，就进一步判断
                            foreach (DomainObject obj in list)
                            {
                                if (obj.IsDirty) return true;
                            }
                        }
                    }
                    break;
            }
            //AggregateRoot和AggregateRootList 只用根据IsPropertyChanged判断即可，因为他们是外部内聚根对象，是否变脏与本地内聚根没有关系
            return false;
        }

        public bool IsPropertyDirty(string propertyName)
        {
            var property = DomainProperty.GetProperty(this.ObjectType, propertyName);
            return IsPropertyDirty(property);
        }

        /// <summary>
        /// 是否有脏的属性
        /// </summary>
        /// <returns></returns>
        private bool HasDirtyProperty()
        {
            var properties = DomainProperty.GetProperties(this.ObjectType);
            foreach (var property in properties)
            {
                if (IsPropertyDirty(property)) return true;
            }
            return false;
        }

        /// <summary>
        /// 在有效的属性对象上执行方法，只有被加载了的对象才执行
        /// </summary>
        /// <param name="action"></param>
        private void InvokeProperties(Action<DomainObject> action)
        {
            var properties = DomainProperty.GetProperties(this.ObjectType);
            foreach (var property in properties)
            {
                switch (property.DomainPropertyType)
                {
                    case DomainPropertyType.EntityObject:
                    case DomainPropertyType.ValueObject:
                        {
                            DomainObject obj = null;
                            if (TryGetValue<DomainObject>(property, ref obj))
                            {
                                action(obj);
                            }
                        }
                        break;
                    case DomainPropertyType.EntityObjectList:
                    case DomainPropertyType.ValueObjectList:
                        {
                            IEnumerable list = null;
                            if (TryGetValue<IEnumerable>(property, ref list))
                            {
                                foreach (DomainObject obj in list)
                                {
                                    action(obj);
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        private object _syncObject = new object();

        #region 数据代理

        private IDataProxy _dataProxy;
        public IDataProxy DataProxy
        {
            get
            {
                if (_dataProxy == null)
                {
                    lock(_syncObject)
                    {
                        if (_dataProxy == null)
                        {
                            _dataProxy = CodeArt.DomainDriven.DataProxy.CreateStorage(this);
                        }
                    }
                }
                return _dataProxy;
            }
            set
            {
                if (_dataProxy != null && value != null) value.Copy(_dataProxy);
                _dataProxy = value;
                if (_dataProxy != null) _dataProxy.Owner = this;
            }
        }

        public bool IsPropertyLoaded(DomainProperty property)
        {
            return this.DataProxy.IsLoaded(property);
        }

        public bool IsPropertyLoaded(string propertyName)
        {
            var property = DomainProperty.GetProperty(this.ObjectType, propertyName);
            return this.DataProxy.IsLoaded(property);
        }

        public int DataVersion
        {
            get
            {
                return this.DataProxy.Version;
            }
        }

        /// <summary>
        /// 同步数据版本号，当确认对象是干净的情况下，你可以手动更新版本号，这在某些情况下很有用
        /// </summary>
        public void SyncDataVersion()
        {
            this.DataProxy.SyncVersion();
        }

        #endregion

        #region 固定规则

        /// <summary>
        /// 固定规则
        /// </summary>
        public IFixedRules FixedRules
        {
            get
            {
                return DomainDriven.FixedRules.Instance;
            }
        }

        /// <summary>
        /// 验证固定规则
        /// </summary>
        /// <returns></returns>
        public virtual ValidationResult Validate()
        {
            return this.FixedRules.Validate(this);
        }

        private static Func<Type, IEnumerable<IObjectValidator>> _getValidators = LazyIndexer.Init<Type, IEnumerable<IObjectValidator>>((objectType) =>
        {
            return ObjectValidatorAttribute.GetValidators(objectType);
        });

        private IEnumerable<IObjectValidator> _validators;

        /// <summary>
        /// 对象验证器
        /// </summary>
        public IEnumerable<IObjectValidator> Validators
        {
            get
            {
                if (_validators == null)
                {
                    _validators = _getValidators(this.ObjectType);
                }
                return _validators;
            }
        }



        #endregion

        #region 执行上下文

        private Dictionary<Guid, RunContext> _ctxs = new Dictionary<Guid, RunContext>();

        private RunContext GetRunContext(Guid runId)
        {
            RunContext ctx = null;
            if (!_ctxs.TryGetValue(runId, out ctx))
            {
                lock (_ctxs)
                {
                    if (!_ctxs.TryGetValue(runId, out ctx))
                    {
                        ctx = new RunContext();
                        _ctxs.Add(runId, ctx);
                    }
                }
            }
            return ctx;
        }


        private RunContext GetRunContextFromAppSession(Guid runId)
        {
            var ctxs = AppSession.GetOrAddItem("RunContext", () =>
            {
                var pool = DictionaryPool<Guid, RunContext>.Instance;
                return Symbiosis.TryMark<Dictionary<Guid, RunContext>>(pool, () =>
                {
                    return new Dictionary<Guid, RunContext>();
                });
            });

            RunContext ctx = null;
            if (!ctxs.TryGetValue(runId, out ctx))
            {
                lock (ctxs)
                {
                    if (!ctxs.TryGetValue(runId, out ctx))
                    {
                        ctx = new RunContext();
                        ctxs.Add(runId, ctx);
                    }
                }
            }
            return ctx;
        }


        #endregion

        #region 领域属性

        /// <summary>
        /// 请保证更改对象状态时的并发安全
        /// 该方法虽然是公开的，但是<paramref name="property"/>都是由类内部或者扩展类定义的，
        /// 所以获取值和设置值只能通过常规的属性操作，无法通过该方法
        /// </summary>
        public virtual void SetValue(DomainProperty property, object value)
        {
            var ctx = GetRunContext(property.RuntimeSetId);
            property.SetChain.Invoke(this, value, ctx);
        }

        /// <summary>
        /// SetValue的最后一步
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        internal void SetValueLastStep(DomainProperty property, object value)
        {
            var oldValue = this.DataProxy.Load(property);
            bool isChanged = false;


            if (property.IsChanged(oldValue, value))
            {
                isChanged = true;
                this.SetPropertyChanged(property);
            }


            //if (property.ChangedMode == PropertyChangedMode.Compare)
            //{
            //    if (this.SetPropertyChanged(property, oldValue, value))
            //    {
            //        isChanged = true;
            //    }
            //}
            //else if (property.ChangedMode == PropertyChangedMode.Definite)
            //{
            //    this.SetPropertyChanged(property);
            //    isChanged = true;
            //}

            if (isChanged)
            {
                var collection = value as IDomainCollection;
                if (collection != null)
                    collection.Parent = this;

                this.DataProxy.Save(property, value, oldValue);

                Logable.Write(property, value, oldValue);

                HandlePropertyChanged(property, value, oldValue);
            }
        }

        /// <summary>
        /// 该方法虽然是公开的，但是<paramref name="property"/>都是由类内部或者扩展类定义的，
        /// 所以获取值和设置值只能通过常规的属性操作，无法通过该方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public T GetValue<T>(DomainProperty property)
        {
            var value = GetValue(property);
            if (value == null) throw new IsNullException(property.Name);
            //if(value is IEmptyable)
            //{
            //    if(!typeof(T).ImplementInterface(typeof(IEmptyable)))
            //    {
            //        return (T)((value as IEmptyable)).GetValue();
            //    }
            //}
            return (T)value;
        }

        /// <summary>
        /// 当属性的值已经被加载，就获取数据，否则不获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool TryGetValue<T>(DomainProperty property, ref T value) where T : class
        {
            if (this.DataProxy.IsLoaded(property))
            {
                value = GetValue<T>(property);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 我们保证领域对象的读操作是线程安全的
        /// 该方法虽然是公开的，但是<paramref name="property"/>都是由类内部或者扩展类定义的，
        /// 所以获取值和设置值只能通过常规的属性操作，无法通过该方法
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual object GetValue(DomainProperty property)
        {
            var ctx = GetRunContextFromAppSession(property.RuntimeGetId); //从当前应用程序会话中获取运行上下文，确保Get操作不会引起并发冲突
            return property.GetChain.Invoke(this, ctx);
        }

        /// <summary>
        /// 获得属性最后一次被更改前的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object GetOldValue(DomainProperty property)
        {
            return this.DataProxy.LoadOld(property);
        }

        internal object GetValueLastStep(DomainProperty property)
        {
            return this.DataProxy.Load(property);
        }



        /// <summary>
        /// 外界标记某个属性发生了变化，这常常是由于属性本身的成员发生了变化，而触发的属性变化
        /// 该方法会触发属性被改变的事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        public void MarkPropertyChanged(DomainProperty property)
        {
            if (!this.IsPropertyLoaded(property)) return;

            this.SetPropertyChanged(property);
            var value = this.GetValue(property);
            HandlePropertyChanged(property, value, value);
        }

        /// <summary>
        /// 处理属性被改变时的行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        private void HandlePropertyChanged<T>(DomainProperty property, T newValue, T oldValue)
        {
            ReadOnlyCheckUp();
            RaisePropertyChanged(property, newValue, oldValue);
            RaiseChangedEvent();
        }


        private void RaisePropertyChanged<T>(DomainProperty property, T newValue, T oldValue)
        {
            if (IsConstructing) return;//构造时，不触发任何事件

            if (!property.IsRegisteredChanged && this.PropertyChanged == null) return;

            var ctx = GetRunContext(property.RuntimeChangedId);

            if (!ctx.InCallBack)
            {
                var args = new DomainPropertyChangedEventArgs(property, newValue, oldValue);
                //先进行对象内部回调方法
                ctx.InCallBack = true;
                property.ChangedChain.Invoke(this, args);
                args.NewValue = this.GetValue(property);//同步数据
                ctx.InCallBack = false;

                //最后执行对象级别挂载的事件
                if (this.PropertyChanged != null)
                {
                    ctx.InCallBack = true;
                    this.PropertyChanged(this, args);
                    args.NewValue = this.GetValue(property);//同步数据
                    ctx.InCallBack = false;
                }
            }
        }

        public event DomainPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 得到被更改了的领域属性的信息
        /// </summary>
        public IEnumerable<(DomainProperty Property,object newValue,object oldValue)> GetChangedProperties()
        {
            var properties = DomainProperty.GetProperties(this.ObjectType);
            List<(DomainProperty Property, object newValue, object oldValue)> items = new List<(DomainProperty Property, object newValue, object oldValue)>();
            foreach (var property in properties)
            {
                if (this.IsPropertyChanged(property))
                {
                    var newValue = this.GetValue(property);
                    var oldValue = this.GetOldValue(property);
                    items.Add((property, newValue, oldValue));
                }
            }
            return items;
        }


        #endregion

        #region 构造

        /// <summary>
        /// 对象构造完毕后的事件
        /// </summary>
        public event DomainObjectConstructedEventHandler Constructed;

        private int _constructedDepth;

        public bool IsConstructing
        {
            get;
            private set;
        }

        /// <summary>
        /// 指示对象已完成构造，请务必在领域对象构造完成后调用此方法
        /// </summary>
        protected void OnConstructed()
        {
            _constructedDepth++;

            if (_constructedDepth > this.TypeDepth)
            {
                throw new DomainDrivenException("构造对象发生未知的错误，构造层级大于对象实际类型层级");
            }

            if (_constructedDepth == this.TypeDepth) //已构造完毕
            {
                IsConstructing = false;
                this.RaiseConstructedEvent();
            }
        }

        private void RaiseConstructedEvent()
        {
            if (this.IsEmpty()) return;
            if (this.Constructed != null)
            {
                var args = new DomainObjectConstructedEventArgs(this);
                this.Constructed(this, args);
            }
            //执行边界事件
            StatusEvent.Execute(this.ObjectType, StatusEventType.Constructed, this);
        }

        #endregion

        public event DomainObjectChangedEventHandler Changed;

        private void RaiseChangedEvent()
        {
            if (this.IsConstructing) return;//构造时，不触发任何事件
            if (this.IsEmpty()) return;//空对象，不触发任何事件

            OnChanged();
            //执行边界事件
            StatusEvent.Execute(this.ObjectType, StatusEventType.Changed, this);
        }

        protected virtual void OnChanged()
        {
            if (this.Changed != null)
            {
                var args = new DomainObjectChangedEventArgs(this);
                this.Changed(this, args);
            }
        }

        #region 辅助

        internal readonly static Type ValueObjectType = typeof(IValueObject);
        internal readonly static Type AggregateRootType = typeof(IAggregateRoot);
        internal readonly static Type EntityObjectType = typeof(IEntityObject);
        internal readonly static Type DomainObjectType = typeof(IDomainObject);
        internal readonly static Type DynamicObjectType = typeof(IDynamicObject);

        internal static bool IsDomainObject(Type type)
        {
            return type.IsImplementOrEquals(DomainObjectType);
        }

        internal static bool IsValueObject(Type type)
        {
            return type.IsImplementOrEquals(ValueObjectType);
        }

        internal static bool IsAggregateRoot(Type type)
        {
            return type.IsImplementOrEquals(AggregateRootType);
        }

        internal static bool IsEntityObject(Type type)
        {
            return type.IsImplementOrEquals(EntityObjectType);
        }

        internal static bool IsDynamicObject(Type type)
        {
            return type.IsImplementOrEquals(DynamicObjectType);
        }


        #endregion

        protected virtual void ReadOnlyCheckUp()
        {
            if (this.IsConstructing) return; //构造阶段不处理

            if (this.IsEmpty()) throw new DomainDrivenException(string.Format(Strings.EmptyReadOnly, this.ObjectType.FullName));
            if (this.IsFromSnapshot) throw new DomainDrivenException(string.Format(Strings.SnapshotReadOnly, this.ObjectType.FullName));
        }

        #region 辅助方法

        private static readonly BindingFlags _flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// 通过调用一个静态成员，来模拟触发领域对象类型或扩展类型的静态构造函数
        /// <para>如果类型没有静态成员，那么就算不触发静态构造也不会影响，因为不需要注入领域属性和空对象</para>
        /// </summary>
        /// <param name="objectOrExtensionType"></param>
        internal static void StaticConstructor(Type objectOrExtensionType)
        {
            //当new一个实例时，静态构造会从基类依次执行,当时如果仅仅只是获得子类的静态成员，那么是不会触发基类的静态构造函数的
            //因此，我们需要从基类开始，依次调用
            var types = objectOrExtensionType.GetInheriteds();
            foreach (var type in types)
            {
                if (!type.IsImplementOrEquals(typeof(IDomainObject))) continue;

                var member = type.GetPropertyAndFields(_flags).FirstOrDefault();
                if (member != null)
                    objectOrExtensionType.GetStaticValue(member.Name);
            }

            //再触发自身
            {
                var member = objectOrExtensionType.GetPropertyAndFields(_flags).FirstOrDefault();
                if (member != null)
                    objectOrExtensionType.GetStaticValue(member.Name);
            }

        }


        public static bool IsFrameworkDomainType(Type objectType)
        {
            if (IsDynamicObject(objectType)) return true;

            //因为框架提供的基类没有标记ObjectRepositoryAttribute
            return IsDomainObject(objectType) &&
                  objectType.IsDefined(typeof(FrameworkDomainAttribute), false);
        }

        public static bool IsMergeDomainType(Type objectType)
        {
            return objectType.IsDefined(typeof(MergeDomainAttribute), false);
        }

        /// <summary>
        /// 获取领域类型定义的空对象
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        internal static object GetEmpty(Type objectType)
        {
            var runtimeType = objectType as RuntimeObjectType;
            if (runtimeType != null) return runtimeType.Define.EmptyInstance;

            return _getObjectEmpty(objectType);
        }

        /// <summary>
        /// 判断类型是否表示一个空的领域对象
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool IsEmpty(Type objectType)
        {
            return objectType.Name.EndsWith("Empty");//为了不触发得到空对象带来的连锁构造，我们简单的认为空对象的名称末尾是 Empty即可，这也是CA的空对象定义约定
            //objectType = RuntimeObjectType.GetDomainType(objectType);
            //return GetEmpty(objectType).GetType() == objectType;
        }


        private static Func<Type, object> _getObjectEmpty = LazyIndexer.Init<Type, object>((objectType) =>
        {
            var empty = objectType.GetStaticValue("Empty");
            if (empty == null)
                throw new DomainDrivenException(string.Format(Strings.NotFoundEmpty, objectType.FullName));
            return empty;
        });

        /// <summary>
        /// 当前应用程序中所有的领域类型
        /// </summary>
        public static IEnumerable<Type> TypeIndex
        {
            get;
            private set;
        }

        private static IEnumerable<Type> GetTypeIndex()
        {
            var types = AssemblyUtil.GetImplementTypes(typeof(IDomainObject));
            List<Type> doTypes = new List<Type>();
            foreach (var type in types)
            {
                if (!IsMergeDomainType(type))
                {
                    var exists = doTypes.FirstOrDefault((t) =>
                    {
                        return t.Name == type.Name;
                    });

                    if (exists != null)
                        throw new DomainDrivenException(string.Format("领域对象 {0} 和 {1} 重名", type.FullName, exists.FullName));
                    doTypes.Add(type);
                }
            }
            return doTypes;
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
            var property = DomainProperty.GetProperty(this.ObjectType, propertyName);
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
            var property = DomainProperty.GetProperty(this.ObjectType, propertyName);
            SetValue(property, value);
            return true;
        }


        public override bool TryConvert(System.Dynamic.ConvertBinder binder, out object result)
        {
            result = this;
            return true;
        }


        #endregion

        private static bool _initialized = false;

        /// <summary>
        /// <para>初始化与领域对象相关的行为，由于许多行为是第一次使用对象的时候才触发，这样会导致没有使用对象却需要一些额外特性的时候缺少数据</para>
        /// <para>所以我们需要在使用领域驱动的之前执行初始化操作</para>
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            //以下代码执行顺序不能变
            TypeIndex = GetTypeIndex();

            //指定动态类型的定义
            RemoteType.Initialize();

            //执行远程能力特性的初始化，收集相关数据
            RemotableAttribute.Initialize();

            //执行日志能力特性的初始化，收集相关数据
            ObjectLogableAttribute.Initialize();

            //触发没有派生类的领域对象的静态构造
            foreach (var objectType in TypeIndex)
            {
                if (DerivedClassAttribute.IsDerived(objectType)) continue; //不执行派生类的
                DomainObject.StaticConstructor(objectType);
            }

            //先执行派生类的
            DerivedClassAttribute.Initialize();

            //再执行扩展的
            ExtendedClassAttribute.Initialize();

            //远程服务的初始化
            RemoteService.Initialize();

            //领域事件宿主的初始化
            EventHost.Initialize();
        }

        /// <summary>
        /// 初始化之后
        /// </summary>
        internal static void Initialized()
        {
            EventHost.Initialized();
        }

        internal static void Cleanup()
        {
            RemoteService.Cleanup();

            EventHost.Cleanup();
        }

        /// <summary>
        /// 检查领域对象是否已被初始化了
        /// </summary>
        internal static void CheckInitialized()
        {
            if (!_initialized)
            {
                throw new DomainDrivenException(Strings.UninitializedDomainObject);
            }
        }

    }
}