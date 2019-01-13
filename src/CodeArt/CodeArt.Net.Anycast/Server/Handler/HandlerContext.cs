using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Net.Anycast
{
    public sealed  class HandlerContext
    {
        internal bool IsCompleted
        {
            get;
            set;
        }

        private HandlerContext()
        {
            this.IsCompleted = false;
        }


        /// <summary>
        /// 指示通道的请求处理已完成，这意味着不再对消息调用服务端默认的处理，比如转发等
        /// </summary>
        public void CompleteRequest()
        {
            this.IsCompleted = true;
        }

        public void Reset()
        {
            this.IsCompleted = false;
        }

        internal static Pool<HandlerContext> Pool = new Pool<HandlerContext>(() =>
        {
            return new HandlerContext();
        }, (ctx, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                ctx.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 60 //闲置时间60秒
        });


    }
}
