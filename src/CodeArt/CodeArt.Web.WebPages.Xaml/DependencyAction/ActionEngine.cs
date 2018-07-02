using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    public static class ActionEngine
    {
        /// <summary>
        /// 执行不需要参数的方法
        /// </summary>
        /// <returns></returns>
        public static object Execute(object[] args, Func<object> action)
        {
            CheckArgumentsLength(args, 0);
            return action();
        }

        /// <summary>
        /// 执行带1个参数的方法
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static object Execute<T0>(object[] args, Func<T0, object> action)
        {
            CheckArgumentsLength(args, 1);
            var arg0 = GetArgument<T0>(args, 0);
            return action(arg0);
        }

        /// <summary>
        /// 执行带2个参数的方法
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="args"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static object Execute<T0,T1>(object[] args, Func<T0, T1, object> action)
        {
            CheckArgumentsLength(args, 2);
            var arg0 = GetArgument<T0>(args, 0);
            var arg1 = GetArgument<T1>(args, 1);
            return action(arg0, arg1);
        }

        /// <summary>
        /// 执行带3个参数的方法
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="args"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static object Execute<T0, T1, T2>(object[] args, Func<T0, T1, T2, object> action)
        {
            CheckArgumentsLength(args, 3);
            var arg0 = GetArgument<T0>(args, 0);
            var arg1 = GetArgument<T1>(args, 1);
            var arg2 = GetArgument<T2>(args, 2);
            return action(arg0, arg1, arg2);
        }

        /// <summary>
        /// 执行带4个参数的方法
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="args"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static object Execute<T0, T1, T2, T3>(object[] args, Func<T0, T1, T2, T3, object> action)
        {
            CheckArgumentsLength(args, 3);
            var arg0 = GetArgument<T0>(args, 0);
            var arg1 = GetArgument<T1>(args, 1);
            var arg2 = GetArgument<T2>(args, 2);
            var arg3 = GetArgument<T3>(args, 3);
            return action(arg0, arg1, arg2, arg3);
        }

        public static object ExecuteScript(object[] args, Func<ScriptView, object> action)
        {
            return Execute<ScriptView>(args, action);
        }


        private static void CheckArgumentsLength(object[] args,int length)
        {
            if (args.Length != length) throw new ArgumentException("参数不正确，无法执行依赖行为");
        }

        private static T GetArgument<T>(object[] args, int index)
        {
            return DataUtil.ToValue<T>(args[index]);
        }


    }
}
