using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Hosting;

using CodeArt.AOP;
using CodeArt.Util;

namespace CodeArt.Web.WebPages
{
    public sealed class WebPageContext
    {
        private HttpContext _httpContext;
        public HttpContext HttpContext
        {
            get { return _httpContext; }
        }

        private WebPage _page;
        public WebPage Page
        {
            get { return _page; }
            internal set 
            {
                if(!this.AnErrorOccurred)
                    _page = value; 
            }
        }

        private HttpRequest _request;
        /// <summary>
        /// Http请求对象
        /// </summary>
        public HttpRequest Request
        {
            get { return _request; }
        }

        private HttpResponse _response;
        /// <summary>
        /// Http响应对象
        /// </summary>
        public HttpResponse Response
        {
            get { return _response; }
        }


        private ResolveRequestCache _cache;
        /// <summary>
        /// 处理缓存的对象
        /// </summary>
        public ResolveRequestCache Cache
        {
            get
            {
                return _cache;
            }
        }

        public string RequestType
        {
            get
            {
                return this.Request.RequestType;
            }
        }

        /// <summary>
        /// 是GET请求
        /// </summary>
        public bool IsGET
        {
            get
            {
                return this.RequestType == "GET";
            }
        }

        public bool IsErrorPage
        {
            get;
            private set;
        }

        private string _virtualPath;
        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get { return _virtualPath; }
            private set 
            { 
                _virtualPath = value;
                _pathExtension = WebUtil.GetPathExtension(_virtualPath);
            }
        }

        private string _pathExtension;
        public string PathExtension
        {
            get { return _pathExtension; }
        }

        private IAspect[] _pageAspects;

        /// <summary>
        /// 页面级别的关注点
        /// </summary>
        public IAspect[] PageAspects
        {
            get
            {
                if (_pageAspects == null) _pageAspects = WebPageRouter.GetAspects(this);
                return _pageAspects;
            }
        }


        public T GetQueryValue<T>(string name, T defaultValue)
        {
            NameValueCollection queryValues = GetQueryValues();
            string value = queryValues[name];
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return DataUtil.ToValue<T>(value);
        }

        public NameValueCollection GetQueryValues()
        {
            var queryValues = GetItem<NameValueCollection>("__queryValues", () =>
            {
                return WebUtil.ProcessQueryString(this.Request.QueryString);
            });
            return queryValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetItem(string name, object value)
        {
            this.Items[name] = value;
        }

        public T GetItem<T>(string name)
        {
            return (T)this.Items[name];
        }

        public T GetItem<T>(string name, Func<T> createValue)
        {
            T value = default(T);
            if (!this.Items.Contains(name))
            {
                value = createValue();
                SetItem(name, value);
            }
            else 
                value = (T)this.Items[name];
            return value;
        }


        private HybridDictionary _items = null;

        public HybridDictionary Items
        {
            get
            {
                if (_items == null) _items = new HybridDictionary();
                return _items;
            }
        }


        public WebPageContext(HttpContext context)
        {
            //为了提高性能，将常用对象单独作为私有成员保存，而不是通过 HttpContext.Current.XXX来访问
            //通过反射可以知道HttpContext.Current.XXX里面有性能损耗
            _httpContext = context;
            _request = context.Request;
            _response = context.Response;
            this.VirtualPath = GetPathValue(context);
            this.IsErrorPage = this.VirtualPath.Equals("/error.htm", StringComparison.OrdinalIgnoreCase);
            _cache = new ResolveRequestCache(this);
        }

        private static string GetPathValue(HttpContext context)
        {
            return context.Request.AppRelativeCurrentExecutionFilePath.Substring(1) + context.Request.PathInfo;
        }

        private WebPageContext(){}

        public static readonly WebPageContext Emtpy = new WebPageContext();


        public static WebPageContext Current
        {
            get
            {
                return HttpContext.Current.Items["__webPageContext"] as WebPageContext;
            }
            internal set
            {
                HttpContext.Current.Items["__webPageContext"] = value;
            }
        }

        public bool IsValidPath()
        {
            return this.Preprocessor.IsValidPath(this.VirtualPath);
        }

        private ICodePreprocessor _preprocessor;
        internal ICodePreprocessor Preprocessor
        {
            get
            {
                if (_preprocessor == null) _preprocessor = VirtualPathPreprocessor.Instance;
                return _preprocessor;
            }
        }


        #region 页面级配置


        private WebPageSetting _setting;
        private bool _settingLoaded = false;
        public T GetConfigValue<T>(string configName, string argName, T defaultValue)
        {
            LoadSetting();
            string value = _setting == null ? null : _setting.GetConfigValue(configName, argName);
            return value == null ? defaultValue : DataUtil.ToValue<T>(value);
        }

        public bool ContainsConfig(string configName, string argName)
        {
            LoadSetting();
            if (_setting == null) return false;
            if (_setting.GetConfigValue(configName, argName) == null) return false;
            return true;
        }

        private void LoadSetting()
        {
            if (!_settingLoaded)
            {
                _setting = this.Preprocessor.ReadSetting(this.VirtualPath);
                _settingLoaded = true;
            }
        }

        #endregion

        public void Redirect(string url)
        {
            throw new RedirectException(url);
        }

        public bool AnErrorOccurred
        {
            get;
            private set;
        }

        public void SetError(Exception ex, int statusCode)
        {
            this.Page = new WebPageError(ex, statusCode);
            this.AnErrorOccurred = true;
        }

        public void SetError(Exception ex)
        {
            if (ex is RedirectException) throw ex; //如果是跳转，则给更高级别的错误处理来处理
            var httpEx = ex as HttpException;
            this.Page = httpEx == null ? new WebPageError(ex) : new WebPageError(ex, httpEx.GetHttpCode());
            this.AnErrorOccurred = true;
        }

        #region 移动端支持

        private ClientDevice _device;

        /// <summary>
        /// 移动端信息
        /// </summary>
        public ClientDevice Device
        {
            get
            {
                if (_device == null)
                {
                    var detector = ClientDeviceRegister.CreateDetector();
                    _device = detector.Detect(this);
                }
                return _device;
            }
        }

        public bool IsMobileDevice
        {
            get
            {
                return this.Device.IsMobile;
            }
            set
            {
                this.Device.IsMobile = value;
            }
        }

        #endregion

        #region 压缩

        private HttpCompressionType _compressionType = 0;

        /// <summary>
        /// 当前请求的压缩类型
        /// </summary>
        public HttpCompressionType CompressionType
        {
            get
            {
                if (_compressionType == 0)
                {
                    _compressionType = GetCompressionMode(this);
                }
                return _compressionType;
            }
        }

        private static HttpCompressionType GetCompressionMode(WebPageContext context)
        {
            var request = context.Request;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding))
            {
                return HttpCompressionType.None;
            }
            acceptEncoding = acceptEncoding.ToUpperInvariant();
            if (acceptEncoding.Contains("GZIP"))
            {
                return HttpCompressionType.GZip;
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                return HttpCompressionType.Deflate;
            }
            else
            {
                return HttpCompressionType.None;
            }
        }

        #endregion

    }
}
