using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

using CodeArt.AppSetting;

namespace CodeArt.Security
{
    /// <summary>
    /// 标记对象是否处于灰度状态
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DarkAttribute : AuthAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">过程名称</param>
        public DarkAttribute()
        {

        }


        public override bool Verify(DTObject data)
        {
            return data.GetValue<bool>("dark", false);
        }

        #region 辅助

        public static DarkAttribute GetTip(Type objectType)
        {
            return AttributeUtil.GetAttribute<DarkAttribute>(objectType);
        }

        #endregion
    }
}

