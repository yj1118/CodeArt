using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 该对象是线程安全地
    /// </summary>
    /// <typeparam name="Q"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class ModuleHandlerProxy<Q,S>
        where Q : class
        where S : class
    {
        private string _handlerKey = null;
        private IModuleHandler<Q, S> _handler = null;

        public ModuleHandlerProxy(string handlerKey,IModuleHandler<Q,S> handler)
        {
            _handlerKey = handlerKey;
            _handler = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I">输入参数的类型</typeparam>
        /// <typeparam name="O">输出参数的类型</typeparam>
        /// <param name="arg">输入参数</param>
        /// <returns></returns>
        public DTObject Process(DTObject arg)
        {
            var request = MapRequest(arg);
            var response = _handler.Process(request);
            return MapResponse(response);
        }

        public DTObject Process(Action<DTObject> fillArg)
        {
            var arg = DTObject.CreateReusable();
            fillArg(arg);
            return Process(arg);
        }

        /// <summary>
        /// 将输入参数映射为请求参数
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Q MapRequest(DTObject arg)
        {
            if (typeof(Q) == typeof(DTObject)) return arg as Q;
            return DTObject.Deserialize<Q>(arg);
        }

        /// <summary>
        /// 将响应参数映射为输出参数
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private DTObject MapResponse(S response)
        {
            if (typeof(S) == typeof(DTObject)) return response as DTObject;
            return DTObject.Serialize(response, false);
        }
    }
}
