using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using System.Reflection;
using CodeArt.Runtime;
using CodeArt.Util;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WebLoadAttribute : Attribute
    {
        public LoadSequence Sequence
        {
            get;
            private set;
        }

        public LoadDevice Device
        {
            get;
            private set;
        }

        public WebLoadAttribute(LoadSequence sequence, LoadDevice device)
        {
            this.Sequence = sequence;
            this.Device = device;
        }

        public WebLoadAttribute(LoadSequence sequence)
            : this(sequence, LoadDevice.Both)
        {
        }

        #region 提取方法

        private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        private static Func<Type, IList<WebLoadMethod>> _getBeforeMethods = LazyIndexer.Init<Type, IList<WebLoadMethod>>((type) =>
        {
            return _getMethods(type, LoadSequence.Before);
        });

        private static Func<Type, IList<WebLoadMethod>> _getAfterMethods = LazyIndexer.Init<Type, IList<WebLoadMethod>>((type) =>
        {
            return _getMethods(type, LoadSequence.After);
        });

        private static IList<WebLoadMethod> _getMethods(Type type, LoadSequence sequence)
        {
            List<WebLoadMethod> result = new List<WebLoadMethod>();
            var methods = type.GetMethods(_flags);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<WebLoadAttribute>();
                if (attr != null && attr.Sequence == sequence)
                    result.Add(new WebLoadMethod(attr, method));
            }

            return result;
        }

        public static IList<WebLoadMethod> GetMethods(Type type, LoadSequence sequence)
        {
            return sequence == LoadSequence.Before ? _getBeforeMethods(type) : _getAfterMethods(type);
        }

        #endregion



        public sealed class WebLoadMethod
        {
            public WebLoadAttribute Config
            {
                get;
                private set;
            }

            public MethodInfo Method
            {
                get;
                private set;
            }

            public WebLoadMethod(WebLoadAttribute config, MethodInfo method)
            {
                this.Config = config;
                this.Method = method;
            }

            public void Execute(WebPage page, params object[] args)
            {
                if (this.Config.Device != LoadDevice.Both)
                {
                    if (this.Config.Device == LoadDevice.PC && page.IsMobileDevice) return;//仅PC端访问
                    if (this.Config.Device == LoadDevice.Mobile && !page.IsMobileDevice) return;//仅手机端访问
                }

                AspectAttribute.Invoke(this.Method, page, args);
            }

        }

    }
}
