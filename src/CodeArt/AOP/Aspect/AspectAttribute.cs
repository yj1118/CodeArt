using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.Util;

namespace CodeArt.AOP
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AspectAttribute : Attribute
    {
        public Type AspectType
        {
            get;
            private set;
        }

        private object[] _args;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspectType">关注点的实现</param>
        public AspectAttribute(Type aspectType, params object[] args)
        {
            this.AspectType = aspectType;
            _args = args;
        }

        private static Func<MemberInfo, IAspect[]> _getAspects = LazyIndexer.Init<MemberInfo, IAspect[]>((member) =>
        {
            var attrs = GetAttributes(member);
            List<IAspect> stages = new List<IAspect>(attrs.Length);
            foreach (var attr in attrs)
            {
                stages.Add(attr.CreateAspect());
            }
            return stages.ToArray();
        });

        public IAspect CreateAspect()
        {
            var aspect = Activator.CreateInstance(this.AspectType, _args) as IAspect;
            if (aspect == null) throw new TypeMismatchException(this.AspectType, typeof(IAspect));
            return aspect;
        }

        public static IAspect[] GetAspects(MemberInfo member)
        {
            return _getAspects(member);
        }

        /// <summary>
        /// 以切面的方式执行方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="info"></param>
        public static void Invoke(MethodInfo method, object self, params object[] args)
        {
            var aspects = GetAspects(method);
            if (aspects != null)
            {
                foreach (var aspect in aspects)
                {
                    aspect.Before();
                }
            }

            try
            {
                method.Invoke(self, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (aspects != null)
            {
                foreach (var aspect in aspects)
                {
                    aspect.After();
                }
            }
        }

        #region 辅助


        private static AspectAttribute[] GetAttributes(MemberInfo member)
        {
            List<AspectAttribute> result = null;
            List<AspectRemoveAttribute> removes = null;

            var attributes = member.GetCustomAttributes();
            foreach(var attr in attributes)
            {
                var aspectAttr = attr as AspectAttribute;
                if (aspectAttr != null)
                {
                    if (result == null) result = new List<AspectAttribute>();//为了节省内存
                    result.Add(aspectAttr);
                }

                var removeAttr = attr as AspectRemoveAttribute;
                if (removeAttr != null)
                {
                    if (removes == null) removes = new List<AspectRemoveAttribute>();
                    removes.Add(removeAttr);
                }

            }

            if(result != null && removes != null)
            {
                for (var i = 0; i < result.Count; i++)
                {
                    var attr = result[i];
                    bool needRemove = false;
                    foreach(var remove in removes)
                    {
                        needRemove = remove.NeedRemove(attr);
                        if (needRemove) break;
                    }
                    if (needRemove)
                    {
                        result.RemoveAt(i);
                        i--;
                    }
                }
            }

            return result == null || result.Count ==0 ? AttributeEmpty : result.ToArray();
        }

        private static AspectAttribute[] AttributeEmpty = Array.Empty<AspectAttribute>();

        #endregion
    }
}
