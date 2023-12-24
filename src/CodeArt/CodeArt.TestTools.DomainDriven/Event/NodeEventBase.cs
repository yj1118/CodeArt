using System;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace CodeArt.TestTools.DomainDriven
{
    /// <summary>
    /// 为测试提供节点事件的支持，每个节点事件可以得到指令，完成其测试行为
    /// </summary>
    public abstract class NodeEventBase : DomainEvent
    {
        [EventArg]
        public string Code
        {
            get;
            set;
        }

        public NodeEventBase()
        {

        }

        protected override IEnumerable<string> GetPreEvents()
        {
            const string prefix = "NodeEvent";

            var index = int.Parse(this.EventName.Substring(prefix.Length));
            List<string> ems = new List<string>();

            var dto = DTObject.Create(this.Code);
            var pre = dto.GetList("pre", false);
            if (pre != null)
            {
                foreach (var e in pre)
                {
                    index++;
                    var em = string.Format("{0}{1}", prefix, index);
                    ems.Add(em);
                }
            }
            return ems;
        }

        protected override void RaiseImplement()
        {
            var dto = DTObject.Create(this.Code);
            var raise = (NodeAction)dto.GetValue<byte>("raise", 1);

            switch (raise)
            {
                case NodeAction.Success:
                    {
                        Console.WriteLine(string.Format("{0} 2秒后完成触发", this.EventName));
                        Thread.Sleep(2000);
                        Console.WriteLine(string.Format("{0} 触发完成", this.EventName));
                        return;
                    }
                case NodeAction.Fail:
                    {
                        Console.WriteLine(string.Format("{0} 2秒后触发失败", this.EventName));
                        Thread.Sleep(2000);
                        Console.WriteLine(string.Format("{0} 触发失败", this.EventName));
                        throw new Exception("触发失败");
                    }
                case NodeAction.Breakdown:
                    {

                    }
                    break;
            }
            return;
        }

        protected override void ReverseImplement()
        {
            var dto = DTObject.Create(this.Code);
            var reverse = (NodeAction)dto.GetValue<byte>("reverse", 1);

            switch (reverse)
            {
                case NodeAction.Success:
                    {
                        Console.WriteLine(string.Format("{0} 2秒后完成回逆", this.EventName));
                        Thread.Sleep(2000);
                        Console.WriteLine(string.Format("{0} 回逆完成", this.EventName));
                        return;
                    }
                case NodeAction.Fail:
                    {
                        Console.WriteLine(string.Format("{0} 2秒后回逆失败", this.EventName));
                        Thread.Sleep(2000);
                        Console.WriteLine(string.Format("{0} 回逆失败", this.EventName));
                        throw new Exception("回逆失败");
                    }
                case NodeAction.Breakdown:
                    {

                    }
                    break;
            }
        }

    }
}
