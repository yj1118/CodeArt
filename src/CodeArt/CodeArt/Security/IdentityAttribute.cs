using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.Security
{
    /// <summary>
    /// 基于身份的授权验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class IdentityAttribute : AuthAttribute
    {
        /// <summary>
        /// 访问该对象所需要的权限
        /// </summary>
        public string Permission
        {
            get;
            set;
        }

        /// <summary>
        /// 访问该对象所需要的角色权限
        /// </summary>
        public string Role
        {
            get;
            set;
        }

        /// <summary>
        /// 该权限需要访问的资源名称，如果为空，那么就不与任何资源绑定，仅考虑权限自身的验证即可
        /// </summary>
        public string Resource
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">过程名称</param>
        public IdentityAttribute()
        {

        }


        public override bool Verify(DTObject data)
        {
            var authorizer = AuthorizerFactory<IdentityAttribute>.Create();
            return authorizer.Verify(this, data);
        }



        #region 辅助

        public static IEnumerable<AuthAttribute> GetTips(Type objectType)
        {
            return AttributeUtil.GetAttributes<AuthAttribute>(objectType);
        }

        #endregion
    }
}

