using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.AOP;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 表示参与依赖项属性系统的对象。DependencyObject 是许多重要的 UI 相关类的直接基类
    /// 1.每个xaml页面，对应一组依赖对象，每次访问该页面，是重复使用这一组依赖对象
    /// 2.不同xaml页面之间的依赖对象是不同的实例，除静态成员外，互不影响
    /// </summary>
    [Aspect(typeof(WebPageInitializer))]
    public class DependencyObject : IDependencyObject, ICurable
    {

        static DependencyObject()
        {
        }

        #region 用户数据

        /// <summary>
        /// 用户数据是当前线程所有组件共享的
        /// </summary>
        private ConcurrentDictionary<string, object> UserData
        {
            get
            {
                const string key = "DependencyObject_UserData";

                var item = AppSession.GetOrAddItem(key,
                   () =>
                   {
                       return new ConcurrentDictionary<string, object>();
                   });
                if (item == null) throw new InvalidOperationException("DependencyObject_UserData为null,无法使用xaml组件");
                return item;

                //var item = HttpContext.Current.Items[key];
                //if (item == null)
                //{
                //    item = new ConcurrentDictionary<string, object>();
                //    HttpContext.Current.Items[key] = item;
                //}
                //return item as ConcurrentDictionary<string, object>;
            }
        }

        /// <summary>
        /// 设置用户数据（User Data）
        /// 用户数据是仅对当前访问者有效的线程安全数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetUDItem(string name, object value)
        {
            this.UserData.AddOrUpdate(name, value, (n, v) =>
            {
                return value;
            });
        }

        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetUDItem(string name)
        {
            object value = null;
            if (this.UserData.TryGetValue(name, out value)) return value;
            return null;
        }

        #endregion

        /// <summary>
        /// 依赖对象编号，全局唯一
        /// </summary>
        public Guid ObjectId { get; private set; }

        public Type ObjectType
        {
            get;
            private set;
        }


        public DependencyObject()
        {
            this.ObjectId = Guid.NewGuid();
            this.ObjectType = this.GetType();
            _runContenxtsKey = string.Format("RunContexts_{0}", this.ObjectId);
            _propertyData = new DataAccess(this.ObjectId, "property");
            _expressionData = new DataAccess(this.ObjectId, "expression");
            _pinnedDataLoader = new PinnedDataLoader(this.ObjectId);
        }


        #region 执行上下文

        private string _runContenxtsKey;

        private Dictionary<Guid, RunContext> RunContenxts
        {
            get
            {
                var item = AppSession.GetOrAddItem(
                   _runContenxtsKey,
                   () =>
                   {
                       return new Dictionary<Guid, RunContext>();
                   });
                if (item == null) throw new InvalidOperationException("RunContenxts为null,无法使用xaml组件");
                return item;


                //var item = HttpContext.Current.Items[_runContenxtsKey];
                //if (item == null)
                //{
                //    item = new Dictionary<Guid, RunContext>();
                //    HttpContext.Current.Items[_runContenxtsKey] = item;
                //}
                //return item as Dictionary<Guid, RunContext>;
            }
        }

        private RunContext GetContext(Guid runId)
        {
            var ctxs = this.RunContenxts;
            RunContext ctx = null;
            if (!ctxs.TryGetValue(runId, out ctx))
            {
                ctx = new RunContext();
                ctxs.Add(runId, ctx);
            }
            return ctx;
        }

        #endregion

        #region 属性

        private DataAccess _propertyData;

        public bool SetValue(string propertyName, object propertyValue)
        {
            return SetValue(propertyName, propertyValue, false);
        }

        /// <summary>
        /// 根据名称设置属性的值，成功赋值返回true
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        internal virtual bool SetValue(string propertyName, object propertyValue, bool ignoreExpression)
        {
            var objType = this.ObjectType;
            var dp = DependencyProperty.GetProperty(objType, propertyName);
            if (dp == null)
            {
                var propertyInfo = objType.ResolveProperty(propertyName);
                if (propertyInfo == null)
                {
                    return false;
                    //throw new XamlException("没有找到" + propertyName + "的定义");
                }
                propertyInfo.SetValue(this, propertyValue);
            }
            else this.SetValue(dp, propertyValue, ignoreExpression);
            return true;
        }

        private object LoadPropertyValue(DependencyProperty dp, bool ignoreExpression)
        {
            if (!ignoreExpression)
            {
                var exp = GetExpression(dp);
                if (exp != null) return exp.GetValue(this, dp);
            }
            return _propertyData.Load(dp.Id, dp.GetDefaultValue);
        }

        private void SavePropertyValue(DependencyProperty dp, object newValue,bool ignoreExpression)
        {
            if (!ignoreExpression)
            {
                var exp = GetExpression(dp);
                if (exp != null)
                {
                    exp.SetValue(this, dp, newValue);
                    return;
                }
            }
            _propertyData.Save(dp.Id, newValue);
        }

        /// <summary>
        /// 属性是否为表达式的定义
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public bool IsExpression(DependencyProperty dp)
        {
            return GetExpression(dp) != null;
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            SetValue(dp, value, false);
        }

        /// <summary>
        /// ignoreExpression为true表示如果value是个表达式，那么不再执行表达式的SetValue，而是直接将值存到对象内部
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        /// <param name="ignoreExpression">为true表示如果value是个表达式，那么不再执行表达式的SetValue，而是直接将值存到对象内部</param>
        internal void SetValue(DependencyProperty dp, object value,bool ignoreExpression)
        {
            var exp = value as Expression;
            if (exp != null)
            {
                this.ClearExpression(dp);//先移除之前的表达式
                this.SetExpression(dp, exp);
                return;
            }

            var ctx = GetContext(dp.Id);

            if (!ctx.InCallBack)
            {
                //先进行对象内部回调方法
                if (dp.PreSetValueCallback != null)
                {
                    ctx.InCallBack = true;
                    var result = dp.PreSetValueCallback(this, ref value);
                    ctx.InCallBack = false;

                    if (!result) return; //false表示不再赋值
                }

                if (dp.IsRegisteredPreSet || this.PropertyPreSet != null)
                {
                    var e = new DependencyPropertyPreSetEventArgs(dp, value);
                    //再执行属性公共挂载的事件
                    if (dp.IsRegisteredPreSet)
                    {
                        ctx.InCallBack = true;
                        dp.OnPreSet(this, e);
                        ctx.InCallBack = false;

                        if (!e.Allow) return; //不再赋值
                        value = e.Value;
                    }

                    //最后执行对象级别挂载的事件
                    if (this.PropertyPreSet != null)
                    {
                        ctx.InCallBack = true;
                        this.PropertyPreSet(this, e);
                        ctx.InCallBack = false;

                        if (!e.Allow) return; //不再赋值
                        value = e.Value;
                    }
                }
            }

            var oldValue = LoadPropertyValue(dp, ignoreExpression);
            if (!object.Equals(oldValue, value))
            {
                SavePropertyValue(dp, value, ignoreExpression);
                OnPropertyChanged(dp, value, oldValue);
            }
        }

        public bool GetValue(string propertyName, ref object value)
        {
            return GetValue(propertyName, ref value, false);
        }

        public object GetValue(string propertyName)
        {
            object value = null;
            GetValue(propertyName, ref value, false);
            return value;
        }

        internal virtual bool GetValue(string propertyName, ref object value, bool ignoreExpression)
        {
            var objType = this.ObjectType;
            var dp = DependencyProperty.GetProperty(objType, propertyName);
            if (dp == null)
            {
                var propertyInfo = objType.ResolveProperty(propertyName);
                if (propertyInfo == null)
                {
                    return false;
                    //throw new XamlException("没有找到" + propertyName + "的定义");
                }
                value = propertyInfo.GetValue(this);
            }
            value = this.GetValue(dp, ignoreExpression);
            return true;
        }

        public object GetValue(DependencyProperty dp)
        {
            return GetValue(dp, false);
        }

        internal object GetValue(DependencyProperty dp,bool ignoreExpression)
        {
            var ctx = GetContext(dp.Id);

            var value = LoadPropertyValue(dp, ignoreExpression);

            if (!ctx.InCallBack)
            {
                //先进行对象内部回调方法
                if (dp.GotValueCallback != null)
                {
                    ctx.InCallBack = true;
                    dp.GotValueCallback(this, ref value);
                    ctx.InCallBack = false;
                }

                if (dp.IsRegisteredGot || this.PropertyGot != null)
                {
                    var e = new DependencyPropertyGotEventArgs(dp, value);
                    //再执行属性公共挂载的事件
                    if (dp.IsRegisteredGot)
                    {
                        ctx.InCallBack = true;
                        dp.OnGot(this, e);
                        ctx.InCallBack = false;

                        value = e.Value;
                    }

                    //最后执行对象级别挂载的事件
                    if (this.PropertyGot != null)
                    {
                        ctx.InCallBack = true;
                        this.PropertyGot(this, e);
                        ctx.InCallBack = false;

                        value = e.Value;
                    }
                }
            }
            return value;
        }

        internal void OnPropertyChanged(DependencyProperty dp, object newValue, object oldValue)
        {
            var handlers = GetPropertyChangedHandlers(dp);

            if ((handlers == null || handlers.Count == 0)
                && dp.ChangedCallBack == null && !dp.IsRegisteredChanged && this.PropertyChanged == null) return;

            var ctx = GetContext(dp.Id);

            if (!ctx.InCallBack)
            {
                var args = new DependencyPropertyChangedEventArgs(dp, newValue, oldValue);

                //先进行对象内部回调方法
                if (dp.ChangedCallBack != null)
                {
                    ctx.InCallBack = true;
                    dp.ChangedCallBack(this, args);
                    args.NewValue = this.GetValue(dp);//同步数据
                    ctx.InCallBack = false;
                }

                //再执行属性公共挂载的事件
                if (dp.IsRegisteredChanged)
                {
                    ctx.InCallBack = true;
                    dp.OnChanged(this, args);
                    args.NewValue = this.GetValue(dp); //同步数据
                    ctx.InCallBack = false;
                }

                if(handlers != null)
                {
                    foreach (var handler in handlers) handler(this, args);
                }

                //最后执行对象级别挂载的事件
                if (this.PropertyChanged != null)
                {
                    ctx.InCallBack = true;
                    this.PropertyChanged(this, args);
                    args.NewValue = this.GetValue(dp);//同步数据
                    ctx.InCallBack = false;
                }
            }
        }


        #region 轻量级的单个属性被改变的事件,提供该机制是为了更好的性能

        private MultiDictionary<DependencyProperty, DependencyPropertyChangedEventHandler> _propertyChangeds;

        /// <summary>
        /// 追加一个监视属性改变的处理器
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="handler"></param>
        public void AddPropertyChanged(DependencyProperty dp, DependencyPropertyChangedEventHandler handler)
        {
            if (_propertyChangeds == null) _propertyChangeds = new MultiDictionary<DependencyProperty, DependencyPropertyChangedEventHandler>(false);
            _propertyChangeds.Add(dp, handler);
        }

        /// <summary>
        /// 移除属性改变的事件处理器
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="handler"></param>
        public void RemovePropertyChanged(DependencyProperty dp, DependencyPropertyChangedEventHandler handler)
        {
            if (_propertyChangeds == null) return;
            _propertyChangeds.RemoveValue(dp, handler);
        }

        private IList<DependencyPropertyChangedEventHandler> GetPropertyChangedHandlers(DependencyProperty dp)
        {
            if (_propertyChangeds == null) return null;
            IList<DependencyPropertyChangedEventHandler> handlers = null;
            if (!_propertyChangeds.TryGetValue(dp, out handlers)) return null;
            return handlers;
        }


        /// <summary>
        /// 清理属性的值，移除表达式并设置值为默认值
        /// </summary>
        /// <param name="dp"></param>
        public void ClearValue(DependencyProperty dp)
        {
            ClearExpression(dp);
            this.SetValue(dp, dp.GetDefaultValue());
        }


        #endregion




        /// <summary>
        /// 尽量不要使用该事件，该事件会在任何一个属性被改变时就触发
        /// 如果要监视单独的属性改变，请使用AddPropertyChanged、RemovePropertyChanged方法，他们能提供更好的性能
        /// </summary>
        public event DependencyPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 事件是多个线程共享的
        /// </summary>
        public event DependencyPropertyPreSetEventHandler PropertyPreSet;

        /// <summary>
        /// 事件是多个线程共享的
        /// </summary>
        public event DependencyPropertyGotEventHandler PropertyGot;

        #endregion

        //#region 依赖行为

        ///// <summary>
        ///// 执行行为
        ///// </summary>
        ///// <param name="property"></param>
        ///// <returns></returns>
        //public object CallAction(DependencyAction action, params object[] args)
        //{
        //    var ctx = GetContext(action.Id);

        //    object returnValue = null;

        //    if (!ctx.InCallBack)
        //    {
        //        //先进行对象内部回调方法
        //        if (action.PreCallActionCallback != null)
        //        {
        //            bool allow = true;

        //            ctx.InCallBack = true;
        //            returnValue = action.PreCallActionCallback(this, args, ref allow);
        //            ctx.InCallBack = false;

        //            if (!allow) return returnValue;
        //        }

        //        if (action.IsRegisteredPreCall || this.ActionPreCall != null)
        //        {
        //            var e = new DependencyActionPreCallEventArgs(action, args);
        //            //再执行行为公共挂载的事件
        //            if (action.IsRegisteredPreCall)
        //            {
        //                ctx.InCallBack = true;
        //                action.OnPreCall(this, e);
        //                ctx.InCallBack = false;

        //                returnValue = e.ReturnValue;
        //                if (!e.Allow) return returnValue; //不再执行行为
        //            }

        //            //最后执行对象级别挂载的事件
        //            if (this.ActionPreCall != null)
        //            {
        //                ctx.InCallBack = true;
        //                this.ActionPreCall(this, e);
        //                ctx.InCallBack = false;

        //                returnValue = e.ReturnValue;
        //                if (!e.Allow) return returnValue; //不再执行行为
        //            }
        //        }
        //    }

        //    returnValue = action.Procedure(this, args);

        //    if (!ctx.InCallBack)
        //    {
        //        if (action.CalledActionCallback != null)
        //        {
        //            ctx.InCallBack = true;
        //            returnValue = action.CalledActionCallback(this, args, returnValue);
        //            ctx.InCallBack = false;
        //        }

        //        if (action.IsRegisteredCalled || this.ActionCalled != null)
        //        {
        //            var e = new DependencyActionCalledEventArgs(action, args, returnValue);
        //            //再执行行为公共挂载的事件
        //            if (action.IsRegisteredCalled)
        //            {
        //                ctx.InCallBack = true;
        //                action.OnCalled(this, e);
        //                ctx.InCallBack = false;

        //                returnValue = e.ReturnValue;
        //            }

        //            //最后执行对象级别挂载的事件
        //            if (this.ActionCalled != null)
        //            {
        //                ctx.InCallBack = true;
        //                this.ActionCalled(this, e);
        //                ctx.InCallBack = false;

        //                returnValue = e.ReturnValue;
        //            }
        //        }
        //    }
        //    return returnValue;
        //}

        //public event DependencyActionPreCallEventHandler ActionPreCall;

        //public event DependencyActionCalledEventHandler ActionCalled;

        //#endregion

        #region 标记扩展

        private DataAccess _expressionData = null;

        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <returns></returns>
        private Expression GetExpression(DependencyProperty dp)
        {
            return _expressionData.Load(dp.Id, null) as Expression;
        }

        /// <summary>
        /// 设置表达式
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="expression"></param>
        private void SetExpression(DependencyProperty dp, Expression expression)
        {
            _expressionData.Save(dp.Id, expression);
            if(expression != null) expression.OnAttach(this, dp);
        }

        /// <summary>
        /// 移除表达式
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="expression"></param>
        private void RemoveExpression(DependencyProperty dp, Expression expression)
        {
            SetExpression(dp, null);
            expression.OnDetach(this, dp);
        }

        /// <summary>
        /// 清理属性的表达式
        /// </summary>
        /// <param name="dp"></param>
        public void ClearExpression(DependencyProperty dp)
        {
            var exp = GetExpression(dp);
            if (exp != null) RemoveExpression(dp, exp);
        }


        #endregion

        /// <summary>
        /// 获取属性实际存贮的值信息
        /// 该方法会优先会返回表达式信息
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        internal object GetActualValue(DependencyProperty dp)
        {
            return this.GetExpression(dp) ?? this.GetValue(dp);
        }

        #region 固化

        private PinnedDataLoader _pinnedDataLoader;

        /// <summary>
        /// 将对象的固化值加载到本地数据中，每次访问页面会且仅会加载一次固化值
        /// </summary>
        public virtual void LoadPinned()
        {
            _pinnedDataLoader.Load(InitData);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void InitData()
        {
            //属性
            _propertyData.Restore(this.ObjectType);
            //标记扩展
            _expressionData.Restore(this.ObjectType);
            //_pinnedDataLoader 数据不需要初始化
        }

        #endregion


        public override int GetHashCode()
        {
            return this.ObjectId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as DependencyObject;
            if (target == null) return false;
            return target.ObjectId == this.ObjectId;
        }

        /// <summary>
        /// 组件初始化完毕后，触发该方法，一个实例该方法仅触发一次
        /// </summary>
        public virtual void OnInit() { }

        internal bool IsInited { get; set; }


        /// <summary>
        /// 在渲染组件之前，加载组件之后，触发该事件，每次执行页面都会触发实例的该事件
        /// </summary>
        public virtual void OnLoad() { }
    }
}
