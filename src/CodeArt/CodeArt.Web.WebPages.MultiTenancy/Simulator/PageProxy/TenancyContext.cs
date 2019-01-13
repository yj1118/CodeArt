using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.AppSetting;
using System.Collections.Specialized;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// 因为该对象是从池中获取，所以该对象只能用于主线程
    /// </summary>
    public class TenancyContext
    {
        public string TenancyId
        {
            get;
            private set;
        }

        public RequestInfo Request
        {
            get;
            private set;
        }

        public IDictionary<string,object> Items
        {
            get;
            private set;
        }


        public T GetQueryValue<T>(string name, T defaultValue)
        {
            NameValueCollection queryValues = this.Request.QueryString;
            string value = queryValues[name];
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return DataUtil.ToValue<T>(value);
        }


        internal TenancyContext()
        {
            this.Request = new RequestInfo();
            this.Items = new Dictionary<string, object>();
        }

        /// <summary>
        /// 从环境代码中初始化环境
        /// </summary>
        /// <param name="contextCode"></param>
        internal void Initialize(string contextCode)
        {
            var dto = DTObject.Create(contextCode);
            this.TenancyId = dto.GetValue<string>("tenancy.id");
            this.Request.Initialize(dto);
        }

        internal void Clear()
        {
            this.Items.Clear();
            this.Request.QueryString.Clear();
        }

        public class RequestInfo
        {
            public string Type
            {
                get;
                private set;
            }

            public string RawUrl
            {
                get;
                set;
            }

            public string VirtualPath
            {
                get;
                set;
            }

            public bool IsMobileDevice
            {
                get;
                private set;
            }

            public string Extension
            {
                get;
                private set;
            }

            public NameValueCollection QueryString
            {
                get;
                private set;
            }

            public RequestInfo()
            {
                this.QueryString = new NameValueCollection();
            }

            internal void Initialize(DTObject context)
            {
                this.Type = context.GetValue<string>("request.type");
                this.RawUrl = context.GetValue<string>("request.url");
                this.VirtualPath = context.GetValue<string>("request.path");
                this.IsMobileDevice = context.GetValue<bool>("request.mobile");
                this.Extension = context.GetValue<string>("request.extension");
                context.EachDictionary("request.query", (key, value) =>
                {
                    this.QueryString.Add(key, value.ToString());
                });
            }
        }


        #region 静态成员

        //TenancyContext

        private const string _sessionKey = "__TenancyContext.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// </summary>
        public static TenancyContext Current
        {
            get
            {
                var current = AppSession.GetItem<TenancyContext>(_sessionKey);
                if (current == null) throw new AppSettingException("没有找到TenancyContext.Current");
                return current;
            }
            set
            {
                AppSession.SetItem<TenancyContext>(_sessionKey, value);
            }
        }


        #endregion


    }


    internal static class TenancyContextPool
    {
        public static IPoolItem<TenancyContext> Borrow()
        {
            return Instance.Borrow();
        }


        public static readonly Pool<TenancyContext> Instance = new Pool<TenancyContext>(() =>
        {
            return new TenancyContext();
        }, (context, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                context.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });
    }


}
