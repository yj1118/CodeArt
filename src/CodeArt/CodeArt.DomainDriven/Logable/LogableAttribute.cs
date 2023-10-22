using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示命令具备日志的能力
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class LogableAttribute : Attribute
    {
        /// <summary>
        /// 日志记录的操作名称
        /// </summary>
        public string ActionName
        {
            get;
            private set;
        }

        private Type _commandType;

        /// <summary>
        /// 命令的类型
        /// </summary>
        internal Type CommandType
        {
            get
            {
                return _commandType;
            }
            private set
            {
                _commandType = value;
                if (this.ActionName == null)
                {
                    this.ActionName = _commandType.Name;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LogableAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName">行为名称，如果不指定，那么就是命令的类名</param>
        public LogableAttribute(string actionName)
        {
            this.ActionName = actionName;
        }

        /// <summary>
        /// 在程序启动的时候分析所有日志特性，记录信息
        /// </summary>
        internal static void Initialize()
        {
            var types = GetLogableTypes();

            foreach (var commandType in types)
            {
                var tip = Create(commandType);
                if (tip != null)
                {
                    _tips.Add(tip);
                }
            }
        }

        #region  执行时

        private static IEnumerable<Type> GetLogableTypes()
        {
            return AssemblyUtil.GetTypesByAttribute<LogableAttribute>();
        }

        #endregion


        private static LogableAttribute Create(Type commandType)
        {
            var tip = AttributeUtil.GetAttribute<LogableAttribute>(commandType);

            if (tip != null)
            {
                tip.CommandType = commandType;
            }
            return tip;
        }


        private readonly static List<LogableAttribute> _tips = new List<LogableAttribute>();

        /// <summary>
        /// 获得当前应用程序定义的所有具备日志能力的特性标签
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LogableAttribute> GetTips()
        {
            return _tips;
        }

        public static LogableAttribute GetTip(Type commandType)
        {
            return _getTip(commandType);
        }

        private static Func<Type, LogableAttribute> _getTip
                = LazyIndexer.Init<Type, LogableAttribute>((commandType) =>
                {
                    var result = _tips.FirstOrDefault((tip) => tip.CommandType == commandType);
                    return result;
                });
    }
}
