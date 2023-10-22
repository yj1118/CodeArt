using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using CodeArt.EasyMQ.Event;
using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.EasyMQ;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 远程服务提供者的基类
    /// </summary>
    internal abstract class RemoteObjectHandler : IEventHandler
    {
        public EventPriority Priority => EventPriority.High;

        public void Handle(string eventName, TransferData data)
        {
            var arg = data.Info;
            AppSession.Identity = arg.GetObject("identity"); //先初始化身份
            Handle(arg);
        }

        protected abstract void Handle(DTObject arg);

        #region 辅助方法

        protected void UseDefines(DTObject arg, Action<AggregateRootDefine, object> action)
        {
            var typeName = arg.GetValue<string>("typeName");
            var defines = RemoteType.GetDefines(typeName);
            foreach (var define in defines)
            {
                var idProperty = DomainProperty.GetProperty(define.MetadataType, EntityObject.IdPropertyName);
                var id = DataUtil.ToValue(arg.GetValue(EntityObject.IdPropertyName), idProperty.PropertyType);
                action((AggregateRootDefine)define, id);
            }
        }

        #endregion

    }
}