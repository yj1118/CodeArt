using System;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Log;
using CodeArt.DTO;

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
                    LogWrapper.Default.Fatal(ex);
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
        }


        /// <summary>
        /// 清理当前回话数据
        /// </summary>
        public static void Dispose()
        {
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
                CodeArt.Language.Init(); //初始化语言选项，因为身份可能含有语言信息，所以需要初始化
                if (IdentityChanged != null)
                    IdentityChanged(value);
            }
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
                return Identity == null ? string.Empty : Identity.GetValue<string>("name", string.Empty);
            }
        }


        #endregion
    }
}
