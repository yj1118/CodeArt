using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Security;

namespace CodeArt.Web.RPC
{
    [SafeAccess]
    public abstract class Procedure : IProcedure
    {
        /// <summary>
        /// 执行过程的路径
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        public ILogExtractor LogExtractor
        {
            get;
            set;
        }

        public virtual DTObject Invoke(DTObject arg)
        {
            return InvokeDynamic(arg);
        }

        protected virtual DTObject InvokeDynamic(dynamic arg)
        {
            return DTObject.Empty;
        }

        /// <summary>
        /// 默认情况下不适用缓存功能
        /// </summary>
        /// <returns></returns>
        public virtual ICache GetCache()
        {
            return null;
        }

        public virtual string GetCacheKey(DTObject arg)
        {
            return string.Empty;
        }

        public virtual string GetResponseCode(DTObject result)
        {
            return result.GetCode(false, false);
        }

        public virtual DTObject GetLogContent(DTObject arg)
        {
            var extractor = this.LogExtractor;
            if (extractor == null) return null;
            return extractor.GetContent(this, arg);
        }


        private IEnumerable<AuthAttribute> _auths;
        private object _syncObject = new object();

        public IEnumerable<AuthAttribute> Auths
        {
            get
            {
                if(_auths == null)
                {
                    lock(_syncObject)
                    {
                        if (_auths == null)
                        {
                            _auths = GetAuths();
                        }
                    }
                }
                return _auths;
            }
        }

        protected virtual IEnumerable<AuthAttribute> GetAuths()
        {
            List<AuthAttribute> auths = new List<AuthAttribute>();

            var dark = DarkAttribute.GetTip(this.GetType());
            if (dark != null) auths.Add(dark);

            var identities = IdentityAttribute.GetTips(this.GetType());
            if (identities != null) auths.AddRange(identities);

            return auths;
        }

        public virtual DTObject GetAuthData(DTObject arg)
        {
            //默认情况是将用户传递的全部参数作为需要验证的内容
            return arg;
        }
    }
}
