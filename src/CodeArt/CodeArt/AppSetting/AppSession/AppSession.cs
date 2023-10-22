using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using CodeArt.Runtime;
using CodeArt.Log;
using CodeArt.DTO;
using System.Collections.Generic;
using System.Globalization;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 应用程序会话，指的是在应用程序执行期间，不同的用户会拥有自己的appSession，该对象仅对当前用户负责
    /// 不会有并发冲突，该对象内部的数据是当前用户独享的
    /// </summary>
    [AppSessionAccess]
    public static class AppSession
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Using(Action action,bool useSymbiosis)
        {
            try
            {
                Initialize();
                if (useSymbiosis) Symbiosis.Open();
                action();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (useSymbiosis) Symbiosis.Close();
                Dispose();
            }
        }

        public static void AsyncUsing(Action action, bool useSymbiosis)
        {
            Task.Run(() =>
            {
                try
                {
                    Initialize();
                    if (useSymbiosis) Symbiosis.Open();
                    action();
                }
                catch (Exception ex)
                {
                    Logger.Fatal(ex);
                }
                finally
                {
                    if (useSymbiosis) Symbiosis.Close();
                    Dispose();
                }
            });
        }


        /// <summary>
        /// 初始化回话
        /// </summary>
        public static void Initialize()
        {
            Current.Initialize();
            InitPreAppSessionStart();
        }

        #region 事件

        private static IEnumerable<PreAppSessionStartAttribute> _starts;
        private static IEnumerable<PreAppSessionEndAttribute> _ends;

        private static void InitEvents()
        {
            _starts = AssemblyUtil.GetAttributes<PreAppSessionStartAttribute>();
            _ends = AssemblyUtil.GetAttributes<PreAppSessionEndAttribute>();
        }


        private static void InitPreAppSessionStart()
        {
            foreach (var attr in _starts)
            {
                attr.Run();
            }
        }

        private static void InitPreAppSessionEnd()
        {
            foreach (var attr in _ends)
            {
                attr.Run();
            }
        }

        #endregion


        /// <summary>
        /// 清理当前回话数据
        /// </summary>
        public static void Dispose()
        {
            InitPreAppSessionEnd();
            Current.Dispose();
        }

        public static T GetOrAddItem<T>(string name, Func<T> factory)
        {
            var appSession = Current;
            object item = appSession.GetItem(name);
            if (item == null)
            {
                item = factory();
                appSession.SetItem(name, item);
            }
            return (T)item;
        }

        public static void SetItem<T>(string name, T value)
        {
            Current.SetItem(name, value);
        }

        public static object GetItem(string name)
        {
            return Current.GetItem(name);
        }

        public static T GetItem<T>(string name)
        {
            return (T)GetItem(name);
        }

        public static T GetItem<T>(string name,T defaultValue)
        {
            var appSession = Current;
            object item = appSession.GetItem(name);
            if (item == null)
            {
                return defaultValue;
            }
            return (T)item;
        }

        public static bool ContainsItem(string name)
        {
            return Current.ContainsItem(name);
        }

        private static IAppSession _current;

        private static IAppSession Current
        {
            get
            {
                if (_current == null)
                {
                    _current = _sessionByConfig ?? _sessionByRegister ?? ThreadSession.Instance;
                }
                return _current;
            }
        }


        /// <summary>
        /// 是否存在回话
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            return Current != null && Current.Initialized;
        }


        private static IAppSession _sessionByConfig;

        static AppSession()
        {
            var imp = Configuration.Current.AppSetting.AppSessionImplementer;
            if (imp != null)
            {
                var appSession = imp.GetInstance<IAppSession>();
                AppSessionAccessAttribute.CheckUp(appSession);
                _sessionByConfig = appSession;
            }
            InitEvents();
        }



        private static IAppSession _sessionByRegister;

        /// <summary>
        /// 注册一个应用程序会话对象，请保证<paramref name="appSession"/>是线程安全的
        /// </summary>
        /// <param name="appSession"></param>
        public static void Register(IAppSession appSession)
        {
            AppSessionAccessAttribute.CheckUp(appSession);
            _sessionByRegister = appSession;
        }

        /// <summary>
        /// 指示当前回话是否属于系统行为
        /// 比如由于业务逻辑，必须要设置某个角色时，这个角色不是公开的，系统行为就可以无视这点
        /// </summary>
        public static bool SystemBehavior
        {
            get
            {
                return (bool)AppSession.GetItem("SystemBehavior", false);
            }
            private set
            {
                AppSession.SetItem("SystemBehavior", value);
            }
        }

        /// <summary>
        /// 系统行为，开启系统行为的权限
        /// </summary>
        /// <param name="action"></param>
        public static void SystemAction(Action action)
        {
            SystemBehavior = true;
            action();
            SystemBehavior = false;
        }

        #region 会话级别的身份和语言

        /// <summary>
        /// 当前应用程序会话使用的身份
        /// </summary>
        public static DTObject Identity
        {
            get
            {
                return AppSession.GetItem("SessionIdentity") as DTObject;
            }
            set
            {
                AppSession.SetItem("SessionIdentity", value);
                if (IdentityChanged != null)
                    IdentityChanged(value);
            }
        }

        private static void InitIdentity()
        {
            if (Identity == null)
                Identity = DTObject.Create();
        }

        /// <summary>
        /// 当会话的身份发生改变时触发
        /// </summary>
        public static event Action<DTObject> IdentityChanged;


        /// <summary>
        /// 当前应用程序会话使用的语言
        /// </summary>
        public static string Language
        {
            get
            {
                return GetItem<string>("language", AppContext.LocalLanguage);
            }
            set
            {
                CultureInfo ci = LanguageUtil.GetCulture(value);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                SetItem("language", value);
            }
        }

        /// <summary>
        /// 租户编号
        /// </summary>
        public static long TenantId
        {
            get
            {
                return Identity == null ?  0 : Identity.GetValue<long>("tenantId", 0);
            }
            set
            {
                InitIdentity();
                Identity.SetValue("tenantId", value);
            }
        }


        /// <summary>
        /// 是否启用租户功能
        /// </summary>
        public static bool TenantEnabled
        {
            get
            {
                return Identity == null ? true : Identity.GetBooleanValue("TenantEnabled", true);
            }
            set
            {
                InitIdentity();
                Identity.SetValue("TenantEnabled", value);
            }
        }

        /// <summary>
        /// 负责人编号
        /// </summary>
        public static string PrincipalId
        {
            get
            {
                return Identity == null ? string.Empty : Identity.GetValue<string>("principalId", string.Empty);
            }
            set
            {
                InitIdentity();
                Identity.SetValue("principalId", value);
            }
        }


        /// <summary>
        /// 访客是否属于灰度中
        /// </summary>
        public static bool Dark
        {
            get
            {
                return Identity == null ? AppDark.In(DTObject.Empty) : Identity.GetValue<bool>("dark", false);
            }
            set
            {
                InitIdentity();
                Identity.SetValue("dark", value);
            }
        }

        public static bool IgnoreAuth
        {
            get
            {
                return Identity == null ? false : Identity.GetValue<bool>("ignoreAuth", false);
            }
            set
            {
                InitIdentity();
                Identity.SetValue("ignoreAuth", value);
            }
        }

        public static DTObject Data
        {
            get
            {
                return Identity == null ? DTObject.Empty : Identity.GetObject("data", DTObject.Empty);
            }
            set
            {
                InitIdentity();
                Identity.SetObject("data", value);
            }
        }


        //#if DEBUG
        //        public static bool IgnoreAuth
        //        {
        //            get
        //            {
        //                return Identity == null ? false : Identity.GetValue<bool>("ignoreAuth", false);
        //            }
        //            set
        //            {
        //                InitIdentity();
        //                Identity.SetValue("ignoreAuth", value);
        //            }
        //        }

        //#endif


        //#if !DEBUG

        //        public static bool IgnoreAuth
        //        {
        //            get
        //            {
        //                return false;
        //            }
        //            set
        //            {
        //                //什么也不执行
        //            }
        //        }

        //#endif



        #endregion
    }
}
